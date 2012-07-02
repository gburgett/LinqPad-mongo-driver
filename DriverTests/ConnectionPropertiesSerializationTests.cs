using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GDSX.Externals.LinqPad.Driver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriverTests
{
    [TestClass]
    public class ConnectionPropertiesSerializationTests
    {
        
        [TestMethod]
        public void ConnectionString_RoundTrip_Equal()
        {
            var prop = new ConnectionProperties
                           {
                               ConnectionString = "ABC"
                           };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void AssemblyLocation_RoundTrip_Equal()
        {
            var prop = new ConnectionProperties
            {
                AssemblyLocations =
                    {
                        "abc"
                    }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void CustomSerializers_RoundTrip_Equal()
        {
            var prop = new ConnectionProperties
            {
                CustomSerializers =
                    {
                        {"abc", "Def"}
                    }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void SelectedDB_RoundTrip_Equal()
        {
            var prop = new ConnectionProperties
            {
                SelectedDatabase = "abcdef"
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void CollectionTypeMappings_EmptyDB_RoundTripEqual()
        {
            var prop = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"DB", new HashSet<CollectionTypeMapping>()}
                    }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void CollectionTypeMappings_OneCollection_RoundTripEqual()
        {
            var prop = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"DB", new HashSet<CollectionTypeMapping>
                                   {
                                       new CollectionTypeMapping
                                           {
                                               CollectionName = "coll",
                                               CollectionType = "string"
                                           }
                                   }}
                    }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void CollectionTypeMappings_TwoDBs_RoundTripEqual()
        {
            var prop = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"DB", new HashSet<CollectionTypeMapping>
                                   {
                                       new CollectionTypeMapping
                                           {
                                               CollectionName = "coll",
                                               CollectionType = "string"
                                           }
                                   }},
                        {"DB2", new HashSet<CollectionTypeMapping>
                                   {
                                       new CollectionTypeMapping
                                           {
                                               CollectionName = "coll2",
                                               CollectionType = "INT"
                                           }
                                   }}

                    }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void AdditionalOptions_BooleanValues_RoundTripEqual()
        {
            var prop = new ConnectionProperties
            {
                AdditionalOptions = new ConnectionAdditionalOptions
                                        {
                                            AllowSaveForAllTypes = true,
                                            BlanketIgnoreExtraElements = true
                                        }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }

        [TestMethod]
        public void AdditionalOptions_ExplicitAllowSave_RoundTripEqual()
        {
            var prop = new ConnectionProperties
            {
                AdditionalOptions = new ConnectionAdditionalOptions
                {
                    ExplicitSaveAllowedTypes =
                        {
                            "String"
                        }
                }
            };

            //
            //Act
            //do the round trip
            var serializer = new ConnectionPropertiesSerializer();
            XElement element = new XElement("test");
            serializer.Serialize(element, prop);

            ConnectionProperties prop2 = serializer.Deserialize(element);

            //Assert
            Assert.AreEqual(prop, prop2);
        }
    }
}
