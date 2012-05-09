using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LINQPad.Extensibility.DataContext;

namespace GDSX.Externals.LinqPad.Driver
{
    /// <summary>
    /// An object representation of all the properties that must be saved for this driver.
    /// Provides serialization to and from <see cref="XElement"/> nodes.
    /// </summary>
    public class ConnectionProperties
    {
        /// <summary>ConnectionString</summary>
        public string ConnectionString { get; set; }

        public HashSet<String> AssemblyLocations { get; set; }

        public Dictionary<string, HashSet<CollectionTypeMapping>> CollectionTypeMappings { get; set; }

        public string SelectedDatabase { get; set; }

        public Dictionary<string, string> CustomSerializers { get; set; }

        public bool Equals(ConnectionProperties other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!Equals(other.ConnectionString, this.ConnectionString))
                return false;

            if (this.AssemblyLocations == null)
            {
                if (other.AssemblyLocations != null)
                    return false;
            }
            else
            {
                if (other.AssemblyLocations == null)
                    return false;

                if (this.AssemblyLocations.Count != other.AssemblyLocations.Count)
                    return false;

                if (!this.AssemblyLocations.All(s => other.AssemblyLocations.Contains(s)))
                    return false;
                
            }

            if(this.CollectionTypeMappings == null)
            {
                if (other.CollectionTypeMappings != null)
                    return false;
            }
            else
            {
                if (other.CollectionTypeMappings == null)
                    return false;

                foreach (var pair in this.CollectionTypeMappings)
                {
                    if (!other.CollectionTypeMappings.ContainsKey(pair.Key))
                        return false;

                    if (pair.Value.Count != other.CollectionTypeMappings[pair.Key].Count)
                        return false;

                    if (!pair.Value.All(s => other.CollectionTypeMappings[pair.Key].Contains(s)))
                        return false;
                }
            }

