using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GDSX.Externals.LinqPad.Driver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriverTests
{
    [TestClass]
    public class ConnectionPropertiesEqualityTests
    {
        [TestMethod]
        public void ConnectionStringsDiffer_NotEqual()
        {
            var props1 = new ConnectionProperties
                    {
                        ConnectionString = "abcdef"
                    };
            var props2 = new ConnectionProperties
                    {
                        ConnectionString = "ghijkl"
                    };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void AssemblyLocationsDiffer_OneIsNull_NotEqual()
        {
            var props1 = new ConnectionProperties
                    {
                        AssemblyLocations =
                            {
                                "abc"
                            }
                    };
            var props2 = new ConnectionProperties
                    {
                    };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void AssemblyLocationsDiffer_Count_NotEqual()
        {
            var props1 = new ConnectionProperties
            {
                AssemblyLocations =
                            {
                                "abc",
                                "DEF"
                            }
            };
            var props2 = new ConnectionProperties
            {
                AssemblyLocations =
                    {
                        "abc"
                    }
            };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void AssemblyLocationsDiffer_Values_NotEqual()
        {
            var props1 = new ConnectionProperties
            {
                AssemblyLocations =
                            {
                                "abc",
                                "DEF"
                            }
            };
            var props2 = new ConnectionProperties
            {
                AssemblyLocations =
                    {
                        "abc",
                        "GHI"
                    }
            };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void AssemblyLocationsSameValues_Equal()
        {
            var props1 = new ConnectionProperties
            {
                AssemblyLocations =
                            {
                                "abc",
                                "DEF"
                            }
            };
            var props2 = new ConnectionProperties
            {
                AssemblyLocations =
                    {
                        "abc",
                        "DEF"
                    }
            };

            Assert.IsTrue(object.Equals(props1, props2));
        }


        [TestMethod]
        public void CollectionTypeMappings_OneIsNull_NotEqual()
        {
            var props1 = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"dev", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "String"
                                            }
                                    }}
                    }
            };
            var props2 = new ConnectionProperties
            {
            };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void CollectionTypeMappings_DifferentDatabases_NotEqual()
        {
            var props1 = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"dev", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "String"
                                            }
                                    }}
                    }
            };
            var props2 = new ConnectionProperties
            {
                CollectionTypeMappings = 
                    {
                        {"dev2", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "String"
                                            }
                                    }}
                    }
            };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void CollectionTypeMappings_MappingsDiffer_NotEqual()
        {
            var props1 = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"dev", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "String"
                                            }
                                    }}
                    }
            };
            var props2 = new ConnectionProperties
            {
                CollectionTypeMappings = 
                    {
                        {"dev", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "Int"
                                            }
                                    }}
                    }
            };

            Assert.IsFalse(object.Equals(props1, props2));
        }

        [TestMethod]
        public void CollectionTypeMappings_MappingsEqual_Equal()
        {
            var props1 = new ConnectionProperties
            {
                CollectionTypeMappings =
                    {
                        {"dev", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "String"
                                            }
                                    }}
                    }
            };
            var props2 = new ConnectionProperties
            {
                CollectionTypeMappings = 
                    {
                        {"dev", new HashSet<CollectionTypeMapping>
                                    {
                                        new CollectionTypeMapping
                                            {
                                                CollectionName = "abc",
                                                CollectionType = "String"
                                            }
                                    }}
                    }
            };

            Assert.IsTrue(object.Equals(props1, props2));
        }
    }
}
