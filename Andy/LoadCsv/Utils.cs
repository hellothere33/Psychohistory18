using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Util.BinarySerializer;
using Util.Net;
using DicFact = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LoadCsv.DataFacturation>>;
using DicPaie = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LoadCsv.DataPaiements>>;
using DicPerf = System.Collections.Generic.Dictionary<string, LoadCsv.DataPerformance>;
using DicTran = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<LoadCsv.DataTransactions>>;

namespace LoadCsv
{
    public static class Utils
    {
        private static bool norm = true;
        private static void CalculateStatsAndCopyToArray(double[] nbs, List<double> vals)
        {
            var stat = StatsD.Create(vals);
            nbs[0] = stat.Min; nbs[1] = stat.Avg; nbs[2] = stat.Max; nbs[3] = stat.Std; nbs[4] = stat.Var;
        }
        private static double[] NormalizeToOne(double[] nbs)
        {
            if (norm) { double max = nbs.Max(); max = max == 0 ? 1 : max; nbs = nbs.Select(i => i / max).ToArray(); } // normalize to be under 1.0
            return nbs;
        }

        #region Hypotheses
        public static double GetNbOfDelinquencies(List<DataFacturation> facts)
        {
            if (uNet.IsNullOrEmpty(facts)) return 0d;
            double d = 0;
            facts.ForEach(e =>
            {
                d += e.DelqCycle;
            });
            return System.Math.Min(1, d/20); // normalize to an arbitrary value near the top (0 ~ 28) with reasonaly high count 
        }

        public static double[] GetTransactionsStats(List<DataTransactions> list)
        {
            double[] nbs = NnRow.CreateArray(5); if (uNet.IsNullOrEmpty(list)) return nbs;
            var vals = new List<double>();
            for (int i = 0; i < list.Count; i++) vals.Add(list[i].TRANSACTION_AMT);
            CalculateStatsAndCopyToArray(nbs, vals);
            nbs = NormalizeToOne(nbs);
            return nbs;
        }

        public static double[] GetPaiementsStats(List<DataPaiements> list)
        {
            double[] nbs = NnRow.CreateArray(5); if (uNet.IsNullOrEmpty(list)) return nbs;
            var vals = new List<double>();
            for (int i = 0; i < list.Count; i++) vals.Add(list[i].TRANSACTION_AMT);
            CalculateStatsAndCopyToArray(nbs, vals);
            nbs = NormalizeToOne(nbs);
            return nbs;
        }
        public static double[] GetFacturationCashBalance(List<DataFacturation> list)
        {
            double[] nbs = NnRow.CreateArray(5); if (uNet.IsNullOrEmpty(list)) return nbs;
            var vals = new List<double>();
            for (int i = 0; i < list.Count; i++) vals.Add(list[i].CashBalance);
            CalculateStatsAndCopyToArray(nbs, vals);
            nbs = NormalizeToOne(nbs);
            return nbs;
        }
        public static double[] GetFacturationCreditLimit(List<DataFacturation> list)
        {
            double[] nbs = NnRow.CreateArray(5); if (uNet.IsNullOrEmpty(list)) return nbs;
            var vals = new List<double>();
            for (int i = 0; i < list.Count; i++) vals.Add(list[i].CreditLimit);
            CalculateStatsAndCopyToArray(nbs, vals);
            nbs = NormalizeToOne(nbs);
            return nbs;
        }
        public static double[] GetFacturationCurrentTotalBalance(List<DataFacturation> list)
        {
            double[] nbs = NnRow.CreateArray(5); if (uNet.IsNullOrEmpty(list)) return nbs;
            var vals = new List<double>();
            for (int i = 0; i < list.Count; i++) vals.Add(list[i].CurrentTotalBalance);
            CalculateStatsAndCopyToArray(nbs, vals);
            nbs = NormalizeToOne(nbs);
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetPaiements(DateTime predictionStartDate, List<DataPaiements> list, int days)
        {
            double[] nbs = NnRow.CreateArray(30);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataPaiements>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();
                double sum = shortList.Sum(e => e.TRANSACTION_AMT);
                nbs[i] = sum;
            }
            nbs = NormalizeToOne(nbs);
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCashBalance(DateTime predictionStartDate, List<DataFacturation> list, int days)
        {
            double[] nbs = NnRow.CreateArray(30);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            tempList.OrderByDescending(e => e.PERIODID_MY);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                double sum = shortList.Sum(e => e.CashBalance);
                nbs[i] = sum;
            }
            nbs = NormalizeToOne(nbs);
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCurrentTotalBalance(DateTime predictionStartDate, List<DataFacturation> list, int days)
        {
            double[] nbs = NnRow.CreateArray(30);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            tempList.OrderByDescending(e => e.PERIODID_MY);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                double sum = shortList.Sum(e => e.CurrentTotalBalance);
                nbs[i] = sum;
            }
            nbs = NormalizeToOne(nbs);
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetTransactions(DateTime predictionStartDate, List<DataTransactions> list, int days)
        {
            double[] nbs = NnRow.CreateArray(30);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataTransactions>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();
                double sum = shortList.Sum(e => e.TRANSACTION_AMT);
                nbs[i] = sum;
            }
            nbs = NormalizeToOne(nbs);
            return nbs;
        }
        #endregion


