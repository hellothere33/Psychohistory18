using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Runtime.EntryPoints;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.BinarySerializer;
using Util.Net;
using DicFact = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LoadCsv.DataFacturation>>;
using DicPaie = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LoadCsv.DataPaiements>>;
using DicPerf = System.Collections.Generic.Dictionary<string, LoadCsv.DataPerformance>;
using DicTran = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LoadCsv.DataTransactions>>;

namespace LoadCsv
{
    /// <summary>
    /// Write a .csv file, called .matched file, where all text values are analyzed and embedded into numerical values.
    /// This is done because neural networks can only work with numerical values.
    /// </summary>
    public static class Analysis
    {
        public static List<NnRow> outputs = new List<NnRow>(); // contains all rows of numbers to write for N.Network


        /// <summary>
        /// Convert a curation csv file, containing such as textual biz names and addresses, to a csv file with only matched results in double.
        /// Can return null if the matched file already exist, or if something went wrong during calculations.
        /// </summary>
        /// <param name="CsvPath"></param>
        /// <returns></returns>
        public static List<NnRow> AnalyzeAndCreateColumnsForNNetwork(bool trainNotTest, bool useFull, bool loadBin)
        {
            DicFact dicFact; DicPaie dicPaie; DicPerf dicPerf; DicTran dicTran;
            if (loadBin) Utils.GetDataFromBinFiles(out dicFact, out dicPaie, out dicPerf, out dicTran);
            else         Utils.GetDataFromCsvFiles(out dicFact, out dicPaie, out dicPerf, out dicTran, trainNotTest, useFull);


            // create csv files containing that embed text in numerical values, by matching parts of business name and address
            int nbRows = dicPerf.Keys.Count;
            var clients = dicPerf.Keys.ToList();
            var rowArray = new NnRow[nbRows]; // we use an array here, because a list isn't compatible with parallelism

            /**/
            // Using single CPU core, good for debugging
            for (int i = 0; i < nbRows; i++)
                rowArray[i] = CalculateRow(clients[i], dicFact, dicPaie, dicPerf, dicTran);
            /*
                // Using multiple CPU cores, use only if there's a lot to process
                Parallel.For(0, nbRows, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
                {
                    rowArray[i] = CalculateRow(clients[i], dicFact, dicPaie, dicPerf, dicTran);
                });
            /**/

            List<NnRow> dataset = rowArray.ToList();
            dataset.RemoveAll(e => null == e);
            return dataset;
        }
        public static bool WriteToCsvFile(string CsvPath, List<NnRow> rows)
        {
            if (!string.IsNullOrWhiteSpace(CsvPath))
                ExportToFile(CsvPath, rows);
            return true;
        }

