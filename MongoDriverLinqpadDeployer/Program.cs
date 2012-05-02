using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MongoDriverLinqpadDeployer
{
    class Program
    {
        private static readonly string[] Files = new[]
        {
            "LinqPadMongoDriver.dll", "LinqPadMongoDriver.pdb",
            "..\\..\\lib\\MongoDB.Bson.dll", "..\\..\\lib\\MongoDB.Driver.dll",
        };

        private static void Main(string[] args)
        {
            string publicKeyToken = string.Empty;
            var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), Files.First()));
            assembly.GetName().GetPublicKeyToken().Select(t => publicKeyToken += t.ToString("x2")).ToList();

            string deploymentPath = string.Format(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                @"LINQPad\Drivers\DataContext\4.0\MongoLinqpadDriver ({0})\"), publicKeyToken);

            Console.WriteLine("Output path: " + deploymentPath);

            if (!Directory.Exists(deploymentPath))
                Directory.CreateDirectory(deploymentPath);

            Console.WriteLine(Directory.GetCurrentDirectory());

            foreach (string file in Files)
            {
                var destFile = Path.Combine(deploymentPath, Path.GetFileName(file));

                if (File.Exists(destFile))
                    File.Delete(destFile);

                if (File.Exists(file))
                    File.Copy(file, destFile);
                else
                    Console.WriteLine("Couldnt find file " + Path.GetFullPath(file));
            }

            Console.ReadKey();
        }
    }
}