            if (this.CustomSerializers == null)
            {
                if (other.CustomSerializers != null)
                    return false;
            }
            else
            {
                if (other.CustomSerializers == null)
                    return false;

                if (this.CustomSerializers.Count != other.CustomSerializers.Count)
                    return false;

                foreach(var pair in this.CustomSerializers)
                {
                    if (!other.CustomSerializers.ContainsKey(pair.Key))
                        return false;

                    if (!string.Equals(pair.Value, other.CustomSerializers[pair.Key]))
                        return false;
                }
            }


            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ConnectionProperties)) return false;
            return Equals((ConnectionProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                if(CollectionTypeMappings != null)
                    result = CollectionTypeMappings.Aggregate(397, (hash, pair) =>
                                                              {
                                                                  hash = (hash * 397) ^ pair.Key.GetHashCode();
                                                                  hash = pair.Value.Aggregate(hash,
                                                                      (h2, ctm) =>
                                                                      {
                                                                          h2 = (h2 * 397) ^ ctm.CollectionName.GetHashCode();
                                                                          h2 = (h2 * 397) ^ ctm.CollectionType.GetHashCode();
                                                                          return h2;
                                                                      });
                                                                  return hash;
                                                              });
                if (AssemblyLocations != null)
                    result = (result * 397) ^ AssemblyLocations.Aggregate(result, (hash, str) => (hash * 397) ^ str.GetHashCode());

                if(CustomSerializers != null)
                    result = (result * 397) ^ CustomSerializers.Aggregate(result, (hash, pair) =>
                                                                                      {
                                                                                          hash = (hash * 397) ^ pair.Key.GetHashCode();
                                                                                          hash = (hash * 397) ^ pair.Value.GetHashCode();
                                                                                          return hash;
                                                                                      });

                return (result*397) ^ (ConnectionString != null ? ConnectionString.GetHashCode() : 0);
            }
        }

        public void Serialize(XElement root)
        {
            root.RemoveAll();

            root.Add(new XElement("ConnectionString", ConnectionString));

            if (this.AssemblyLocations != null)
            {
                var assemblyLocations = new XElement("AssemblyLocations");
                foreach (string s in this.AssemblyLocations)
                {
                    assemblyLocations.Add(new XElement("Location", s));
                }
                root.Add(assemblyLocations);
            }

            if (this.CollectionTypeMappings != null)
            {
                var typeMappings = new XElement("CollectionTypeMappings");
                foreach (var pair in this.CollectionTypeMappings)
                {
                    var db = new XElement("Database");
                    db.SetAttributeValue("name", pair.Key);
                    foreach (var map in pair.Value)
                    {
                        db.Add(map.Serialize());
                    }
                    typeMappings.Add(db);
                }
                root.Add(typeMappings);
            }

            var selectedDb = new XElement("SelectedDb", this.SelectedDatabase);
            root.Add(selectedDb);

            if (this.CustomSerializers != null)
            {
                var customSerializers = new XElement("CustomSerializers");
                foreach (var pair in this.CustomSerializers)
                {
                    var serializer = new XElement("Serializer");
                    serializer.SetAttributeValue("type", pair.Key);
                    serializer.SetAttributeValue("serializer", pair.Value);
                    customSerializers.Add(serializer);
                }
                root.Add(customSerializers);
            }
        }

        public void Deserialize(XElement root)
        {
            var xElement = root.Element("ConnectionString");
            if (xElement != null)
                this.ConnectionString = xElement.Value;

            xElement = root.Element("AssemblyLocations");
            if (xElement != null)
            {
                this.AssemblyLocations = new HashSet<string>();
                foreach(var el in xElement.Descendants("Location"))
                {
                    this.AssemblyLocations.Add(el.Value);
                }
            }

            xElement = root.Element("CollectionTypeMappings");
            if(xElement != null)
            {
                this.CollectionTypeMappings = new Dictionary<string, HashSet<CollectionTypeMapping>>();
                foreach(var db in xElement.Descendants("Database"))
                {
                    var set = new HashSet<CollectionTypeMapping>();
                    foreach(var coll in db.Descendants("CollectionTypeMapping"))
                    {
                        var newMap = new CollectionTypeMapping();
                        newMap.Deserialize(coll);
                        set.Add(newMap);
                    }
                    this.CollectionTypeMappings.Add(db.Attribute("name").Value, set);
                }
            }

            xElement = root.Element("SelectedDb");
            if(xElement != null)
                this.SelectedDatabase = xElement.Value;

            xElement = root.Element("CustomSerializers");
            if(xElement != null)
            {
                this.CustomSerializers = new Dictionary<string,string>();
                foreach(var serializer in xElement.Descendants("Serializer"))
                {
                    var key = serializer.Attribute("type").Value;
                    var value = serializer.Attribute("serializer").Value;

                    this.CustomSerializers[key] = value;
                }
            }
                
        }
    }

    /// <summary>
    /// A mapping from a Collection name to the deserialized Type we enforce
    /// on the collection
    /// </summary>
    public class CollectionTypeMapping
    {
        public string CollectionName { get; set; }

        public String CollectionType { get; set; }

        public bool Equals(CollectionTypeMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.CollectionName, CollectionName) &&
                Equals(other.CollectionType, this.CollectionType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CollectionTypeMapping)) return false;
            return Equals((CollectionTypeMapping)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CollectionName != null ? CollectionName.GetHashCode() : 0) * 397) ^
                    (this.CollectionType != null ? this.CollectionType.GetHashCode() : 0);
            }
        }

        public XElement Serialize()
        {
            var ret = new XElement("CollectionTypeMapping");
            ret.SetAttributeValue("CollectionName", this.CollectionName);
            ret.SetAttributeValue("CollectionType", this.CollectionType);
            //ret.SetAttributeValue("Assembly", this.AssemblyName);

            return ret;
        }

        public void Deserialize(XElement element)
        {
            if (!element.Name.Equals((XName)"CollectionTypeMapping"))
                throw new ArgumentException("Element must have name CollectionTypeMapping");

            var attr = element.Attribute("CollectionName");
            if (attr != null)
                this.CollectionName = attr.Value;

            attr = element.Attribute("CollectionType");
            if (attr != null)
                this.CollectionType = attr.Value;

            //attr = element.Attribute("Assembly");
            //if (attr != null)
            //    this.AssemblyName = attr.Value;
        }

        public CollectionTypeMapping Clone()
        {
            return new CollectionTypeMapping
            {
                CollectionName = this.CollectionName,
                CollectionType = this.CollectionType
            };
        }
    }
    
}
