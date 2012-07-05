using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDSX.Externals.LinqPad.Driver
{
    public class DynamicComparer<T> : IComparer<T>
    {
        public Func<T, T, int> Comparer { get; private set; }

        public DynamicComparer(Func<T, T, int> comparer)
        {
            this.Comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return Comparer(x, y);
        }
    }

    public class CollectionEqualityComparer<T> : IEqualityComparer<ICollection<T>>
    {
        public bool Equals(ICollection<T> x, ICollection<T> y)
        {
            //if one is null the other should be null or empty
            if (x == null)
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

    /// <summary>
    /// Compares two assembly paths to see if they point to the same .dll file, in different locations.
    /// </summary>
    public class AssemblyPathEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Equals(
                    System.IO.Path.GetFileName(x),
                    System.IO.Path.GetFileName(y)
                );
        }

        public int GetHashCode(string obj)
        {
            string filename = System.IO.Path.GetFileName(obj);
            if(filename == null)
                return 0;

            return filename.GetHashCode();
        }
    }

}
