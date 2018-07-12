﻿using Microsoft.ML.Runtime.Api;
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
        public const int len = 10;
    }
    public class MLNetData
    {
        [ColumnName("Features")]
        [VectorType(Constants.len)]
        public float[] Features;

        [ColumnName("Label")]
        public float Label;
    }
    public class MLNetPredict
    {
        [ColumnName("PredictedLabel")]
        public DvBool PredictedLabel;
    }
}