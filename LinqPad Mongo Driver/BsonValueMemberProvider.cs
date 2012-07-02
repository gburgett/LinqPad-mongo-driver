using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

namespace GDSX.Externals.LinqPad.Driver
{
    public class BsonValueMemberProvider : LINQPad.ICustomMemberProvider
    {
        private static readonly HashSet<string> IgnoredProperties;
        private static readonly HashSet<Type> IgnoredTypes = new HashSet<Type>
            {
                typeof(BsonDocument),
                typeof(BsonArray)
            };

        private BsonValue mValue;
        private PropertyInfo[] mPropsToWrite;

        static BsonValueMemberProvider()
        {
            HashSet<string> ignored =new HashSet<string>
            {
                //special props
                "Item",
                "AsByteArray",
                "AsLocalTime",
                "AsRegex",
                "AsString",
                "AsUniversalTime",
                "AsStringOrNull",
                "IsNumeric",
                "IsString",
                "AsBsonValue"
            };
            //special names appearing in multiple props
            string[] toIgnore = new[]
                {
                    "Boolean",
                    "DateTime",
                    "Double",
                    "Guid",
                    "Int32",
                    "Int64",
                    "ObjectId"
                };
            AddRange(ignored, toIgnore.Select(x =>
                    string.Concat("As", x)
                ));
            AddRange(ignored, toIgnore.Select(x =>
                    string.Concat("AsNullable", x)
                ));
            AddRange(ignored, toIgnore.Select(x => 
                    string.Concat("Is", x)
                ));

            //bson types
            string[] types = Enum.GetNames(typeof(BsonType))
                .Concat(new []
                {
                    "BinaryData"
                }).ToArray();
            AddRange(ignored, types.Select(x =>
                    string.Concat("AsBson", x)
                ));
            AddRange(ignored, types.Select(x =>
                    string.Concat("IsBson", x)
                ));
            IgnoredProperties = ignored;
        }

        public static bool ShouldProvide(object obj)
        {
            return obj != null &&
                obj is BsonValue &&
                !IgnoredTypes.Contains(obj.GetType());
        }

        private static void AddRange(HashSet<string> set, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                set.Add(value);
            }
        }


        public BsonValueMemberProvider(BsonValue value)
        {
            this.mValue = value;
            this.mPropsToWrite = mValue.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !IgnoredProperties.Contains(p.Name) && p.GetIndexParameters().Length == 0)
                .ToArray();
        }


        public IEnumerable<string> GetNames()
        {
            return mPropsToWrite.Select(p => p.Name);
        }

        public IEnumerable<Type> GetTypes()
        {
            return mPropsToWrite.Select(p => p.PropertyType);
        }

        public IEnumerable<object> GetValues()
        {
            return mPropsToWrite.Select(p => p.GetValue(this.mValue, null));
        }


    }
}
