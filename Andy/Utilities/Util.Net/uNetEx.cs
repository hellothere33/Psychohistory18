using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Net
{
    public static class uNetEx
    {
        public static List<T> ToFlatList<T>(this Dictionary<Guid, List<T>> aggregate)
        {
            var output = new List<T>();
            if (null == aggregate || aggregate.Keys.Count <= 0 || aggregate.Values.Count <= 0) return output;

            foreach (var k in aggregate.Keys)
            {
                if (null == aggregate[k] || aggregate[k].Count <= 0)
                    continue;
                output.AddRange(aggregate[k]);
            }
            return output;
        }

        public static List<T> ToFlatList<T>(this Dictionary<int, List<T>> aggregate)
        {
            var output = new List<T>();
            if (null == aggregate || aggregate.Keys.Count <= 0 || aggregate.Values.Count <= 0) return output;

            foreach (var k in aggregate.Keys)
            {
                if (null == aggregate[k] || aggregate[k].Count <= 0)
                    continue;
                output.AddRange(aggregate[k]);
            }
            return output;
        }

        /// <summary>
        /// Append a new dictionary to an aggregate dictionary. The aggregate will contain everything by reference in the end.
        /// There is no check for item duplication.
        /// </summary>
        public static void Append<T>(this Dictionary<int, List<T>> aggregate, Dictionary<int, List<T>> newFragmentToAppend)
        {
            if (null == aggregate) aggregate = new Dictionary<int, List<T>>();
            if (null == newFragmentToAppend) return;

            foreach (var k in newFragmentToAppend.Keys)
            {
                if (null == newFragmentToAppend[k])
                    continue;
                if (!aggregate.ContainsKey(k))
                    aggregate.Add(k, new List<T>());
                aggregate[k].AddRange(newFragmentToAppend[k]);
            }
        }

        /// <summary>
        /// Append a new dictionary to an aggregate dictionary. The aggregate will contain everything by reference in the end.
        /// There is no check for item duplication.
        /// </summary>
        public static void Append<T>(this Dictionary<Guid, List<T>> aggregate, Dictionary<Guid, List<T>> newFragmentToAppend)
        {
            if (null == aggregate) aggregate = new Dictionary<Guid, List<T>>();
            if (null == newFragmentToAppend) return;

            foreach (var k in newFragmentToAppend.Keys)
            {
                if (null == newFragmentToAppend[k])
                    continue;
                if (!aggregate.ContainsKey(k))
                    aggregate.Add(k, new List<T>());
                aggregate[k].AddRange(newFragmentToAppend[k]);
            }
        }

    }
}
