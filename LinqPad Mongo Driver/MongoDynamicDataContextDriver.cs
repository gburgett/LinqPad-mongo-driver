using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.Services.Client;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using LINQPad.Extensibility.DataContext;
using Microsoft.CSharp;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace GDSX.Externals.LinqPad.Driver
{
    public class MongoDynamicDataContextDriver : DynamicDataContextDriver
    {
        /// <summary>
        /// Types common to all Bson driver instances
        /// </summary>
        public static Dictionary<string, Type> CommonTypes = new[]
            {
                typeof(BsonDocument)
            }.ToDictionary(t => t.ToString());
        
        private static readonly PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));

        public override object[] GetContextConstructorArguments(IConnectionInfo cxInfo)
        {
            var props = new ConnectionProperties();
            props.Deserialize(cxInfo.DriverData);

            // dynamically create a Mongo database object
            object mongo = MongoServer.Create(props.ConnectionString);

            return new[] { mongo };
        }

        public override ParameterDescriptor[] GetContextConstructorParameters(IConnectionInfo cxInfo)
        {
            return new[] { new ParameterDescriptor("mongo", typeof(MongoServer).FullName), };
        }

        public override string GetConnectionDescription(IConnectionInfo cxInfo)
        {
            return cxInfo.DisplayName;
        }

        public override bool ShowConnectionDialog(IConnectionInfo cxInfo, bool isNewConnection)
        {
            var props = new ConnectionProperties();
            if(!isNewConnection)
            {
                props.Deserialize(cxInfo.DriverData);
            }

            using(var form = new ConnectionDialog(props, isNewConnection, LoadAssemblySafely))
            {
                var result = form.ShowDialog();
   
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    props.Serialize(cxInfo.DriverData);
                    cxInfo.DisplayName = string.Format("{0} ({1})", props.SelectedDatabase, props.ConnectionString);
                    return true;
                }
            }

            return false;
        }

        public override string Name
        {
            get { return "Mongo BSON Driver"; }
        }

        public override string Author
        {
            get { return "Gordon Burgett"; }
        }

        public override List<ExplorerItem> GetSchemaAndBuildAssembly(IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
        {
            var props = new ConnectionProperties();
            props.Deserialize(cxInfo.DriverData);
            
            List<Assembly> assemblies = 
                props.AssemblyLocations.Select(LoadAssemblySafely).ToList();

            var code = new[] { GenerateDynamicCode(props, assemblies, nameSpace, typeName) }
                        .Concat(GetStaticCodeFiles());

            BuildAssembly(props, code, assemblyToBuild, GetDriverFolder);

            return BuildSchema(props, assemblies);
        }

        public List<ExplorerItem> BuildSchema(ConnectionProperties props, List<Assembly> assemblies)
        {
            
            List<ExplorerItem> ret = new List<ExplorerItem>();

            List<ExplorerItem> UntypedCollections = new List<ExplorerItem>();
            List<ExplorerItem> TypedCollections = new List<ExplorerItem>();

            ret.Add(new ExplorerItem("db", ExplorerItemKind.QueryableObject, ExplorerIcon.LinkedDatabase));
            
            foreach (var ctm in props.CollectionTypeMappings[props.SelectedDatabase])
            {
                ExplorerItem coll = null;
                Type type = null;
                if (!string.IsNullOrEmpty(ctm.CollectionType))
                {
                    type = this.TryLoadType(assemblies, ctm.CollectionType);
                }

                if (type != null)
                {
                    var name = DoPluralize(ctm.CollectionName);

                    coll = new ExplorerItem(name,
                        ExplorerItemKind.QueryableObject, ExplorerIcon.Table);

                    foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        //ignore BSON ignored properties
                        if (info.GetCustomAttributes(typeof(BsonIgnoreAttribute), true).Any())
                            continue;

                        if (coll.Children == null)
                            coll.Children = new List<ExplorerItem>();

                        coll.Children.Add(new ExplorerItem(info.Name, ExplorerItemKind.Property, ExplorerIcon.Column));
                    }

                    ret.Add(coll);

                    var tColl = new ExplorerItem(name + "Collection", ExplorerItemKind.CollectionLink, ExplorerIcon.View);
                    tColl.Children = coll.Children;
                    TypedCollections.Add(tColl);
                }
                else
                {
                    var name = DoPluralize(ctm.CollectionName);
                    UntypedCollections.Add(new ExplorerItem(name + "Collection", ExplorerItemKind.CollectionLink, ExplorerIcon.View));
                }
            }

            if (TypedCollections.Count > 0)
            {
                var item = new ExplorerItem("Collections", ExplorerItemKind.Category, ExplorerIcon.Box);
                item.Children = TypedCollections;
                ret.Add(item);
            }


            if(UntypedCollections.Count > 0)
            {
                var item = new ExplorerItem("Untyped Collections", ExplorerItemKind.Category, ExplorerIcon.Box);
                item.Children = UntypedCollections;
                ret.Add(item);
            }


            if (ret.Count == 0)
                throw new Exception("No databases mapped to types");
            return ret;
        }

        public string GenerateDynamicCode(ConnectionProperties props, List<Assembly> assemblies, string nameSpace, string typeName)
        {
            var writer = new StringWriter();

            // write namespace and opening brace 
            if (!string.IsNullOrEmpty(nameSpace))
            {
                writer.Write("namespace ");
                writer.WriteLine(nameSpace);
                writer.WriteLine("{");
            }

            // write using directives
            writer.WriteLine(
@"using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.IO;
using System;
using GDSX.Externals.LinqPad.Driver;
");

            // write type declaration
            writer.WriteLine(string.Format("public class {0}\n{{", typeName));
            writer.WriteLine(string.Format("\tpublic {0}(MongoServer mongo)\n\t{{", typeName));
            writer.WriteLine(
@"     this.mongo = mongo;
    }

    private readonly MongoServer mongo;

    public TextWriter SqlTabWriter { get; set; }
");

            string db = props.SelectedDatabase;
                
            writer.WriteLine(string.Format(
                    "\tpublic MongoDatabase db {{ get {{ return this.mongo.GetDatabase(\"{0}\"); }} }}\n", db));

            HashSet<string> names = new HashSet<string>();
            List<string> initializations = new List<string>();

            foreach(var collection in props.CollectionTypeMappings[db])
            {
                var name = collection.CollectionName;
                Type type = null;
                if (!string.IsNullOrEmpty(collection.CollectionType) && !string.IsNullOrEmpty(name))
                {
                    type = this.TryLoadType(assemblies, collection.CollectionType);
                }

                var pluralizedName = DoPluralize(name);

                if (type == null)
                {
                    //writer.WriteLine(string.Format(
                    //    "\tpublic MongoCollection {1}Collection {{ get {{ return this.db.GetCollection(\"{0}\"); }} }}\r\n" +
                    //    "\tpublic IQueryable<{2}> {1} {{ get {{ return this.{1}Collection.AsQueryable<{2}>().Select(x => x); }} }}", name, pluralizedName, typeof(BsonDocument).FullName));
                }
                else
                {
                    names.Add(pluralizedName);
                    //Select(x => x) to get around bug in mongo query provider
                    writer.WriteLine(string.Format(
                        "\tprivate Interceptor<{0}> m{2};\r\n" + 
                        "\tpublic IQueryable<{0}> {2} {{ get {{ return this.m{2}.AsQueryable<{0}>().Select(x => x); }} }}\n", type.FullName, name, pluralizedName));

                    initializations.Add(string.Format("\t\tthis.m{2} = new Interceptor<{0}>(this.db.GetCollection<{0}>(\"{1}\"), this.SqlTabWriter);", type.FullName, name, pluralizedName));

                    writer.WriteLine(string.Format(
                        "\tpublic Interceptor<{0}> {2}Collection {{ get {{ return this.m{2}; }} }}\n", type.FullName, name, pluralizedName));

                }
            }

            writer.WriteLine(
@"    /// <summary>
    /// Submits all changes to queried objects by calling Save on the objects given by
    /// <see cref=""ToUpdate""/>
    /// </summary>
    public void SubmitChanges()
    {
        int modified = 0;");

            foreach(string name in names)
            {
                writer.WriteLine(string.Format("\t\tmodified += this.m{0}.SubmitChanges();", name));
            }

                writer.WriteLine(
@"    
        if (modified == 0 && !TrackChanges){
            Console.WriteLine(""No changes were submitted.  Did you mean to turn on TrackChanges?"");
        }
    }");

            writer.WriteLine(
@"    private bool mTrackChanges = false;
    /// <summary>
    /// If true, changes made to collection objects will be tracked and can be updated
    /// using SubmitChanges.  Defaults to off to avoid unnecessary serialization.
    /// </summary>
    public bool TrackChanges
    {
        get
        {
            return mTrackChanges;
        }
        set
        {
            mTrackChanges = value;
");

            foreach (string name in names)
            {
                writer.WriteLine(string.Format("\t\t\tthis.m{0}.TrackChanges = value;", name));
            }

            writer.WriteLine(
@"        }
    }");

                writer.WriteLine(
@"    public void InitCollections()
    {");
            foreach(string init in initializations)
            {
                writer.WriteLine(init);
            }
            
            writer.WriteLine("\t}");

                
            // write type closing brace 
            writer.WriteLine("}");

            // write namespace closing brace 
            if (!string.IsNullOrEmpty(nameSpace))
                writer.WriteLine("}");
            
            return writer.ToString();
        }
        
        public IEnumerable<string> GetStaticCodeFiles()
        {
            var ass = Assembly.GetExecutingAssembly();
            string[] files = { "GDSX.Externals.LinqPad.Driver.InterceptorCollection.cs" };
            return files.Select(x =>
                                    {
                                        var stream = ass.GetManifestResourceStream(x);
                                        if (stream == null)
                                            throw new Exception("Could not find static code files");
                                        using (var reader = new StreamReader(stream))
                                            return reader.ReadToEnd();
                                    });
        }

        private string DoPluralize(string s)
        {
            var split = s.Split('.');
            //pluralize the last word
            split[split.Length - 1] = pluralizationService.Pluralize(split[split.Length - 1]);
            return String.Join("_", split);
        }

    
        private Type TryLoadType(IEnumerable<Assembly> assemblies, string typeName)
        {
            string nameToLoad = typeName.Split(',').First();

            Type ret;
            if (CommonTypes.TryGetValue(nameToLoad, out ret))
                return ret;

            Assembly ass = assemblies.FirstOrDefault(x => x.GetType(nameToLoad) != null);
            if (ass != null)
                return ass.GetType(nameToLoad);
            
            return Type.GetType(nameToLoad);
        }

        public override void TearDownContext(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager, object[] constructorArguments)
        {
            ((MongoServer)constructorArguments[0]).Disconnect();
        }

        public void BuildAssembly(ConnectionProperties props, IEnumerable<string> code, AssemblyName name, Func<string> GetDriverFolder)
        {
            // Use the CSharpCodeProvider to compile the generated code:
            CompilerResults results;
            using (var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } }))
            {
                var assemblyNames = new List<string>();
                assemblyNames.AddRange("System.dll System.Core.dll".Split());
                assemblyNames.AddRange(props.AssemblyLocations);
                assemblyNames.Add(Path.Combine(GetDriverFolder(), "MongoDB.Driver.dll"));
                assemblyNames.Add(Path.Combine(GetDriverFolder(), "MongoDB.Bson.dll"));
                
                var options = new CompilerParameters(
                    assemblyNames.ToArray(),
                    name.CodeBase,
                    true);
                results = codeProvider.CompileAssemblyFromSource(options, code.ToArray());
            }
            if (results.Errors.Count > 0)
                throw new Exception
                    ("Cannot compile typed context: " + results.Errors[0].ErrorText + " (line " + results.Errors[0].Line + ")");
        }

        public override IEnumerable<string> GetAssembliesToAdd(IConnectionInfo cxInfo)
        {
            ConnectionProperties props = new ConnectionProperties();
            props.Deserialize(cxInfo.DriverData);

            return new []
                       {
                            Path.Combine(GetDriverFolder(), "MongoDB.Driver.dll"),
                            Path.Combine(GetDriverFolder(), "MongoDB.Bson.dll")
                       }
                       .Concat(props.AssemblyLocations);
        }

        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo)
        {
            ConnectionProperties props = new ConnectionProperties();
            props.Deserialize(cxInfo.DriverData);

            return new []
                {
                    "MongoDB.Driver.Builders",
                    "MongoDB.Driver",
                }.Concat(
            props.CollectionTypeMappings[props.SelectedDatabase].Select(x => x.CollectionType)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(name =>
                            {
                                name = name.Split(',').First();
                                int lastDot = name.LastIndexOf('.');
                                if (lastDot <= -1)
                                    return null;

                                return name.Substring(0, lastDot);
                            })
                .Where(x => !string.IsNullOrEmpty(x)))
                .Distinct();
        }

        public override void InitializeContext(IConnectionInfo cxInfo, object context, QueryExecutionManager executionManager)
        {
            
            base.InitializeContext(cxInfo, context, executionManager);

            PropertyInfo pinf = context.GetType().GetProperty("SqlTabWriter", BindingFlags.Instance | BindingFlags.Public);
            pinf.SetValue(context, executionManager.SqlTranslationWriter, null);

            MethodInfo init = context.GetType().GetMethod("InitCollections", BindingFlags.Instance | BindingFlags.Public);
            init.Invoke(context, new object[] { });

            var props = new ConnectionProperties();
            props.Deserialize(cxInfo.DriverData);

            if(props.CustomSerializers != null)
            {
                var assemblies = props.AssemblyLocations.Select(LoadAssemblySafely).ToList();
                foreach (var pair in props.CustomSerializers)
                {
                    var type = assemblies.Select(a => a.GetType(pair.Key)).FirstOrDefault(x => x != null);
                    var serializer = assemblies.Select(a => a.GetType(pair.Value)).FirstOrDefault(x => x != null);
                    if (type == null || serializer == null)
                        return;
                        //throw new Exception(string.Format("Unable to initialize custom serializer {0} for type {1}", pair.Value, pair.Key));

                    BsonSerializer.RegisterSerializer(type, (IBsonSerializer)Activator.CreateInstance(serializer));
                }
            }
                
        }
    }

   
}