        public static string CreateNNetworkAndLearn(List<NnRow> rows)
        {
            // Prepare data
            double trainingSplitRatio = 0.7;
            int trainCount = (int)(rows.Count * trainingSplitRatio);
            var trainData = new MLNetData[trainCount];
            var testData = new MLNetData[rows.Count - trainCount];
            MLNetData[] allData = Convert(rows);
            // Split into Training and Testing sets
            Array.Copy(allData, 0, trainData, 0, trainCount);
            Array.Copy(allData, trainCount, testData, 0, rows.Count - trainCount);
            var allCollection   = CollectionDataSource.Create(allData);
            var trainCollection = CollectionDataSource.Create(trainData);
            var testCollection  = CollectionDataSource.Create(testData);

            
            double acc, auc, f1;
            PredictionModel<MLNetData, MLNetPredict> modelAll, modelTrain, modelBest;
            
          //(acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new EnsembleBinaryClassifier                      ());
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new AveragedPerceptronBinaryClassifier            ()); // acc 0.83, auc 0.86, f1 0.45
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new FastForestBinaryClassifier                    ()); // acc 0.85, auc 0.89, f1 0.46
            (acc, auc, f1, modelBest)= TrainAndGetMetrics(allCollection, allCollection, new FastTreeBinaryClassifier                      ()); // acc 0.95, auc 0.97, f1 0.85
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new FieldAwareFactorizationMachineBinaryClassifier()); // acc 0.85, auc 0.88, f1 0.56
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new GeneralizedAdditiveModelBinaryClassifier      ()); // acc 0.81, auc 0.80, f1 NaN
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new LinearSvmBinaryClassifier                     ()); // acc 0.82, auc 0.86, f1 0.16
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new LogisticRegressionBinaryClassifier            ()); // acc 0.84, auc 0.86, f1 0.40
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new StochasticDualCoordinateAscentBinaryClassifier()); // acc 0.84, auc 0.86, f1 0.40
            (acc, auc, f1, modelAll) = TrainAndGetMetrics(allCollection, allCollection, new StochasticGradientDescentBinaryClassifier     ()); // acc 0.83, auc 0.86, f1 0.29



            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new AveragedPerceptronBinaryClassifier            ()); // acc 0.82, auc 0.84, f1 0.45
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new FastForestBinaryClassifier                    ()); // acc 0.82, auc 0.83, f1 0.23
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new FastTreeBinaryClassifier                      ()); // acc 0.82, auc 0.84, f1 0.46
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new FieldAwareFactorizationMachineBinaryClassifier()); // acc 0.83, auc 0.85, f1 0.37
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new GeneralizedAdditiveModelBinaryClassifier      ()); // acc 0.81, auc 0.75, f1 NaN
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new LinearSvmBinaryClassifier                     ()); // acc 0.81, auc 0.83, f1 0.14
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new LogisticRegressionBinaryClassifier            ()); // acc 0.83, auc 0.84, f1 0.39
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new StochasticDualCoordinateAscentBinaryClassifier()); // acc 0.82, auc 0.84, f1 0.43
            (acc, auc, f1, modelTrain) = TrainAndGetMetrics(trainCollection, testCollection, new StochasticGradientDescentBinaryClassifier     ()); // acc 0.83, auc 0.83, f1 0.34

            // Evaluate a training model
            //Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            //Console.WriteLine($"Auc: {metrics.Auc:P2}");
            //Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            //var cv = new CrossValidator();
            //CrossValidationOutput<MLNetData, MLNetPredict> cvRes = cv.CrossValidate<MLNetData, MLNetPredict>(pipelineAll);
            //Console.WriteLine($"Rms = {metrics.Rms}");
            //Console.WriteLine($"RSquared = {metrics.RSquared}");


            // Train the overall model
            string NnModelPath = @"NnInputs\mlDotNet_Datacup.model";
            modelBest.WriteAsync(NnModelPath);
            return NnModelPath;
        }

        public static (double acc, double auc, double f1, PredictionModel<MLNetData, MLNetPredict> model) 
            TrainAndGetMetrics(ILearningPipelineLoader dataTrain, ILearningPipelineLoader dataTest, ILearningPipelineItem trainer)
        {
            var pipeline = new LearningPipeline();
            pipeline  .Add(dataTrain);
            pipeline  .Add(trainer);
            var model     = pipeline.Train<MLNetData, MLNetPredict>();
            var evaluator = new BinaryClassificationEvaluator();
            var metrics   = evaluator.Evaluate(model, dataTest);
            return (metrics.Accuracy, metrics.Auc, metrics.F1Score, model);
        }

        public static MLNetData[] Convert(List<NnRow> rows)
        {
            var allData = new MLNetData[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                allData[i] = new MLNetData();
                allData[i].Features = new float[Constants.len];
                var arr = Array.ConvertAll(rows[i].nbs, x => (float)x);
                Array.Copy(arr, 0, allData[i].Features, 0, arr.Length);
                allData[i].Label = rows[i].verdict/* == 0? false: true*/;
            }
            return allData;
        }

        public static List<DataSolution> Predict(string NnModelPath)
        {
            List<DataSolution> predictions = new List<DataSolution>();
            if (string.IsNullOrWhiteSpace(NnModelPath)) return predictions;

            var task = PredictionModel.ReadAsync<MLNetData, MLNetPredict>(NnModelPath);
            PredictionModel<MLNetData, MLNetPredict> model = task.Result;

            List<NnRow> NnRows = Analysis.AnalyzeAndCreateColumnsForNNetwork(trainNotTest: false, useFull: true, loadBin: false);
            MLNetData[] testSet = Convert(NnRows);
            List<MLNetPredict> preds = model.Predict(testSet).ToList();
            if (NnRows.Count != preds.Count) throw new InvalidDataException();

            // Write out predictions to CSV
            for (int i = 0; i < NnRows.Count; i++)
                predictions.Add(new DataSolution() { ID_CPTE = NnRows[i].id, Default = (int)preds[i].PredictedLabel });

            return predictions;
        }



        /// <summary>
        /// Very important function where we decide which columns to use during comparisons, 
        /// and how many values their comparisons are converted into
        /// </summary>
        /// <param name="csvDatas"></param>
        /// <param name="rowArray"></param>
        /// <param name="i"></param>
        private static NnRow CalculateRow(string key, DicFact dicFact, DicPaie dicPaie, DicPerf dicPerf, DicTran dicTran)
        {
            var rowsFact = dicFact.ContainsKey(key) ? dicFact[key] : null;
            var rowsPaie = dicPaie.ContainsKey(key) ? dicPaie[key] : null;
            var rowPerf  = dicPerf.ContainsKey(key) ? dicPerf[key] : null;
            var rowsTran = dicTran.ContainsKey(key) ? dicTran[key] : null;

            int verdict = rowPerf.Default.HasValue ? rowPerf.Default.Value : 0;
            DateTime predictionStartDate = rowPerf.PERIODID_MY;
            var ms = new List<NnRow>();

            //---------------------------------------------
            // Calculate here data representing variables which we think is correlated to the verdict

            // nb of delinquent cycles already
            ms.Add(new NnRow(Utils.GetNbOfDelinquencies(rowsFact)));

            ms.Add(new NnRow(Utils.GetFacturationCashBalance        (predictionStartDate, rowsFact, 28)));
            ms.Add(new NnRow(Utils.GetFacturationCurrentTotalBalance(predictionStartDate, rowsFact, 28)));
            ms.Add(new NnRow(Utils.GetFacturationCashBalance        (rowsFact))); // basic statistics such as min, max, average, std, var
            ms.Add(new NnRow(Utils.GetFacturationCreditLimit        (rowsFact))); // basic statistics such as min, max, average, std, var
            ms.Add(new NnRow(Utils.GetFacturationCurrentTotalBalance(rowsFact))); // basic statistics such as min, max, average, std, var
            
            ms.Add(new NnRow(Utils.GetPaiements        (predictionStartDate, rowsPaie, 28))); // paiements amounts in 4 weeks
            ms.Add(new NnRow(Utils.GetPaiementsStats   (rowsPaie))); // basic statistics such as min, max, average, std, var

            ms.Add(new NnRow(Utils.GetTransactions(predictionStartDate, rowsTran, 28))); // transaction amounts in 4 weeks
            ms.Add(new NnRow(Utils.GetTransactionsStats(rowsTran))); // basic statistics such as min, max, average, std, var

            


            //---------------------------------------------
            // export info back to caller
            return new NnRow(verdict, ms, rowPerf.ID_CPTE);
        }

        /// <summary>
        /// Write a csv file where all text values are analyzed and embedded into numerical values. This is done to 
        /// corresponding to the input 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sep"></param>
        public static void ExportToFile(string path, List<DataSolution> rows, string sep = ",")
        {
            // To prevent errors, we'll write to a temporary file, then change its file name
            string temp = Path.ChangeExtension(path, ".temp");
            if (File.Exists(temp)) File.Delete(temp);
            if (File.Exists(path)) File.Delete(path);

            var file = FileSolution.ExportCsvFile(path, rows);
        }

        /// <summary>
        /// Write a csv file where all text values are analyzed and embedded into numerical values. This is done to 
        /// corresponding to the input 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sep"></param>
        public static void ExportToFile(string path, List<NnRow> rows, string sep = ",")
        {
            // To prevent errors, we'll write to a temporary file, then change its file name
            string temp = Path.ChangeExtension(path, ".temp");
            if (File.Exists(temp)) File.Delete(temp);
            if (File.Exists(path)) File.Delete(path);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(temp))
            {
                // Write header
                string header = "id"      + sep + 
                                "verdict" + sep;
                for (int i = 0; i < rows[0].nbs.Length-1; i++)
                    header += $"val{i+1}{sep}";
                header += $"val{rows[0].nbs.Length}";
                file.WriteLine(header);

                // Write each row
                for (int i = 0; i < rows.Count; i++)
                {
                    string content = rows[i].id      + sep +
                                     rows[i].verdict + sep +
                                     string.Join(sep, rows[i].nbs);
                    file.WriteLine(content);
                }
            }
            File.Move(temp, path);
        }

    }
}
