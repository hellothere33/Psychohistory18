using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sMath = System.Math;

namespace Util.Net
{
    public class uNet
    {
        /// <summary>
        /// Verify if a generic list is null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(T[] list)
        {
            if (null == list || list.Length <= 0) return true;
            return false;
        }

        /// <summary>
        /// Verify if a generic list is null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(List<T> list)
        {
            if (null == list || list.Count <= 0) return true;
            return false;
        }
        /// <summary>
        /// Verify if a generic list is null or empty
        /// </summary>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> list)
        {
            if (null == list || list.Count() <= 0) return true;
            return false;
        }

        /// <summary>
        /// Verify if a generic list is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static void GetListOfNulls<T>(List<T> list, int capacity)
        {
            list = new List<T>(capacity);
            for (int i = 0; i < capacity; i++)
                list.Add(default(T));
        }

        /// <summary>
        /// Return indexes at which the input list's elements are null. This will return an empty list instead of a null list.
        /// </summary>
        public static List<int> GetIndexesOfNulls<T>(IEnumerable<T> list)
        {
            List<int> idxOfNulls = new List<int>();
            for (int i = 0; i < list.Count(); i++)
                if (null == list.ElementAt(i)) idxOfNulls.Add(i);
            return idxOfNulls;
        }

        public static List<List<Tuple<T, T>>> FindAllCombinations<T>(List<List<T>> clusters)
        {
            if (IsNullOrEmpty(clusters)) return new List<List<Tuple<T, T>>>();

            var listOfAllCombinations = new List<List<Tuple<T, T>>>();
            clusters.ForEach(e => listOfAllCombinations.Add(FindAllCombinations(e)));
            return listOfAllCombinations;
        }

        /// <summary>
        /// Returns a list of paired elements. These pairs are all possible non-duplicated combinations of elements
        /// in the input list.
        /// The output list is a shallow copy.
        /// </summary>
        public static List<Tuple<T, T>> FindAllCombinations<T>(List<T> cluster)
        {
            if (IsNullOrEmpty(cluster) || 1 == cluster.Count) return new List<Tuple<T, T>>();

            var allCombinations = new List<Tuple<T, T>>();
            for (int i = 0; i < cluster.Count - 1; i++)
            {
                for (int j = i + 1; j < cluster.Count; j++)
                {
                    allCombinations.Add(new Tuple<T, T>(cluster[i], cluster[j]));
                }
            }
            return allCombinations;
        }

        /// <summary>
        /// Delegate func that allow comparison of e1 and e2, returns true if they are related in some way.
        /// </summary>
        public delegate bool Delegate_AreTheyRelated<T, T2>(T e1, T e2, T2 criteria);

        /// <summary>
        /// Generic algo to group a list of elements together in new list of separate sub-lists, according to a criteria returned by the delegate function.
        /// Guaranteed to never return null. An empty list is the minimum.
        /// </param>
        public static List<List<T>> GenericGroupingCallingDelegate<T, T2>(List<T> inputs, T2 criteria, Delegate_AreTheyRelated<T, T2> AreTheyRelated)
        {
            var outputLists = new List<List<T>>();
            if (IsNullOrEmpty(inputs)) return outputLists;

            var clones = new List<T>(inputs);
            int antiInfinitLoopingCount = clones.Count * clones.Count;

            do
            {
                // maintenance
                antiInfinitLoopingCount--;
                var temp = new List<T>();

                // retrive next subject
                temp.Add(clones[0]);
                clones.RemoveAt(0);

                // find other elements that are related to current subject, according to the delegate function
                var toExtract = new List<int>();
                for (int i = 0; i < clones.Count; i++)
                {
                    bool related = false;
                    related = AreTheyRelated(temp[0], clones[i], criteria);
                    if (related)
                        toExtract.Add(i);
                }

                // extract overlaps
                for (int i = toExtract.Count - 1; 0 <= i; i--)
                {
                    temp.Add(clones[toExtract[i]]);
                    clones.RemoveAt(toExtract[i]);
                }

                // sanity check: should never happen
                if (temp.Count <= 0)
                    continue;

                outputLists.Add(temp);
            }
            while (0 < clones.Count && 0 < antiInfinitLoopingCount); // loop is still more left to be done and if we haven't tripped the inifite looping protection

            return outputLists;
        }

        /// <summary>
        /// Delegate func that returns the index of the element in the input list, that according to a provided function results in the
        /// smallest or highest value.
        /// </summary>
        public delegate double Delegate_CalculateSomeValue<T>(T e1);
        public static int GenericFindMinMaxDelegate<T>(List<T> inputs, bool trueForMinFalseForMax, Delegate_CalculateSomeValue<T> evalFunc)
        {
            if (IsNullOrEmpty(inputs)) return -1;

            var vals = new double[inputs.Count];
            int lastMinOrMax = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                double current = evalFunc(inputs[i]);
                vals[i] = current;
                if (i <= 0) continue;
                lastMinOrMax = returnIndexIfSmallerOrHigher(trueForMinFalseForMax, vals[lastMinOrMax], lastMinOrMax, i, current);
            }

            return lastMinOrMax;
        }

        /// <summary>
        /// Delegate func that returns the index of the element in the input list, that according to a provided function results in the
        /// smallest or highest value.
        /// </summary>
        public delegate double Delegate_CalculateSomeValue<T1, T2>(T1 e1, T2 e2);
        public static int GenericFindMinMaxDelegate<T1, T2>(T1 e1, List<T2> inputs, bool trueForMinFalseForMax, Delegate_CalculateSomeValue<T1, T2> evalFunc)
        {
            if (IsNullOrEmpty(inputs)) return -1;

            var vals = new double[inputs.Count];
            int lastMinOrMax = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                double current = evalFunc(e1, inputs[i]);
                vals[i] = current;
                if (i <= 0) continue;

                lastMinOrMax = returnIndexIfSmallerOrHigher(trueForMinFalseForMax, vals[lastMinOrMax], lastMinOrMax, i, current);
            }

            return lastMinOrMax;
        }

        private static int returnIndexIfSmallerOrHigher(bool trueForMinFalseForMax, double lastVal, int lastMinOrMax, int i, double current)
        {
            if (trueForMinFalseForMax)
            {
                if (current < lastVal)
                    lastMinOrMax = i;
            }
            else
            {
                if (lastVal < current)
                    lastMinOrMax = i;
            }

            return lastMinOrMax;
        }
    }
}
