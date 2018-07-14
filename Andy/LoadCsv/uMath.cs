using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadCsv
{
    public static class uMath
    {
        public static int Int(this double d)
        {
            return (int)System.Math.Round(d, 0, MidpointRounding.AwayFromZero);
        }
        public static int CompareDates(DateTime a, DateTime b, bool sortByMostRecentFirst = true)
        {
            //int output = 0;
            //a.CompareTo()
            //if (a < b.StatementDate) output = 1;
            //if (b < a.StatementDate) output = -1;
            return 0;
        }
    }
}