        public static List<T> SortMostRecentFirst<T>(List<T> list)
        {
            switch (list)
            {
                case List<DataFacturation > data: return data.OrderByDescending(e => e.StatementDate   ).ToList() as List<T>;
                case List<DataPaiements   > data: return data.OrderByDescending(e => e.TRANSACTION_DTTM).ToList() as List<T>;
                case List<DataPerformance > data: return data.OrderByDescending(e => e.PERIODID_MY     ).ToList() as List<T>;
                case List<DataTransactions> data: return data.OrderByDescending(e => e.TRANSACTION_DTTM).ToList() as List<T>;
            }
            return new List<T>();
        }


        public static void LoadTrainSets(out FileFacturation fileFactTrain, out FilePaiements    filePaieTrain,
                                         out FilePerformance filePerfTrain, out FileTransactions fileTranTrain)
        {
            fileFactTrain = FileFacturation .LoadCsvFile(@"Data/facturation_train.csv");
            filePaieTrain = FilePaiements   .LoadCsvFile(@"Data/paiements_train.csv");
            filePerfTrain = FilePerformance .LoadCsvFile(@"Data/performance_train.csv");
            fileTranTrain = FileTransactions.LoadCsvFile(@"Data/transactions_train.csv");
        }

        public static void LoadTestSets(out FileFacturation fileFactTest, out FilePaiements    filePaieTest,
                                        out FilePerformance filePerfTest, out FileTransactions fileTranTest)
        {
            fileFactTest = FileFacturation .LoadCsvFile(@"Data/facturation_test.csv");
            filePaieTest = FilePaiements   .LoadCsvFile(@"Data/paiements_test.csv");
            filePerfTest = FilePerformance .LoadCsvFile(@"Data/performance_test.csv");
            fileTranTest = FileTransactions.LoadCsvFile(@"Data/transactions_test.csv");
            //fileSolution = FileSolution.LoadCsvFile(@"Data/sample_solution.csv");
        }

        public static void GetDataFromBinFiles(out DicFact dicFact, out DicPaie dicPaie,
                                               out DicPerf dicPerf, out DicTran dicTran)
        {
            // Load from Binary files
            dicFact = uBin.Deserialize<DicFact>("dicFact.bin");
            dicPaie = uBin.Deserialize<DicPaie>("dicPaie.bin");
            dicPerf = uBin.Deserialize<DicPerf>("dicPerf.bin");
            dicTran = uBin.Deserialize<DicTran>("dicTran.bin");
        }

