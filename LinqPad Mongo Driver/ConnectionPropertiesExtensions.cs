using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace GDSX.Externals.LinqPad.Driver
{
    internal static class ConnectionPropertiesExtensions
    {
        /// <summary>
        /// Connects to the given Mongo server and trims the list of databases and collections based
        /// on what collections actually exist currently in the DB.
        /// </summary>
        /// <param name="mongo">The MongoServer instance to connect to.</param>
        /// <param name="dbMappings">The list of existing DB mappings.</param>
        public static void TrimDatabaseMappings(this MongoServer mongo, Dictionary<String, HashSet<CollectionTypeMapping>> dbMappings)
        {
            HashSet<string> seenDBs = new HashSet<string>();

            //Load up each database by name, retrieving the current collection mappings.
            foreach (string name in mongo.GetDatabaseNames())
            {
                seenDBs.Add(name);

                HashSet<CollectionTypeMapping> mappings = null;
                if (!dbMappings.TryGetValue(name, out mappings))
                {
                    mappings = new HashSet<CollectionTypeMapping>();
                    dbMappings[name] = mappings;
                }

                var mappingLookup = mappings.ToDictionary(x => x.CollectionName);
                HashSet<string> seenCollections = new HashSet<string>();

                MongoDatabase db = mongo.GetDatabase(name);
                foreach (string collectionName in db.GetCollectionNames())
                {
                    seenCollections.Add(collectionName);

                    CollectionTypeMapping mapping = null;
                    if (!mappingLookup.TryGetValue(collectionName, out mapping))
                    {
                        mapping = new CollectionTypeMapping
                        {
                            CollectionName = collectionName
                        };
                        mappings.Add(mapping);
                    }
                }

                //trim collections that don't exist in the DB
                mappings.RemoveWhere(x => !seenCollections.Contains(x.CollectionName));
            }

            //trim all DB's that don't exist in the connection
            foreach (string dbName in dbMappings.Keys.ToArray())
            {
                if (!seenDBs.Contains(dbName))
                    dbMappings.Remove(dbName);
            }
        }

    }
}
