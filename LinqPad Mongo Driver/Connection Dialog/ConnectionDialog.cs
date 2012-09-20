using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CSharp;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GDSX.Externals.LinqPad.Driver
{
    public partial class ConnectionDialog : Form
    {
        private ConnectionProperties mConnection;
        private ConnectionAdditionalOptions mAdditionalOptions;
        private LinqPadQuery mInitializationQuery;

        private readonly bool mIsNewConnection;

        private bool mCloseAlreadyValidated = false;

        private Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();
        private readonly Dictionary<Type, Type> SerializerMappings = new Dictionary<Type, Type>();
        private readonly SortedSet<Type> LoadedTypes = new SortedSet<Type>(
            MongoDynamicDataContextDriver.CommonTypes.Values,
            new DynamicComparer<Type>((t1, t2) => string.Compare(t1.FullName, t2.FullName, false)));
                                                  
        private readonly Func<string, Assembly> loadSafely;

        private Dictionary<string, HashSet<CollectionTypeMapping>> mDatabases = new Dictionary<string, HashSet<CollectionTypeMapping>>();

        private readonly IXElementSerializer<ConnectionProperties> connectionPropertiesSerializer = new ConnectionPropertiesSerializer();

        public ConnectionDialog(ConnectionProperties props, bool isNewConnection, Func<string, Assembly> loadSafely)
        {
            this.mConnection = props;
            this.mAdditionalOptions = props.AdditionalOptions.Clone();

            this.mIsNewConnection = isNewConnection;
            this.loadSafely = loadSafely;

            InitializeComponent();

            this.SetLoadedQueryName(null);

            if (!this.mIsNewConnection)
                this.LoadFrom(props);
            else
                this.UpdateLoadedAssemblies();
        }

        //Loads all the exported types from the currently loaded assemblies
        public void LoadTypes()
        {
            if (string.IsNullOrWhiteSpace(this.txtConnectionString.Text))
                return;

            var connString = this.txtConnectionString.Text.Trim();

            var mongo = MongoServer.Create(connString);
            try
            {
                mongo.TrimDatabaseMappings(this.mDatabases);
            }
            finally
            {
                mongo.Disconnect();
            }

            string selectedDb = (string)this.cbDatabases.SelectedItem;
            this.cbDatabases.Items.Clear();
            this.cbDatabases.Items.AddRange(this.mDatabases.Keys.Cast<object>().ToArray());
            if(selectedDb != null && this.mDatabases.ContainsKey(selectedDb))
            {
                this.cbDatabases.SelectedItem = selectedDb;
            }
            else
            {
                this.cbDatabases.SelectedItem = this.mDatabases.Keys.FirstOrDefault();
            }
            
            if (this.cbDatabases.SelectedItem != null)
                this.dgCollectionTypes.DataSource = this.mDatabases[(string)this.cbDatabases.SelectedItem].Select(m => new TypeMappingWrapper(m)).ToList();
            else
                this.dgCollectionTypes.DataSource = null;
        }

        #region event handlers
        private void ConnectionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!this.mCloseAlreadyValidated)
            {
                e.Cancel = !this.DoCancel();
                this.mCloseAlreadyValidated = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(this.DoSave())
            {
                this.mCloseAlreadyValidated = true;
                this.Close();
            }

            this.mCloseAlreadyValidated = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(this.DoCancel())
            {
                this.mCloseAlreadyValidated = true;
                this.Close();
            }

            this.mCloseAlreadyValidated = false;
        }

        private void btnAddAssembly_Click(object sender, EventArgs e)
        {
            using (var chooser = new OpenFileDialog())
            {
                chooser.ShowDialog();


                foreach (var ass in chooser.FileNames)
                {
                    var loaded = this.loadSafely(ass);
                    this.LoadedAssemblies.Add(ass, loaded);
                    this.LoadedTypes.AddRange(loaded.GetExportedTypes());
                }


                UpdateLoadedAssemblies();
            }
        }

        private void btnRemoveAssembly_Click(object sender, EventArgs e)
        {
            foreach (LoadedAssemblyWrapper ass in lbLoadedAssemblies.SelectedItems)
            {
                this.LoadedAssemblies.Remove(ass.Location);
                if (ass.Assembly != null)
                {
                    var assTypes = new HashSet<Type>(ass.Assembly.GetExportedTypes());
                    this.LoadedTypes.RemoveWhere(assTypes.Contains);
                }
            }

            UpdateLoadedAssemblies();
        }

        private void lbLoadedAssemblies_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = this.lbLoadedAssemblies.SelectedItem as LoadedAssemblyWrapper;
            if (item == null)
                return;

            if(item.Assembly == null)
            {
                using(var frm = new OpenFileDialog())
                {
                    frm.FileName = item.Location;
                    var result = frm.ShowDialog();
                    if(result == DialogResult.OK)
                    {
                        if(File.Exists(frm.FileName))
                        {
                            try
                            {
                                var assembly = loadSafely(frm.FileName);
                                this.LoadedTypes.AddRange(assembly.GetExportedTypes());
                                this.LoadedAssemblies[frm.FileName] = assembly;
                                this.LoadedAssemblies.Remove(item.Location);
                            }catch(Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }

                        UpdateLoadedAssemblies();
                    }
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.LoadTypes();
        }

        private void tvKnownTypes_DoubleClick(object sender, EventArgs e)
        {
            if(tvKnownTypes.SelectedNode != null && tvKnownTypes.SelectedNode.Tag != null)
            {
                foreach (var row in this.dgCollectionTypes.SelectedCells.Cast<DataGridViewCell>().Where(x => string.Equals(x.OwningColumn.Name, "CollectionType")))
                {
                    row.Value = ((Type)tvKnownTypes.SelectedNode.Tag).ToString();
                }
            }
                
        }

        private void btnAddSerializer_Click(object sender, EventArgs e)
        {
            var types = this.LoadedTypes.Where(x => typeof(IBsonSerializer).IsAssignableFrom(x));
            using (var frm = new CustomSerializerSelector(MakeTree(this.LoadedTypes).ToArray(), (MakeTree(types).ToArray())))
            {
                var result = frm.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.SerializerMappings[frm.SelectedType] = frm.SelectedSerializer;
                    this.lbCustomSerializers.Items.Clear();
                    this.lbCustomSerializers.Items.AddRange(
                        this.SerializerMappings.Select(pair => new SerializerMappingWrapper
                        {
                            Type = pair.Key,
                            Serializer = pair.Value
                        }).Cast<object>().ToArray());
                }
            }
        }

        private void btnRemoveSerializers_Click(object sender, EventArgs e)
        {
            if (this.lbCustomSerializers.SelectedItem == null)
                return;

            var item = (SerializerMappingWrapper)this.lbCustomSerializers.SelectedItem;
            this.SerializerMappings.Remove(item.Type);
            this.lbCustomSerializers.Items.Remove(item);
        }
        
        private void cbDatabases_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.cbDatabases.SelectedItem != null)
                this.dgCollectionTypes.DataSource = this.mDatabases[(string)this.cbDatabases.SelectedItem].Select(m => new TypeMappingWrapper(m)).ToList();
            else
                this.dgCollectionTypes.DataSource = null;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var chooser = new SaveFileDialog())
                {
                    var result = chooser.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        var doc = new XDocument(
                                new XDeclaration("1.0", "UTF-8", "yes")
                            );
                        var el = new XElement("Properties");
                        var working = new ConnectionProperties();
                        this.Populate(working);

                        connectionPropertiesSerializer.Serialize(el, working);
                        doc.Add(el);

                        using (var writer = System.Xml.XmlWriter.Create(chooser.OpenFile(), new System.Xml.XmlWriterSettings
                                                                                                {
                                                                                                    Indent = true,
                                                                                                    CloseOutput = true
                                                                                                }))
                        {

                            doc.WriteTo(writer);
                            writer.Flush();
                        }
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Export failed: " + ex);
            }
        }
        
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var chooser = new OpenFileDialog())
                {
                    chooser.DefaultExt = ".xml";
                    chooser.Filter = "Xml Files|*.xml|All Files|*.*";

                    var result = chooser.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        ConnectionProperties props;
                        using (var reader = System.Xml.XmlReader.Create(chooser.OpenFile(), new System.Xml.XmlReaderSettings
                                                                                                {
                                                                                                    CloseInput = true
                                                                                                }))
                        {
                            var doc = XDocument.Load(reader);
                            props = connectionPropertiesSerializer.Deserialize(doc.Element("Properties"));
                        }

                        this.LoadFrom(props);
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Import failed: " + ex.ToString());
            }
        }

        private void additionalOptionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConnectionProperties props = new ConnectionProperties();
            this.Populate(props);

            using (var frm = new AdditionalOptions(props, this.LoadedTypes))
            {
                frm.Name = "Additional Options";
                DialogResult result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.mAdditionalOptions = frm.SelectedOptions;
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnImport_Click(sender, e);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnExport_Click(sender, e);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mConnection = new ConnectionProperties();
            this.LoadFrom(this.mConnection);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnSave_Click(sender, e);
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.btnCancel_Click(sender, e);
        }

        private void createShellQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var props = new ConnectionProperties();
            this.Populate(props);

            string contents = this.CreateShellQuery(props);
            using (var chooser = new SaveFileDialog())
            {
                chooser.AddExtension = true;
                chooser.DefaultExt = ".linq";
                chooser.Filter = "LinqPad Queries|*.linq|All Files|*.*";

                DialogResult result = chooser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(chooser.OpenFile()))
                    {
                        writer.Write(contents);
                    }

                    this.ViewLocation(chooser.FileName);
                }
            }
        }

        private void loadQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinqPadQuery query;
            using (var chooser = new OpenFileDialog())
            {
                chooser.DefaultExt = ".linq";
                chooser.Filter = "LinqPad Queries|*.linq|All Files|*.*";

                DialogResult result = chooser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    query = LinqPadQuery.CreateFrom(chooser.FileName);
                }
                else
                {
                    return;
                }
            }

            var props = new ConnectionProperties();
            this.Populate(props);
            List<string> errors = ValidateLinqQuery(query, props);
            if (errors.Count == 0)
            {
                this.mInitializationQuery = query;
                this.SetLoadedQueryName(Path.GetFileName(this.mInitializationQuery.Location));
            }
            else
            {
                DisplayErrors(errors);
            }
        }


        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.mInitializationQuery == null)
            {
                MessageBox.Show("No query loaded");
                return;
            }
            if (!File.Exists(this.mInitializationQuery.Location))
            {
                MessageBox.Show(string.Format("File {0} no longer exists", this.mInitializationQuery.Location));
                return;
            }

            ViewLocation(this.mInitializationQuery.Location);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mInitializationQuery = null;
            this.SetLoadedQueryName(null);
        }

        private void reloadFromDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.mInitializationQuery == null)
                return;
            if(!File.Exists(this.mInitializationQuery.Location))
            {
                MessageBox.Show(string.Format("Query '{0}' no longer exists", this.mInitializationQuery.Location));
                return;
            }

            ConnectionProperties props = new ConnectionProperties();
            this.Populate(props);
            
            LinqPadQuery query = LinqPadQuery.CreateFrom(this.mInitializationQuery.Location);
            List<string> errors = this.ValidateLinqQuery(query, props);
            if(errors.Count != 0)
            {
                MessageBox.Show("Loaded query has errors: \r\n" +
                    string.Join("\r\n", errors));
                return;
            }

            this.mInitializationQuery = query;
            this.SetLoadedQueryName(Path.GetFileName(this.mInitializationQuery.Location));
        }

        #endregion


        private void SetLoadedQueryName(string text)
        {
            if (text == null)
            {
                this.noQueryLoadedToolStripMenuItem.Text = "No Query Loaded";
                foreach (var dropDownItem in this.noQueryLoadedToolStripMenuItem.DropDownItems.Cast<ToolStripItem>())
                {
                    dropDownItem.Visible = false;
                }
            }
            else
            {
                this.noQueryLoadedToolStripMenuItem.Text = text;
                foreach (var dropDownItem in this.noQueryLoadedToolStripMenuItem.DropDownItems.Cast<ToolStripItem>())
                {
                    dropDownItem.Visible = true;
                }
            }
        }

        private string CreateShellQuery(ConnectionProperties props)
        {
            var driver = new MongoDynamicDataContextDriver();

            //build the query XDocument
            var doc = new XDocument();
            var query = new XElement("Query");
            doc.Add(query);
            query.SetAttributeValue("Kind", "Program");
            query.SetElementValue("Reference", Path.Combine(driver.GetDriverFolder(), "LinqPadMongoDriver.dll"));
            foreach (string loc in props.AssemblyLocations)
            {
                var el = new XElement("Reference");
                el.SetValue(loc);
                query.Add(el);
            }
            foreach (string ns in MongoDynamicDataContextDriver.GetNamespacesToAdd(props)
                .Concat(new[]
                    {
                        "System",
                        "GDSX.Externals.LinqPad.Driver"
                    }))
            {
                var el = new XElement("Namespace");
                el.SetValue(ns);
                query.Add(el);
            }

            StringBuilder sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings{OmitXmlDeclaration = true}))
                doc.Save(writer);

            sb.AppendLine();


            var ass = Assembly.GetExecutingAssembly();
            using (var stream = ass.GetManifestResourceStream("GDSX.Externals.LinqPad.Driver.ShellInitQuery.linq"))
            {
                if (stream == null)
                    throw new Exception("Could not find static code files");
                using (var reader = new StreamReader(stream))
                    sb.Append(reader.ReadToEnd());
            }

            return sb.ToString();
        }



        private List<string> ValidateLinqQuery(LinqPadQuery query, ConnectionProperties props)
        {
            List<string> errors = new List<string>();

            StringBuilder sb = new StringBuilder();
            foreach (var ns in query.Namespaces)
            {
                sb.Append("using ").Append(ns).AppendLine(";");
            }
            sb.AppendFormat(@"
public class TestQuery
{{
    public TestQuery()
    {{

    }}

    {0}
}}", query.Query);

            var driver = new MongoDynamicDataContextDriver();
            CompilerResults results;
            using (var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } }))
            {
                var assemblyNames = new HashSet<string>(query.References, new AssemblyPathEqualityComparer());
                
                //add additional assemblies which may or may not have been overridden
                assemblyNames.AddRange("System.dll System.Core.dll".Split());
                assemblyNames.Add(Path.Combine(driver.GetDriverFolder(), "MongoDB.Driver.dll"));
                assemblyNames.Add(Path.Combine(driver.GetDriverFolder(), "MongoDB.Bson.dll"));
                assemblyNames.Add(Path.Combine(driver.GetDriverFolder(), "LinqPadMongoDriver.dll"));

                var options = new CompilerParameters(assemblyNames.ToArray());
                options.GenerateInMemory = true;

                results = codeProvider.CompileAssemblyFromSource(options, sb.ToString());
            }
            if (results.Errors.Count > 0)
            {
                errors.AddRange(results.Errors.Cast<CompilerError>().Select(x => x.ToString()));

                return errors;
            }

            Type compiledType = results.CompiledAssembly.GetType("TestQuery");
            object instance = Activator.CreateInstance(compiledType);
            MethodInfo initMethod = compiledType.GetMethod("Initialize", new[] { typeof(ConnectionProperties) });
            if (initMethod == null)
            {
                errors.Add(string.Format("The query must contain a method called Initialize that takes one parameter of type {0}", typeof(ConnectionProperties)));
                return errors;
            }


            return errors;
        }

        private void ViewLocation(string path)
        {
            System.Diagnostics.Process p = new Process();
            p.StartInfo.FileName = path;
            p.Start();
        }

        /// <summary>
        /// populates the connection properties with the values in the form
        /// </summary>
        private List<string> Populate(ConnectionProperties props, bool doValidate = false)
        {
            List<string> errors = new List<string>();

            props.ConnectionString = this.txtConnectionString.Text.Trim();

            props.AssemblyLocations = new HashSet<string>();
            foreach (string loc in this.LoadedAssemblies.Keys)
                props.AssemblyLocations.Add(loc);

            props.CollectionTypeMappings = new Dictionary<string, HashSet<CollectionTypeMapping>>();
            foreach(var pair in this.mDatabases)
            {
                var set = new HashSet<CollectionTypeMapping>();
                foreach(CollectionTypeMapping map in pair.Value)
                {
                    set.Add(map.Clone());
                }
                props.CollectionTypeMappings[pair.Key] = set;
            }

            props.SelectedDatabase = (string)this.cbDatabases.SelectedItem;

            props.CustomSerializers = new Dictionary<string, string>();
            foreach (var pair in this.SerializerMappings)
                props.CustomSerializers.Add(pair.Key.ToString(), pair.Value.ToString());

            props.AdditionalOptions = this.mAdditionalOptions;

            if(this.mInitializationQuery != null && doValidate)
            {
                this.mInitializationQuery.Reload();
                errors.AddRange(ValidateLinqQuery(this.mInitializationQuery, props));
            }
            props.InitializationQuery = this.mInitializationQuery;

            return errors;
        }
        
        /// <summary>
        /// loads the saved connection properties into the form
        /// </summary>
        private void LoadFrom(ConnectionProperties props)
        {
            this.txtConnectionString.Text = props.ConnectionString;
            this.LoadedAssemblies = props.AssemblyLocations.ToDictionary(loc => loc, loc =>
                                                                                         {
                                                                                             if (!File.Exists(loc))
                                                                                                 return null;
                                                                                             else
                                                                                                return this.loadSafely(loc);
                                                                                         });
            this.LoadedTypes.Clear();
            this.LoadedTypes.AddRange(MongoDynamicDataContextDriver.CommonTypes.Values);
            var badAssemblies = new HashSet<Assembly>();
            this.LoadedTypes.AddRange(this.LoadedAssemblies.Values.Where(x => x != null)
                .SelectMany(ass =>
                    {
                        try
                        {
                            return ass.GetExportedTypes();
                        }
                        catch (Exception)
                        {
                            badAssemblies.Add(ass);
                            return Enumerable.Empty<Type>();
                        }
                    }));
            foreach (var badAssembly in badAssemblies)
            {
                var key = this.LoadedAssemblies.First(x => x.Value == badAssembly).Key;
                this.LoadedAssemblies[key] = null;
            }
            this.mDatabases = new Dictionary<string, HashSet<CollectionTypeMapping>>();
            foreach (var pair in props.CollectionTypeMappings)
            {
                this.mDatabases[pair.Key] = new HashSet<CollectionTypeMapping>(pair.Value);
            }

            this.cbDatabases.Items.Clear();
            this.cbDatabases.Items.AddRange(this.mDatabases.Keys.Cast<object>().ToArray());
            string db = this.mDatabases.Keys.FirstOrDefault();
            this.cbDatabases.SelectedItem = db;
            if (db != null)
                this.dgCollectionTypes.DataSource = this.mDatabases[db].Select(m => new TypeMappingWrapper(m)).ToList();
            else
                this.dgCollectionTypes.DataSource = null;
            if(props.SelectedDatabase != null)
                this.cbDatabases.SelectedIndex = this.cbDatabases.Items.IndexOf(props.SelectedDatabase);

            this.lbCustomSerializers.Items.Clear();
            this.SerializerMappings.Clear();
            if (props.CustomSerializers != null)
                foreach(var pair in props.CustomSerializers)
                {
                    var keyType = this.LoadedTypes.FirstOrDefault(x => string.Equals(pair.Key, x.FullName));
                    var valType = this.LoadedTypes.FirstOrDefault(x => string.Equals(pair.Value, x.FullName));
                    if (keyType != null && valType != null)
                        this.SerializerMappings[keyType] = valType;
                }
            this.lbCustomSerializers.Items.AddRange(this.SerializerMappings.Select(pair => new SerializerMappingWrapper { Type = pair.Key, Serializer = pair.Value }).Cast<object>().ToArray());

            this.mAdditionalOptions = props.AdditionalOptions;

            this.mInitializationQuery = props.InitializationQuery;
            if(this.mInitializationQuery != null)
            {
                this.SetLoadedQueryName(Path.GetFileName(this.mInitializationQuery.Location));
            }

            UpdateLoadedAssemblies();
        }

        /// <summary>
        /// Checks to see if the data changed from save, and if so, prompts the user.
        /// </summary>
        /// <returns>True if the form should close, False if the form should stay open.</returns>
        private bool DoCancel()
        {
            var working = new ConnectionProperties();
            this.Populate(working);

            if(!working.Equals(this.mConnection))
            {
                var result = MessageBox.Show("Changes have been made.  Do you want to save first?", "Cancel", MessageBoxButtons.YesNoCancel);
                if(result == System.Windows.Forms.DialogResult.No)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return true;
                }
                else if(result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return false;
                }
                else if(result == System.Windows.Forms.DialogResult.Yes)
                {
                    return this.DoSave();
                }

                return false;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            return true;
        }

        /// <summary>
        /// validates and saves the data to the connection
        /// </summary>
        /// <returns>true if the data successfully saved, false if the form should stay open</returns>
        private bool DoSave()
        {
            List<string> errors = this.Populate(this.mConnection, true);

            if(errors.Count == 0)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                return true;
            }
            else
            {
                DisplayErrors(errors);
                return false;
            }
        }

        private void DisplayErrors(List<string> errors)
        {
            var result = MessageBox.Show("There are errors with your settings: \r\n" +
                       String.Join("\r\n", errors),
                       "Invalid Settings",
                       MessageBoxButtons.OK);
        }

        //Updates the loaded assemblies view and the type tree view
        private void UpdateLoadedAssemblies()
        {
            this.lbLoadedAssemblies.Items.Clear();
            this.lbLoadedAssemblies.Items.AddRange(this.LoadedAssemblies.Select(x => new LoadedAssemblyWrapper(x)).Cast<object>().ToArray());

            this.tvKnownTypes.Nodes.Clear();
            this.tvKnownTypes.Nodes.AddRange(MakeTree(this.LoadedTypes).ToArray());
        }

        /// <summary>
        /// Makes a TreeView tree of nodes out of the sorted enumerable of types.  The <see cref="TreeNode.Tag"/>
        /// property contains the <see cref="Type"/> object of the represented type, or null if the node is an
        /// intermediate node that does not represent a type.
        /// </summary>
        /// <param name="typeNames">An ordered enumeration of <see cref="Type"/> objects.  Unfortunately this
        /// can't be an <see cref="IOrderedEnumerable{T}"/> because <see cref="SortedSet{T}"/> does not implement
        /// that interface.</param>
        /// <returns>An array of <see cref="TreeNode"/> objects suitable for a <see cref="TreeView"/>.</returns>
        public static TreeNode[] MakeTree(IEnumerable<Type> typeNames)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            string[] currentPath = new string[0];
            TreeNode currentNode = null;
            foreach(Type t in typeNames)
            {
                if (t == null)
                    continue;

                string[] path = t.FullName.Split('.');

                if(path.Length <= 0) continue;

                while(currentNode != null &&
                    !currentPath.IsSamePath(path) &&
                    !currentPath.IsParentOf(path))
                {
                    currentNode = currentNode.Parent;
                    if (currentNode == null)
                        currentPath = new string[0];
                    else
                        currentPath = currentNode.Text.Split('.');
                }

                int startIndex = (currentNode == null ? 0 : currentPath.Length);

                for(int i = startIndex; i < path.Length; i++)
                {
                    TreeNode temp = new TreeNode();
                    string[] tempPath = new string[i + 1];
                    Array.Copy(path, tempPath, tempPath.Length);
                    temp.Text = string.Join(".", tempPath);

                    if(currentNode == null)
                    {
                        //top of tree
                        currentNode = temp;
                        currentPath = tempPath;
                        nodes.Add(temp);
                    }
                    else
                    {
                        currentNode.Nodes.Add(temp);
                        currentNode = temp;
                        currentPath = tempPath;
                    }
                }

                currentNode.Tag = t;
                currentNode.Text = t.FullName;
            }

            return nodes.ToArray();
        }

        /// <summary>
        /// Data wrapper for the Collection type mappings to control view formatting
        /// </summary>
        public class TypeMappingWrapper : IEquatable<TypeMappingWrapper>
        {
            private CollectionTypeMapping mData;

            public TypeMappingWrapper(CollectionTypeMapping data)
            {
                this.mData = data;
            }

            public string CollectionName
            {
                get
                {
                    return mData.CollectionName;
                }
                set
                {
                    mData.CollectionName = value;
                }
            }

            
            public string CollectionType
            {
                get { return mData.CollectionType; }
                set { mData.CollectionType = value; }
                //get
                //{
                //    return mData.CollectionType;
                //}
                //set
                //{
                    
                //    var ass = mDialog.LoadedAssemblies.Keys.FirstOrDefault(x => x.GetType(value) != null);

                //    if (ass != null)
                //    {
                //        var type = ass.GetType(value);
                //        mData.CollectionType = type.AssemblyQualifiedName;
                //        mData.AssemblyName = ass.FullName;
                //    }
                //    else
                //    {
                //        var type = Type.GetType(value);
                //        if (type == null)
                //            throw new ArgumentException("Value must be a type");

                //        mData.CollectionType = type.AssemblyQualifiedName;
                //        mData.AssemblyName = null;
                //    }
                        
                //}
            }
            
            internal CollectionTypeMapping GetUnderlyingData()
            {
                return mData;
            }

            public bool Equals(TypeMappingWrapper other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.mData, mData);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (TypeMappingWrapper)) return false;
                return Equals((TypeMappingWrapper) obj);
            }

            public override int GetHashCode()
            {
                return (mData != null ? mData.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Data wrapper for the Serializer mappings to control view formatting
        /// </summary>
        public class SerializerMappingWrapper
        {
            public Type Type { get; set; }
            public Type Serializer { get; set; }

            public override string ToString()
            {
                return string.Format("'{0}' uses serializer '{1}'", Type, Serializer);
            }
        }

        /// <summary>
        /// Data wrapper for the Loaded Assemblies to control view formatting
        /// </summary>
        public class LoadedAssemblyWrapper
        {
            public String Location { get; private set; }
            public Assembly Assembly { get; set; }

            public LoadedAssemblyWrapper(KeyValuePair<string, Assembly> assemblyPair)
            {
                this.Location = assemblyPair.Key;
                this.Assembly = assemblyPair.Value;
            }

            public override string ToString()
            {
                if(this.Assembly == null)
                {
                    return string.Format("Unable to load assembly ({0})", Location);
                }
                else
                {
                    return string.Format("{0} ({1})", this.Assembly.GetName().Name, this.Location);
                }
            }
        }

        private void cmsCollectionTypes_ClearType_Click(object sender, EventArgs e)
        {
            foreach (var selectedRow in this.dgCollectionTypes.SelectedCells.Cast<DataGridViewCell>().Select(x => x.OwningRow).Distinct())
            {
                ((TypeMappingWrapper)selectedRow.DataBoundItem).CollectionType = null;
            }
        }

        private void cmsCollectionTypes_Delete_Click(object sender, EventArgs e)
        {
            if (this.cbDatabases.SelectedItem != null)
            {
                var selectedWrappers = new HashSet<TypeMappingWrapper>(
                    this.dgCollectionTypes.SelectedCells.Cast<DataGridViewCell>().Select(x => x.OwningRow.DataBoundItem).Cast<TypeMappingWrapper>()
                    );
                this.mDatabases[(string)this.cbDatabases.SelectedItem]
                    .RemoveWhere(ctm => selectedWrappers.Contains(new TypeMappingWrapper(ctm)));

                var list = this.mDatabases[(string)this.cbDatabases.SelectedItem]
                    .Select(m => new TypeMappingWrapper(m)).ToList();
                
                this.dgCollectionTypes.DataSource = list;
                
                this.dgCollectionTypes.ClearSelection();
            }
            else
            {
                MessageBox.Show("Cannot remove a row when no database is selected", "Error", MessageBoxButtons.OK);
            }
        }

        private void cmsCollectionTypes_Add_Click(object sender, EventArgs e)
        {
            if (this.cbDatabases.SelectedItem != null)
            {
                this.mDatabases[(string)this.cbDatabases.SelectedItem].Add(new CollectionTypeMapping());

                var list = this.mDatabases[(string)this.cbDatabases.SelectedItem]
                    .Select(m => new TypeMappingWrapper(m)).ToList();
                this.dgCollectionTypes.DataSource = list;
                int lastRow = this.dgCollectionTypes.Rows.GetLastRow(DataGridViewElementStates.Visible);
                
                this.dgCollectionTypes.ClearSelection();
                this.dgCollectionTypes.Rows[lastRow].Cells.OfType<DataGridViewCell>().First().Selected = true;
            }
            else
            {
                MessageBox.Show("Cannot add a row when no database is selected", "Error", MessageBoxButtons.OK);
            }
        }

        private void dgCollectionTypes_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = this.dgCollectionTypes.HitTest(e.X, e.Y);
                this.dgCollectionTypes.ClearSelection();
                this.dgCollectionTypes.Rows[hti.RowIndex].Selected = true;
            }
        }

        
    }
    


    static class Extensions
    {
        public static T AddRange<T,U>(this T set, IEnumerable<U> values) where T : ISet<U>
        {
            set.UnionWith(values);
            return set;
        }

        public static string NamespaceQualifiedName(this Type t)
        {
            return t.Namespace + "." + t.Name;
        }

        public static bool IsSamePath(this string[] path1, string[] path2)
        {
            if (path1.Length != path2.Length)
                return false;

            return path2.AsEnumerable().SequenceEqual(path1);
        }

        public static bool IsParentOf(this string[] me, string[] other)
        {
            //null cannot be parent or child
            if (me == null || other == null)
                return false;

            //child must be long enough to have content after the dot, i.e. 'parent.child'
            if (me.Length >= other.Length)
                return false;

            //the first 'me.length' topics must be equal
            return !me.Where((t, i) => !string.Equals(t, other[i], StringComparison.OrdinalIgnoreCase)).Any();
        }
    }
}
