using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LINQPad.Extensibility.DataContext;
using MongoDB.Bson.Serialization.Conventions;

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

        public ConnectionAdditionalOptions AdditionalOptions { get; set; }
        
        public ConnectionProperties()
        {
            AssemblyLocations = new HashSet<string>();
            CollectionTypeMappings = new Dictionary<string, HashSet<CollectionTypeMapping>>();
            CustomSerializers = new Dictionary<string, string>();
            AdditionalOptions = new ConnectionAdditionalOptions();
        }

        #region equality
        #region EqualityComparers
        protected static readonly CollectionEqualityComparer<string> AssemblyLocationsEqualityComparer = new CollectionEqualityComparer<string>();
        protected static readonly DictionaryEqualityComparer<string, HashSet<CollectionTypeMapping>> CollectionTypeMappingEqualityComparer = 
            new DictionaryEqualityComparer<string, HashSet<CollectionTypeMapping>>
                {
                    ValueEqualityComparer = new CollectionEqualityComparer<CollectionTypeMapping>()
                };

        protected static readonly DictionaryEqualityComparer<string, string> CustomSerializersEqualityComparer = new DictionaryEqualityComparer<string, string>();
        protected static readonly StringEqualityComparer stringComparer = new StringEqualityComparer();
        #endregion

        public bool Equals(ConnectionProperties other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!stringComparer.Equals(other.ConnectionString, this.ConnectionString))
                return false;

            if (!AssemblyLocationsEqualityComparer.Equals(this.AssemblyLocations, other.AssemblyLocations))
                return false;

            if (!CollectionTypeMappingEqualityComparer.Equals(this.CollectionTypeMappings, other.CollectionTypeMappings))
                return false;

            if (!CustomSerializersEqualityComparer.Equals(this.CustomSerializers, other.CustomSerializers))
                return false;


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
                int result = AssemblyLocationsEqualityComparer.GetHashCode(this.AssemblyLocations);

                result = (result * 397) ^ CustomSerializersEqualityComparer.GetHashCode(this.CustomSerializers);

                result = (result * 397) ^ CollectionTypeMappingEqualityComparer.GetHashCode(this.CollectionTypeMappings);
                
                return (result*397) ^ (ConnectionString != null ? ConnectionString.GetHashCode() : 0);
            }
        }

        #endregion

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

        public CollectionTypeMapping Clone()
        {
            return new CollectionTypeMapping
            {
                CollectionName = this.CollectionName,
                CollectionType = this.CollectionType
            };
        }
    }
    

    public class ConnectionAdditionalOptions
    {
        /// <summary>
        /// Gets or sets whether or not the <see cref="AlwaysIgnoreExtraElementsConvention"/>
        /// should be used for all types
        /// </summary>
        public bool BlanketIgnoreExtraElements { get; set; }

        /// <summary>
        /// Gets or sets whether the Interceptor should allow 'Save' to work normally,
        /// or, if false, throw an exception whenever 'Save' is called for a type that
        /// is not one of the explicitly allowed types.
        /// </summary>
        public bool AllowSaveForAllTypes { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="Type"/> objects representing the
        /// types that are explicitly allowed to be saved to Mongo using the 'Save'
        /// method of the Interceptor collections.
        /// </summary>
        public List<String> ExplicitSaveAllowedTypes { get; set; }


        public ConnectionAdditionalOptions()
        {
            //defaults
            BlanketIgnoreExtraElements = false;
            AllowSaveForAllTypes = false;
            ExplicitSaveAllowedTypes = new List<String>();
        }


        #region Equality

        protected static readonly CollectionEqualityComparer<String> ExplicitSaveEqualityComparer = new CollectionEqualityComparer<String>();

        public bool Equals(ConnectionAdditionalOptions other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if(!Equals(BlanketIgnoreExtraElements, other.BlanketIgnoreExtraElements))
                return false;
            if(!Equals(AllowSaveForAllTypes, other.AllowSaveForAllTypes))
                return false;

            if (!ExplicitSaveEqualityComparer.Equals(this.ExplicitSaveAllowedTypes, other.ExplicitSaveAllowedTypes))
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ConnectionAdditionalOptions)) return false;
            return Equals((ConnectionAdditionalOptions) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = BlanketIgnoreExtraElements.GetHashCode();
                result = (result*397) ^ AllowSaveForAllTypes.GetHashCode();
                result = (result * 397) ^ ExplicitSaveEqualityComparer.GetHashCode(this.ExplicitSaveAllowedTypes);
                return result;
            }
        }

        #endregion

        public ConnectionAdditionalOptions Clone()
        {
            var ret = new ConnectionAdditionalOptions();
            ret.AllowSaveForAllTypes = this.AllowSaveForAllTypes;
            ret.BlanketIgnoreExtraElements = this.BlanketIgnoreExtraElements;
            ret.ExplicitSaveAllowedTypes.AddRange(this.ExplicitSaveAllowedTypes);

            return ret;
        }
    }

    #region Equality
    public class CollectionEqualityComparer<T> : IEqualityComparer<ICollection<T>>
    {
        public bool Equals(ICollection<T> x, ICollection<T> y)
        {
            //if one is null the other should be null or empty
            if(x == null)
            {
                return y == null || y.Count == 0;
            }
            if (y == null)
                return x.Count == 0;

            //quick check - counts
            if (x.Count != y.Count)
                return false;

            //deep check - y contains every element of x
            return x.All(y.Contains);
        }

        public int GetHashCode(ICollection<T> obj)
        {
            if (obj == null)
                return 0;

            return obj.Aggregate(397, (hash, o) => (hash * 397) ^ o.GetHashCode());
        }
    }

    public class DictionaryEqualityComparer<T, U> : IEqualityComparer<IDictionary<T, U>>
    {
        /// <summary>
        /// Gets or sets the EqualityComparer used to compare the values of the dictionary
        /// </summary>
        public IEqualityComparer<U> ValueEqualityComparer { get; set; }

        public bool Equals(IDictionary<T, U> x, IDictionary<T, U> y)
        {
            if (x == null)
            {
                return y == null || y.Count == 0;
            }
            if (y == null)
                return x.Count == 0;

            if (y.Count != x.Count)
                return false;

            foreach (KeyValuePair<T, U> pair in x)
            {

                U yValue;
                
                //TODO: add key equality comparer if we ever need it
                if (!y.ContainsKey(pair.Key))
                    return false;
                yValue = y[pair.Key];
                

                if (ValueEqualityComparer == null)
                {
                    if (!object.Equals(pair.Value, yValue))
                        return false;
                }
                else
                {
                    if (!ValueEqualityComparer.Equals(pair.Value, yValue))
                        return false;
                }
            }

            return true;
        }

        public int GetHashCode(IDictionary<T, U> obj)
        {
            if (obj == null)
                return 0;

            return obj.Aggregate(397, (hash, pair) =>
                {
                    hash = (hash * 397) ^ pair.Key.GetHashCode();
                    hash = (hash * 397) ^ (ValueEqualityComparer == null ? 
                        pair.Value.GetHashCode() :
                        ValueEqualityComparer.GetHashCode(pair.Value));

                    return hash;
                });
        }
    }

    public class StringEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (string.IsNullOrEmpty(x))
                return string.IsNullOrEmpty(y);

            if (string.IsNullOrEmpty(y))
                return false;

            return string.Equals(x, y);
        }

        public int GetHashCode(string obj)
        {
            if (string.IsNullOrEmpty(obj))
                return 0;

            return obj.GetHashCode();
        }
    }

    #endregion

    #region Serialization

    public interface IXElementSerializer<T>
    {
        /// <summary>
        /// Serializes the given object into the base element specified by the <paramref name="root"/>.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="obj"></param>
        void Serialize(XElement root, T obj);

        /// <summary>
        /// Deserializes an object of type <typeparam name="T"/> out of the given XElement.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        T Deserialize(XElement root);
    }

    public class XElementSerializer<T> : IXElementSerializer<T>
    {
        protected readonly System.Xml.Serialization.XmlSerializer Serializer = new XmlSerializer(typeof(T), "");

        public virtual void Serialize(XElement root, T obj)
        {
            var doc = new XDocument();
            using (XmlWriter writer = doc.CreateWriter())
            {
                Serializer.Serialize(writer, obj);
            }
            root.Add(doc.Root);
        }

        public virtual T Deserialize(XElement root)
        {
            var enumerator = root.Elements().GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Expected at least one element in the XML element " + root.ToString());

            XElement element = enumerator.Current;
            if (enumerator.MoveNext())
                throw new InvalidOperationException("Expected only one element in the XML element " + root.ToString());

            using (XmlReader reader = element.CreateReader())
            {
                return (T)Serializer.Deserialize(reader);
            }
        }
    }

    public class ConnectionPropertiesSerializer : IXElementSerializer<ConnectionProperties>
    {
        private readonly IXElementSerializer<CollectionTypeMapping> ctmSerializer = new CollectionTypeMappingSerializer();
        private readonly IXElementSerializer<ConnectionAdditionalOptions> additionalOptionsSerializer = new XElementSerializer<ConnectionAdditionalOptions>(); 


        public void Serialize(XElement root, ConnectionProperties obj)
        {
            root.RemoveAll();

            root.Add(new XElement("ConnectionString", obj.ConnectionString));

            //assembly locations
            if (obj.AssemblyLocations != null)
            {
                var assemblyLocations = new XElement("AssemblyLocations");
                foreach (string s in obj.AssemblyLocations)
                {
                    assemblyLocations.Add(new XElement("Location", s));
                }
                root.Add(assemblyLocations);
            }

            //collection type mappings
            if (obj.CollectionTypeMappings != null)
            {
                var typeMappings = new XElement("CollectionTypeMappings");
                foreach (var pair in obj.CollectionTypeMappings)
                {
                    var db = new XElement("Database");
                    db.SetAttributeValue("name", pair.Key);
                    foreach (var map in pair.Value)
                    {
                        XElement mapElement = new XElement("CollectionTypeMapping");
                        ctmSerializer.Serialize(mapElement, map);
                        db.Add(mapElement);
                    }
                    typeMappings.Add(db);
                }
                root.Add(typeMappings);
            }

            //selected DB
            var selectedDb = new XElement("SelectedDb", obj.SelectedDatabase);
            root.Add(selectedDb);

            //custom serializers
            if (obj.CustomSerializers != null)
            {
                var customSerializers = new XElement("CustomSerializers");
                foreach (var pair in obj.CustomSerializers)
                {
                    var serializer = new XElement("Serializer");
                    serializer.SetAttributeValue("type", pair.Key);
                    serializer.SetAttributeValue("serializer", pair.Value);
                    customSerializers.Add(serializer);
                }
                root.Add(customSerializers);
            }

            //additional options
            if (obj.AdditionalOptions != null)
            {
                var additionalOptions = new XElement("AdditionalOptions");
                this.additionalOptionsSerializer.Serialize(additionalOptions, obj.AdditionalOptions);
                root.Add(additionalOptions);
            }
        }

        public ConnectionProperties Deserialize(XElement root)
        {
            ConnectionProperties obj = new ConnectionProperties();

            var xElement = root.Element("ConnectionString");
            if (xElement != null)
                obj.ConnectionString = xElement.Value;

            //assembly locations
            xElement = root.Element("AssemblyLocations");
            if (xElement != null)
            {
                obj.AssemblyLocations = new HashSet<string>();
                foreach (var el in xElement.Descendants("Location"))
                {
                    obj.AssemblyLocations.Add(el.Value);
                }
            }

            //collection type mappings
            xElement = root.Element("CollectionTypeMappings");
            if (xElement != null)
            {
                obj.CollectionTypeMappings = new Dictionary<string, HashSet<CollectionTypeMapping>>();
                foreach (var db in xElement.Descendants("Database").Where(x => x.Attribute("name") != null))
                {
                    var set = new HashSet<CollectionTypeMapping>();
                    foreach (var coll in db.Descendants("CollectionTypeMapping"))
                    {
                        CollectionTypeMapping newMap = ctmSerializer.Deserialize(coll);
                        set.Add(newMap);
                    }
                    obj.CollectionTypeMappings.Add(db.Attribute("name").Value, set);
                }
            }

            //selected DB
            xElement = root.Element("SelectedDb");
            if (xElement != null)
                obj.SelectedDatabase = xElement.Value;

            //custom serializers
            xElement = root.Element("CustomSerializers");
            if (xElement != null)
            {
                obj.CustomSerializers = new Dictionary<string, string>();
                foreach (var serializer in xElement.Descendants("Serializer"))
                {
                    var key = serializer.Attribute("type").Value;
                    var value = serializer.Attribute("serializer").Value;

                    obj.CustomSerializers[key] = value;
                }
            }

            //additional options
            xElement = root.Element("AdditionalOptions");
            if(xElement != null)
            {
                obj.AdditionalOptions = this.additionalOptionsSerializer.Deserialize(xElement);
            }

            return obj;
        }

        
        
    }

    public class CollectionTypeMappingSerializer: IXElementSerializer<CollectionTypeMapping>
    {
        public void Serialize(XElement root, CollectionTypeMapping obj)
        {
            if (!root.Name.Equals((XName)"CollectionTypeMapping"))
                throw new ArgumentException("Element must have name CollectionTypeMapping");

            root.SetAttributeValue("CollectionName", obj.CollectionName);
            root.SetAttributeValue("CollectionType", obj.CollectionType);
        }

        public CollectionTypeMapping Deserialize(XElement root)
        {
            CollectionTypeMapping obj = new CollectionTypeMapping();

            if (!root.Name.Equals((XName)"CollectionTypeMapping"))
                throw new ArgumentException("Element must have name CollectionTypeMapping");

            var attr = root.Attribute("CollectionName");
            if (attr != null)
                obj.CollectionName = attr.Value;

            attr = root.Attribute("CollectionType");
            if (attr != null)
                obj.CollectionType = attr.Value;

            return obj;
        }
    }

    //public class ConnectionAdditionalOptionsSerializer : IXElementSerializer<ConnectionAdditionalOptions>
    //{
    //    public void Serialize(XElement root, ConnectionAdditionalOptions obj)
    //    {
    //        if (!root.Name.Equals((XName)"AdditionalOptions"))
    //            throw new ArgumentException("Element must have name AdditionalOptions");

    //        root.SetElementValue("AllowSaveForAllTypes", obj.AllowSaveForAllTypes);
    //        root.SetElementValue("BlanketIgnoreExtraElements", obj.BlanketIgnoreExtraElements);
    //        XElement element = new XElement("ExplicitSaveAllowedTypes");
    //        foreach (Type t in obj.ExplicitSaveAllowedTypes)
    //        {
    //            element.Add(new XElement("type", t.ToString()));
    //        }
    //        root.Add(element);
    //    }

    //    public ConnectionAdditionalOptions Deserialize(XElement root)
    //    {
    //        if (!root.Name.Equals((XName)"AdditionalOptions"))
    //            throw new ArgumentException("Element must have name AdditionalOptions");

    //        ConnectionAdditionalOptions ret =  new ConnectionAdditionalOptions();

    //        XElement element = root.Element("AllowSaveForAllTypes");
    //        if (element != null)
    //            ret.AllowSaveForAllTypes = bool.Parse(element.Value);
    //        element = root.Element("BlanketIgnoreExtraElements");
    //        if (element != null)
    //            ret.BlanketIgnoreExtraElements = bool.Parse(element.Value);

    //        element = root.Element("ExplicitSaveAllowedTypes");
    //        if(element != null)
    //            foreach (XElement xElement in element.Elements())
    //            {
    //                if(xElement.Name != "type")
    //                    throw new InvalidOperationException("Unexpected element " + xElement + "in element collection " + element);

    //                ret.ex
    //            }

    //        return ret;
    //    }
    //}

#endregion
}
