using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Net;

namespace LoadCsv
{
    /// <summary>
    /// Write a .csv file, called .matched file, where all text values are analyzed and embedded into numerical values.
    /// This is done because neural networks can only work with numerical values.
    /// </summary>
    public class FileAnalysis
    {
        public string             path = null;
        public List<DataAnalysis> rows = null;
        public FileAnalysis(string p, List<DataAnalysis> r) { path = p; rows = r; }
        public bool IsValid()
        {
            if (uNet.IsNullOrEmpty(rows) || null == rows[0].scores || uNet.IsNullOrEmpty(rows[0].scores))
                return false; // no data to write
            return true;
        }

        public static void LoadTrainSets(out FileFacturation fileFactTrain, out FilePaiements filePaieTrain,
                                         out FilePerformance filePerfTrain, out FileTransactions fileTranTrain)
        {
            fileFactTrain = FileFacturation .LoadCsvFile(@"Data/facturation_train.csv");
            filePaieTrain = FilePaiements   .LoadCsvFile(@"Data/paiements_train.csv");
            filePerfTrain = FilePerformance .LoadCsvFile(@"Data/performance_train.csv");
            fileTranTrain = FileTransactions.LoadCsvFile(@"Data/transactions_train.csv");
        }

        public static void LoadTestSets(out FileFacturation fileFactTest, out FilePaiements filePaieTest,
                      out FilePerformance filePerfTest, out FileTransactions fileTranTest, out FileSolution fileSolution)
        {
            fileFactTest = FileFacturation .LoadCsvFile(@"Data/facturation_test.csv");
            filePaieTest = FilePaiements   .LoadCsvFile(@"Data/paiements_test.csv");
            filePerfTest = FilePerformance .LoadCsvFile(@"Data/performance_test.csv");
            fileTranTest = FileTransactions.LoadCsvFile(@"Data/transactions_test.csv");
            fileSolution = FileSolution    .LoadCsvFile(@"Data/sample_solution.csv");
        }

        /// <summary>
        /// Write a csv file where all text values are analyzed and embedded into numerical values. This is done to 
        /// corresponding to the input 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sep"></param>
        public void ExportToFile(string path, string sep = ",")
        {
            if (!IsValid()) return; // no data to write

            // To prevent errors, we'll write to a temporary file, then change its file name
            string temp = Path.ChangeExtension(path, ".temp");
            if (File.Exists(temp)) File.Delete(temp);
            if (File.Exists(path)) File.Delete(path);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(temp))
            {
                // Write header
                string header = "Default" + sep;
                for (int i = 0; i < rows[0].scores.Length-1; i++)
                    header += $"val{i+1}{sep}";
                header += $"val{rows[0].scores.Length}";
                file.WriteLine(header);

                // Write each row
                for (int i = 0; i < rows.Count; i++)
                {
                    string content = rows[i].verdict + sep + string.Join(sep, rows[i].scores);
                    file.WriteLine(content);
                }
            }
            File.Move(temp, path);
        }

        /// <summary>
        /// Convert a curation csv file, containing such as textual biz names and addresses, to a csv file with only matched results in double.
        /// Can return null if the matched file already exist, or if something went wrong during calculations.
        /// </summary>
        /// <param name="CsvPath"></param>
        /// <returns></returns>
        public static FileAnalysis AnalyzeAndEmbedCurationExport(string CsvPath, bool removeRowsWithoutVerdict)
        {
            string matPath = CsvPath;
            if (File.Exists(matPath)) return null; // to save time, don't generate again if it's already done before

            LoadTrainSets(out FileFacturation fileFactTrain, out FilePaiements filePaieTrain,
                          out FilePerformance filePerfTrain, out FileTransactions fileTranTrain);
            //LoadTestSets(out FileFacturation fileFactTest, out FilePaiements filePaieTest,
            //        out FilePerformance filePerfTest, out FileTransactions fileTranTest, out FileSolution fileSolution);
            

            // create csv files containing that embed text in numerical values, by matching parts of business name and address
            int nbRows = filePerfTrain.rows.Count;
            var rowArray = new DataAnalysis[nbRows]; // we use an array here, because a list isn't compatible with parallelism
            
/**/
            // Using single CPU core, good for debugging
            for (int i = 0; i < nbRows; i++)
                CalculateRow(rowArray, i, fileFactTrain, filePaieTrain, filePerfTrain, fileTranTrain);
/*
            // Using multiple CPU cores, use only if there's a lot to process
            Parallel.For(0, nbRows, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
            {
                CalculateRow(rowArray, i, fileFactTrain, filePaieTrain, filePerfTrain, fileTranTrain);
            });
/**/

            var listAllScores = rowArray.ToList();
            FileAnalysis matchedFile = new FileAnalysis(matPath, listAllScores);
            if (nbRows != matchedFile.rows.Count)
            {
                Console.WriteLine($"Error in file {matPath}:");
                Console.WriteLine($"we should always have the same number of rows in the original csv {nbRows} as in the matched csv {matchedFile.rows.Count}.");
                return null;
            }

            matchedFile.ExportToFile(matchedFile.path);
            return matchedFile;
        }

