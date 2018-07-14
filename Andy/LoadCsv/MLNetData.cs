using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadCsv
{
    public static class Constants
    {
        public const int len = 90;
    }
    public class MLNetData
    {
        [ColumnName("Features")]
        [VectorType(Constants.len)]
        public float[] Features;

        [ColumnName("Label")]
        public float Label;

        public override string ToString()
        {
            return $"lbl:{Label} {Features[0]} {Features[1]} {Features[2]}";
        }
    }
    public class MLNetPredict
    {
        [ColumnName("PredictedLabel")]
        public DvBool PredictedLabel;

        public override string ToString()
        {
            return $"pred:{PredictedLabel}";
        }
    }
}
