using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using GDSX.Externals.LinqPad.Driver;
using LINQPad.Extensibility.DataContext;

namespace TestApp
{
    class Program
    {
        static string outDir = @"\..\..\..\LinqPad Mongo Driver\bin\Debug";

        [STAThread]
        static void Main(string[] args)
        {
            ConsoleColor old = Console.ForegroundColor;
            try{
                var props = new ConnectionProperties();
                using (var frm = new ConnectionDialog(props, true, Assembly.LoadFrom))
                {
                    var result = frm.ShowDialog();
                    if (result != System.Windows.Forms.DialogResult.OK)
                        return;
                }

                var cxi = new FakeConnection();
                props.Serialize(cxi.DriverData);

                var driver = new MongoDynamicDataContextDriver();

                
                List<Assembly> assemblies = props.AssemblyLocations.Select(Assembly.LoadFrom).ToList();
                var code = driver.GetStaticCodeFiles().Concat(new string[] {driver.GenerateDynamicCode(props, assemblies, "", "driver")});

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("------------------------------------------------");
                foreach (string s in code)
                {
                    Console.WriteLine(s);
                    Console.WriteLine("------------------------------------------------");
                }
                Console.ForegroundColor = old;

                using (var frm = new SaveFileDialog())
                {
                    var result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(frm.OpenFile()))
                        {
                            foreach (string s in code)
                            {
                                writer.WriteLine(s);
                                writer.WriteLine("---------------------------------------------------");
                            }

                            writer.Flush();
                        }
                        
                    }
                }

                
               

            }catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
            }
            finally{
                Console.ForegroundColor = old;

                Console.ReadLine();
            }
        }

        private class FakeConnection : IConnectionInfo
        {
            public string Encrypt(string data)
            {
                return data;
            }

            public string Decrypt(string data)
            {
                return data;
            }

            public IDatabaseInfo DatabaseInfo
            {
                get { throw new NotImplementedException(); }
            }

            public ICustomTypeInfo CustomTypeInfo
            {
                get { throw new NotImplementedException(); }
            }

            public IDynamicSchemaOptions DynamicSchemaOptions
            {
                get { throw new NotImplementedException(); }
            }

            public string DisplayName
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string AppConfigPath
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public bool Persist
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            private XElement mDriverData = new XElement("Hi");
            public XElement DriverData
            {
                get { return mDriverData; }
                set { mDriverData = value; }
            }

            private IDictionary<string, object> mSessionData = new Dictionary<string, object>();
            public IDictionary<string, object> SessionData
            {
                get { return mSessionData; }
            }
        }
    }
}
