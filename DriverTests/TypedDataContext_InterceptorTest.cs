using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GDSX.Externals.LinqPad.Driver;
using LINQPad.Extensibility.DataContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.IO;
using MongoDB.Driver.Builders;
using Moq;

namespace DriverTests
{
    
    
    /// <summary>
    ///This is a test class for TypedDataContext_InterceptorTest and is intended
    ///to contain all TypedDataContext_InterceptorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TypedDataContext_InterceptorTest
    {

        private static Assembly GeneratedAssembly;
        private static ConnectionProperties mConnectionProperties = new ConnectionProperties
        {
            CollectionTypeMappings = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<CollectionTypeMapping>>
                {
                    {"db", new HashSet<CollectionTypeMapping>()
                                {
                                    new CollectionTypeMapping
                                        {
                                            CollectionName = "collection",
                                            CollectionType = typeof(string).ToString()
                                        },
                                    new CollectionTypeMapping
                                        {
                                            CollectionName = "untyped"
                                        }
                                }}                              
                },
            ConnectionString = "ConnectionString",
            SelectedDatabase = "db",
            AssemblyLocations = new HashSet<string>()
        };

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get;set; }

        [ClassInitialize]
        public static void FixtureSetup(TestContext ctx)
        {
            var driver = new MongoDynamicDataContextDriver();
            var code = new[] { driver.GenerateDynamicCode(mConnectionProperties, new List<Assembly>(), "testns", "driver") }
                .Concat(driver.GetStaticCodeFiles());

            var assName = new AssemblyName
                {
                    CodeBase = Path.GetTempFileName(),
                };
            driver.BuildAssembly(mConnectionProperties, code, assName, () => "");
            GeneratedAssembly = Assembly.LoadFrom(assName.CodeBase);

            Console.WriteLine("Code:");
            foreach(string c in code)
            {
                Console.WriteLine(c);
                Console.WriteLine("------------------");
            }
        }


        [ClassCleanup]
        public static void FixtureTearDown()
        {
            if(File.Exists(GeneratedAssembly.CodeBase))
                File.Delete(GeneratedAssembly.CodeBase);
        }


        private PrivateObject MakeInterceptor<T>(MongoCollection<T> collection, TextWriter writer)
        {
            var generic = GeneratedAssembly.GetType("GDSX.Internals.LinqPad.Driver.Interceptor");

            return new PrivateObject(generic.MakeGenericType(new[] { typeof(T) }), new object[] { collection, writer });
        }

        private PrivateObject MakeTypedContext(MongoServer server)
        {
            return new PrivateObject(GeneratedAssembly.GetType("testns.driver"), new object[]{server });
        }

        [TestMethod]
        public void Test_Compiles()
        {
            
        }

        private class TestSerializable
        {
            [BsonId]
            public string TestId { get; set; }
        }

        [TestMethod]
        public void Interceptor_GetId_GetsId()
        {
            var getId = GeneratedAssembly.GetType("GDSX.Externals.LinqPad.Driver.Interceptor`1", true).MakeGenericType(typeof(string))
                .GetMethod("GetId", BindingFlags.Public | BindingFlags.Static);
            object ret = getId.Invoke(null, new object[]{new TestSerializable { TestId = "abcdef" }});
            Assert.AreEqual("abcdef", ret.ToString());
        }

        //[TestMethod]
        //public void Interceptor_DeepHash_Hashes()
        //{
        //    //var deepHash = GeneratedAssembly.GetType("GDSX.Externals.LinqPad.Driver.Interceptor`1", true).MakeGenericType(typeof(TestSerializable))
        //    //    .GetMethod("DeepHash", BindingFlags.Public | BindingFlags.Static);
        //    //deepHash = deepHash.MakeGenericMethod(typeof(TestSerializable));

        //    //int hash = (int)deepHash.Invoke(null, new object[] { new TestSerializable { TestId = "abcdef" } });
        //    //int hash2 = (int)deepHash.Invoke(null, new object[] { new TestSerializable { TestId = "abcdef" } });

