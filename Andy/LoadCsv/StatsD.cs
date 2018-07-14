using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Net;
using sMath = System.Math;

namespace LoadCsv
{
    /// <summary>
    /// Calculates stats from data
    /// </summary>
    public class StatsD
    {
        private List<double> Vals = null;
        public double Max { get; set; }
        public double Avg { get; set; }
        public double Min { get; set; }
        public double Std { get; set; }
        public double Var { get; set; }


        private StatsD()
        {
            Vals = new List<double>(100);
        }
        public static StatsD Create(List<double> inputs)
        {
            if (uNet.IsNullOrEmpty(inputs)) return null;
            var stat = new StatsD();

            stat.Vals = new List<double>(inputs);
            if (!stat.UpdateStats()) return null;
            return stat;
        }

        private bool UpdateStats()
        {
            if (uNet.IsNullOrEmpty(Vals)) return false;
            
            Max = Vals.Max();           
            Avg = uStats.Mean(Vals);    
            Min = Vals.Min();           
            Std = uStats.Std(Vals);     
            Var = uStats.Variance(Vals);     

            return true;
        }
    }
}