        public static void GetDataFromCsvFiles(out DicFact dicFact, out DicPaie dicPaie,
                                               out DicPerf dicPerf, out DicTran dicTran, bool trainNotTest, bool useFull)
        {
            FileFacturation fileFact = null; FilePaiements    filePaie = null;
            FilePerformance filePerf = null; FileTransactions fileTran = null;

            if (trainNotTest) LoadTrainSets(out fileFact, out filePaie, out filePerf, out fileTran);
            else              LoadTestSets (out fileFact, out filePaie, out filePerf, out fileTran);

            // Temporarily remove clients not in transactions file
            var listUniqueClients = new List<string>();
            filePerf.rows.ForEach(e => listUniqueClients.Add(e.ID_CPTE));
            listUniqueClients = listUniqueClients.Distinct().ToList();
            if (!useFull)
                listUniqueClients = listUniqueClients.GetRange(0,100); // DebugOnly: get only a few clients to speed testing up
            fileFact.rows = fileFact.rows.FindAll(a => listUniqueClients.Contains(a.ID_CPTE));
            filePaie.rows = filePaie.rows.FindAll(a => listUniqueClients.Contains(a.ID_CPTE));
            filePerf.rows = filePerf.rows.FindAll(a => listUniqueClients.Contains(a.ID_CPTE));
            fileTran.rows = fileTran.rows.FindAll(a => listUniqueClients.Contains(a.ID_CPTE));

            // Group dataset by client
            GroupDataByClient(fileFact, filePaie, filePerf, fileTran, out dicFact, out dicPaie, out dicPerf, out dicTran);


            // Remove all data after the requested prediction date
            var perfKeys = dicPerf.Keys.ToList();
            foreach (var k in perfKeys)
            {
                if (!dicFact.ContainsKey(k)) continue;
                var date = dicPerf[k].PERIODID_MY;
                dicFact[k].RemoveAll(e => date <= e.StatementDate);
            }
            foreach (var k in perfKeys)
            {
                if (!dicPaie.ContainsKey(k)) continue;
                var date = dicPerf[k].PERIODID_MY;
                dicPaie[k].RemoveAll(e => date <= e.TRANSACTION_DTTM);
            }
            foreach (var k in perfKeys)
            {
                if (!dicTran.ContainsKey(k)) continue;
                var date = dicPerf[k].PERIODID_MY;
                dicTran[k].RemoveAll(e => date <= e.TRANSACTION_DTTM);
            }

            // Save to Binary files
            bool save = false;
            if (save)
            {
                uBin.Serialize("dicFact.bin", dicFact);
                uBin.Serialize("dicPaie.bin", dicPaie);
                uBin.Serialize("dicPerf.bin", dicPerf);
                uBin.Serialize("dicTran.bin", dicTran);
            }
        }

        /// <summary>
        /// Get contents of CSV files sorted by descending dates. Note that the keys themselves are unsorted.
        /// </summary>
        private static void GroupDataByClient(FileFacturation fileFact, FilePaiements filePaie,
                                              FilePerformance filePerf, FileTransactions fileTran,
                                    out DicFact dicFact, out DicPaie dicPaie, out DicPerf dicPerf, out DicTran dicTran)
        {
            dicFact = new DicFact();
            foreach (var row in fileFact.rows)
            {
                if (!dicFact.ContainsKey(row.ID_CPTE)) dicFact.Add(row.ID_CPTE, new List<DataFacturation>());
                dicFact[row.ID_CPTE].Add(row);
            }
            dicFact = dicFact.ToDictionary(d => d.Key, d => Utils.SortMostRecentFirst(d.Value));
            
            dicPaie = new DicPaie();
            foreach (var row in filePaie.rows)
            {
                if (!dicPaie.ContainsKey(row.ID_CPTE)) dicPaie.Add(row.ID_CPTE, new List<DataPaiements>());
                dicPaie[row.ID_CPTE].Add(row);
            }
            dicPaie = dicPaie.ToDictionary(d => d.Key, d => Utils.SortMostRecentFirst(d.Value));

            dicPerf = new DicPerf();
            foreach (var row in filePerf.rows)
                if (!dicPerf.ContainsKey(row.ID_CPTE)) dicPerf.Add(row.ID_CPTE, row);
            // no need to sort, because there's only one element, not a list
            

            dicTran = new DicTran();
            foreach (var row in fileTran.rows)
            {
                if (!dicTran.ContainsKey(row.ID_CPTE)) dicTran.Add(row.ID_CPTE, new List<DataTransactions>());
                dicTran[row.ID_CPTE].Add(row);
            }
            dicTran = dicTran.ToDictionary(d => d.Key, d => Utils.SortMostRecentFirst(d.Value));
        }
    }
}