        //    //Assert.AreEqual(hash, hash2);

        //}

        [TestMethod]
        public void Interceptor_HasChanged_FalseWhenNoChanges()
        {
            var hasChanged = GeneratedAssembly.GetType("GDSX.Externals.LinqPad.Driver.Interceptor`1", true).MakeGenericType(typeof(TestSerializable))
                .GetMethod("HasChanged", BindingFlags.Public | BindingFlags.Static);
            hasChanged = hasChanged.MakeGenericMethod(typeof(TestSerializable));

            //var deepHash = GeneratedAssembly.GetType("GDSX.Externals.LinqPad.Driver.Interceptor`1", true).MakeGenericType(typeof(TestSerializable))
            //    .GetMethod("DeepHash", BindingFlags.Public | BindingFlags.Static);
            //deepHash = deepHash.MakeGenericMethod(typeof(TestSerializable));

            var toTest = new TestSerializable { TestId = "abcdef" };
            //int hash = (int)deepHash.Invoke(null, new object[] { toTest });

            var originalDoc = toTest.ToBsonDocument();

            bool changed = (bool)hasChanged.Invoke(null, new object[] { toTest, originalDoc });

            Assert.IsFalse(changed);
        }

        [TestMethod]
        public void Interceptor_HasChanged_TrueWhenChangesMade()
        {
            var hasChanged = GeneratedAssembly.GetType("GDSX.Externals.LinqPad.Driver.Interceptor`1", true).MakeGenericType(typeof(TestSerializable))
                .GetMethod("HasChanged", BindingFlags.Public | BindingFlags.Static);
            hasChanged = hasChanged.MakeGenericMethod(typeof(TestSerializable));

            //var deepHash = GeneratedAssembly.GetType("GDSX.Externals.LinqPad.Driver.Interceptor`1", true).MakeGenericType(typeof(TestSerializable))
            //    .GetMethod("DeepHash", BindingFlags.Public | BindingFlags.Static);
            //deepHash = deepHash.MakeGenericMethod(typeof(TestSerializable));

            var toTest = new TestSerializable { TestId = "abcdef" };
            //int hash = (int)deepHash.Invoke(null, new object[] { toTest });
            var origDocument = toTest.ToBsonDocument();

            toTest.TestId = "qrx";

            bool changed = (bool)hasChanged.Invoke(null, new object[] { toTest, origDocument });

            Assert.IsTrue(changed);
        }

        //[TestMethod]
        //public void Interceptor_TrackChanges_RecordsSeenObjects()
        //{
        //    var serverSettings = new MongoServerSettings { Server = new MongoServerAddress("localhost") };
        //    var server = new Mock<MongoServer>(MockBehavior.Strict, serverSettings);
        //    server.SetupGet(x => x.Settings).Returns(serverSettings);
        //    var dbSettings = new MongoDatabaseSettings(server.Object, "testDb");
        //    var db = new Mock<MongoDatabase>(MockBehavior.Strict, server.Object, dbSettings);
        //    db.SetupGet(x => x.Settings).Returns(dbSettings);
        //    db.Setup(x => x.GetCollection(It.IsAny<MongoCollectionSettings<BsonDocument>>())).Returns((MongoCollection<BsonDocument>)null);
        //    var collection = new Mock<MongoCollection<TestSerializable>>(MockBehavior.Strict, db.Object, new MongoCollectionSettings<TestSerializable>(db.Object, "testCollection"));
        //    StringBuilder sb = new StringBuilder();
        //    using(var writer = new StringWriter(sb)){

        //        var instance = MakeInterceptor(collection.Object, writer);

        //        instance.SetProperty("TrackChanges", true, null);

        //        var cursor = instance.Invoke("FindAs", typeof(TestSerializable), Query.EQ("_id", "abcdef"));
        //    }

        //}

        

        
    }
}
