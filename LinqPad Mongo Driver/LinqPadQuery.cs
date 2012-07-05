using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GDSX.Externals.LinqPad.Driver
{
    public class LinqPadQuery
    {
        public XDocument ConnectionInfo { get; set; }

        public String Query { get; set; }

        public String Location { get; set; }

        public IEnumerable<string> References
        {
            get
            {
                if (this.ConnectionInfo == null || this.ConnectionInfo.Root == null)
                    return Enumerable.Empty<string>();

                return this.ConnectionInfo.Root.Elements("Reference").Select(x => x.Value);
            }
        }

        public IEnumerable<string> Namespaces
        {
            get
            {
                if (this.ConnectionInfo == null || this.ConnectionInfo.Root == null)
                    return Enumerable.Empty<string>();

                return this.ConnectionInfo.Root.Elements("Namespace").Select(x => x.Value);
            }
        }

        public LinqPadQuery()
        {
        
        }

        public LinqPadQuery(XDocument connectionInfo, string query)
        {
            this.ConnectionInfo = connectionInfo;
            this.Query = query;
        }

        public static LinqPadQuery CreateFrom(string path)
        {
            LinqPadQuery retval = CreateFrom(File.ReadAllBytes(path));
            retval.Location = path;
            return retval;
        }

        public static LinqPadQuery CreateFrom(byte[] bytes)
        {
            XDocument doc;
            string query;
            using (var memStream = new MemoryStream(bytes))
            {
                using (var reader = new XmlTextReader(memStream))
                {
                    reader.Read();
                    if (reader.Name != "Query")
                    {
                        return null;
                    }

                    doc = XDocument.Load(reader.ReadSubtree());

                    memStream.Position = 0;
                    using (StreamReader sr = new StreamReader(memStream))
                    {
                        for (int i = 0; i < reader.LineNumber; i++)
                        {
                            sr.ReadLine();
                        }

                        query = sr.ReadToEnd();
                    }
                }
            }

            return new LinqPadQuery(doc, query);
        }

        public bool Equals(LinqPadQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return XNode.EqualityComparer.Equals(other.ConnectionInfo, this.ConnectionInfo) &&
                string.Equals(other.Location, Location, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(other.Query, Query, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LinqPadQuery)) return false;
            return Equals((LinqPadQuery) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ConnectionInfo != null ? XNode.EqualityComparer.GetHashCode(this.ConnectionInfo) : 0);
                result = (result*397) ^ (Location != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Location) : 0);
                result = (result * 397) ^ (Query != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Query) : 0);
                return result;
            }
        }
    }


    public class LinqPadQuerySerializer : IXElementSerializer<LinqPadQuery>
    {
        public void Serialize(XElement root, LinqPadQuery obj)
        {
            if(obj.ConnectionInfo != null)
            {
                XElement el = new XElement("ConnectionInfo");
                el.Add(obj.ConnectionInfo.Root);
                root.Add(el);
            }

            if(!string.IsNullOrEmpty(obj.Location))
            {
                root.SetElementValue("Location", obj.Location);
            }

            if(!string.IsNullOrEmpty(obj.Query))
            {
                root.SetElementValue("Query", obj.Query);
            }
        }

        public LinqPadQuery Deserialize(XElement root)
        {
            LinqPadQuery ret = new LinqPadQuery();

            XElement element = root.Element("ConnectionInfo");
            if(element != null)
            {
                XDocument doc = new XDocument(
                    element.Elements().FirstOrDefault()
                );
                ret.ConnectionInfo = doc;
            }

            element = root.Element("Location");
            if(element != null)
            {
                ret.Location = element.Value;
            }

            element = root.Element("Query");
            if(element != null)
            {
                ret.Query = element.Value;
            }

            return ret;
        }
    }

}