        /// <summary>
        /// Very important function where we decide which columns to use during comparisons, 
        /// and how many values their comparisons are converted into
        /// </summary>
        /// <param name="csvDatas"></param>
        /// <param name="rowArray"></param>
        /// <param name="i"></param>
        private static void CalculateRow(DataAnalysis[] rowArray, int i,
                                         FileFacturation fileFactTrain, FilePaiements filePaieTrain,
                                         FilePerformance filePerfTrain, FileTransactions fileTranTrain)
        {
            var clientId = filePerfTrain.rows[i].ID_CPTE;
            var rowsFact = fileFactTrain.GetClientRows(clientId);
            var rowsPaie = filePaieTrain.GetClientRows(clientId);
            var rowsTran = fileTranTrain.GetClientRows(clientId);
            

            int verdict = filePerfTrain.rows[i].Default;
            var ms      = new List<DataAnalysis>();

            //---------------------------------------------
            // trigger special cases
            //ms.Add(new DataAnalysis(ContainOrMatchTheseWords(row, wordsPartial, wordsExactly)));
        }
    }
    public class DataAnalysis
    {
        private const double defaultScoreForWhenMatchingWasntPerformed = 0;
        private const double lowestScoreWhenMatchingWasAtWorst = 0.001;
        public double[] scores = null;
        public int      verdict= -1;
        public int      count { get { return null == scores? -1 : scores.Length; } }
        public DataAnalysis(int maxNbWords)
        {
            scores = CreateArray(maxNbWords, defaultScoreForWhenMatchingWasntPerformed);
        }

        public DataAnalysis(double[] _scores)
        {
            scores = _scores;
        }

        /// <summary>
        /// Guaranteed to return a valid output data
        /// </summary>
        public DataAnalysis(double value)
        {
            // To guarantee returning a valid output data, we create a fixed sized table then fill in value
            scores = new double[1];
            scores[0] = value;
            verdict = -1; // set to impossible value, so we know something went wrong if it appeared in the final export
        }
        /// <summary>
        /// Put several horizontal pieces of a row together, into a long horizontal row
        /// </summary>
        public DataAnalysis(int Verdict, List<DataAnalysis> ms)
        {
            verdict = Verdict;
            int expectedNbCols = 0;
            if (uNet.IsNullOrEmpty(ms))
            {
                scores = new double[0];
                return;
            }
            // To guarantee returning a valid output data, we create a fixed sized table then fill in values
            ms.ForEach(m => expectedNbCols += m.scores.Length);
            scores = CreateArray(expectedNbCols, defaultScoreForWhenMatchingWasntPerformed);

            int lengthSoFar = 0;
            for (int i = 0; i < ms.Count; i++)
            {
                ms[i].scores.CopyTo(scores, lengthSoFar);
                lengthSoFar += ms[i].scores.Length;
            }
        }
        public override string ToString()
        {
            string s = $"{verdict}: ";
            foreach (var d in scores)
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
        
        ///// <summary>
        ///// </summary>
        //public static DataAnalysis Compare2PhoneNumbers(string text1, string text2)
        //{
        //    string[] wds1 = uStr.ExtractPhoneNumbers(text1);
        //    string[] wds2 = uStr.ExtractPhoneNumbers(text2);
        //    var match = new DataAnalysis(wds1.Length); // area + local
        //    for (int i = 0; i < wds1.Length; i++)
        //        match.scores[i] = GetHighestSimilarityScore(wds1[i], wds2);
        //    return match;
        //}
    }
}
