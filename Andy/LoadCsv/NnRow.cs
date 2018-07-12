using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Net;

namespace LoadCsv
{
    /// <summary>
    /// A row of numbers for consumption by a neural network
    /// </summary>
    [Serializable]
    public class NnRow
    {
        private const double defaultScoreForWhenMatchingWasntPerformed = 0;
        private const double lowestScoreWhenMatchingWasAtWorst = 0.001;
        public double[] nbs = null;
        public int      verdict= -1;
        public int      count { get { return null == nbs? -1 : nbs.Length; } }
        public NnRow(int maxNbWords)
        {
            nbs = CreateArray(maxNbWords, defaultScoreForWhenMatchingWasntPerformed);
        }

        public NnRow(double[] _scores)
        {
            nbs = _scores;
        }

        /// <summary>
        /// Guaranteed to return a valid output data
        /// </summary>
        public NnRow(double value)
        {
            // To guarantee returning a valid output data, we create a fixed sized table then fill in value
            nbs = new double[1];
            nbs[0] = value;
            verdict = -1; // set to impossible value, so we know something went wrong if it appeared in the final export
        }
        /// <summary>
        /// Put several horizontal pieces of a row together, into a long horizontal row
        /// </summary>
        public NnRow(int Verdict, List<NnRow> ms)
        {
            verdict = Verdict;
            int expectedNbCols = 0;
            if (uNet.IsNullOrEmpty(ms))
            {
                nbs = new double[0];
                return;
            }
            // To guarantee returning a valid output data, we create a fixed sized table then fill in values
            ms.ForEach(m => expectedNbCols += m.nbs.Length);
            nbs = CreateArray(expectedNbCols, defaultScoreForWhenMatchingWasntPerformed);

            int lengthSoFar = 0;
            for (int i = 0; i < ms.Count; i++)
            {
                ms[i].nbs.CopyTo(nbs, lengthSoFar);
                lengthSoFar += ms[i].nbs.Length;
            }
        }
        public override string ToString()
        {
            string s = $"{verdict}: ";
            foreach (var d in nbs)
                s += $"{d} ";
            return s;
        }
        public static double[] CreateArray(int length, double defaultValue)
        {
            double[] scores = new double[length];
            // all set to 0 by defaultdefault
            for (int i = 0; i < scores.Length; i++)
                scores[i] = defaultValue;
            return scores;
        }
    }
}
