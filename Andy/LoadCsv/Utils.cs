using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Util.BinarySerializer;
using Util.Net;
using sMath = System.Math;
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
            nbs[0] = stat.Min; nbs[1] = stat.Avg; nbs[2] = stat.Max; nbs[3] = stat.Std; //nbs[4] = stat.Var;
        }
        private static double[] NormalizeToOne(double[] nbs)
        {
            if (norm) { double max = nbs.Max(); max = max == 0 ? 1 : max; nbs = nbs.Select(i => i / max).ToArray(); } // normalize to be under 1.0
            return nbs;
        }
        private static double[] NormalizeToOne(double[] nbs, double min, double max)
        {
            if (norm)
            {
                nbs = nbs.Select(d =>
                {
                    if (d < min) return 0;
                    if (max < d) return 0;
                    double v = (d - min) / (max - min);
                    return v;
                }).ToArray();
            } // normalize to be under 1.0
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

        private static void Init_Categories(List<DataTransactions> list, DateTime predictionStartDate, int nbDaysMostRecent, 
                                            List<string> categories, out double[] nbs, out List<DataTransactions> shortList)
        {
            nbs            = NnRow.CreateArray(categories.Count);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            Start          = End - TimeSpan.FromDays(nbDaysMostRecent);
            shortList      = list.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();
        }
        
        public static double[] GetTransactionsCategories_SICGROUP(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> {"AT","AX","AA","BA","AJ","AV","AW","AN","AO","AE","AH","AQ","AL","AG","AB","AD","AY","AM","BB","BC","AP","AI","AC","AU","AZ","AR","AF","AS","AK"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.SICGROUP == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
        }
        public static double[] GetTransactionsCategories_TRANSACTION_TYPE_XCD(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> {"F","B","G","E","D","C","A"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.TRANSACTION_TYPE_XCD == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
        }
        public static double[] GetTransactionsCategories_TRANSACTION_CATEGORY_XCD(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> {"E","B","C","A","D"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.TRANSACTION_CATEGORY_XCD == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
        }
        public static double[] GetTransactionsCategories_DECISION_XCD(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> {"C","A","B"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.DECISION_XCD == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
        }
        public static double[] GetTransactionsCategories_MERCHANT_COUNTRY_XCD(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> {"DP","BB","DA","AF","BT","BW","CD","AS","CE","EI","BJ","DU","BX","BZ","AJ","ED","DQ","EF","CK","DO","DI","AN","BD","CW"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.MERCHANT_COUNTRY_XCD == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
        }
        public static double[] GetTransactionsCategories_MERCHANT_CITY_NAME(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> {"2122999","735207","2734290","2271380","179603","23849","2871582","452039","596711","1461886","680536","2720203","1533431","1389367","1652194","365767","414343","1523946","2328808","2859894","597796","484852"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.MERCHANT_CITY_NAME == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
        }
        public static double[] GetTransactionsCategories_MERCHANT_CATEGORY_XCD(List<DataTransactions> list, DateTime predictionStartDate, int nbCategories, int nbDaysMostRecent)
        {
            var categories = new List<string> { "KK","FF","EE","JJ","YZ","V","QQ","AD","OO","HH","DD","PP","YY","J","VV","BC","CC","W","UU","AG"};
            categories = categories.GetRange(0, nbCategories);
            Init_Categories(list, predictionStartDate, nbDaysMostRecent, categories, out double[] nbs, out List<DataTransactions> shortList);
            for (int i = 0; i < categories.Count; i++)
                nbs[i] = shortList.Count(e => e.MERCHANT_CATEGORY_XCD == categories[i]);
            nbs = NormalizeToOne(nbs, 0, list.Count);
            return nbs;
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
        public static double[] GetPaiements(int len, DateTime predictionStartDate, List<DataPaiements> list, int days)
        {
            double[] nbs = NnRow.CreateArray(len);
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
        public static double[] GetPaiementsFreqStats(int nbPeriods, DateTime predictionStartDate, List<DataPaiements> list, int days)
        {
            int nbStats = 4;
            var allStats = new double[nbPeriods * nbStats];
            if (uNet.IsNullOrEmpty(list)) return allStats;

            var tempList = new List<DataPaiements>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbPeriods && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();

                // Calculate frequencies
                var durations = new List<double>();
                double[] stats = new double[nbStats];
                if (3 <= shortList.Count)
                {
                    for (int j = 0; j < shortList.Count - 1; j++)
                    {
                        TimeSpan duration = shortList[j].TRANSACTION_DTTM - shortList[j + 1].TRANSACTION_DTTM;
                        durations.Add(duration.TotalHours);
                    }
                    CalculateStatsAndCopyToArray(stats, durations);
                    stats = NormalizeToOne(stats, 0, days*24);
                }
                allStats[i*4    ] = stats[0];
                allStats[i*4 + 1] = stats[1];
                allStats[i*4 + 2] = stats[2];
                allStats[i*4 + 3] = stats[3];
            }
            
            return allStats;
        }
        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetTransactionsFreqStats(int nbPeriods, DateTime predictionStartDate, List<DataTransactions> list, int days)
        {
            int nbStats = 4;
            var allStats = new double[nbPeriods * nbStats];
            if (uNet.IsNullOrEmpty(list)) return allStats;

            var tempList = new List<DataTransactions>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbPeriods && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();

                // Calculate frequencies
                var durations = new List<double>();
                double[] stats = new double[nbStats];
                if (3 <= shortList.Count)
                {
                    for (int j = 0; j < shortList.Count - 1; j++)
                    {
                        TimeSpan duration = shortList[j].TRANSACTION_DTTM - shortList[j + 1].TRANSACTION_DTTM;
                        durations.Add(duration.TotalHours);
                    }
                    CalculateStatsAndCopyToArray(stats, durations);
                    stats = NormalizeToOne(stats, 0, days*24);
                }
                allStats[i*4    ] = stats[0];
                allStats[i*4 + 1] = stats[1];
                allStats[i*4 + 2] = stats[2];
                allStats[i*4 + 3] = stats[3];
            }
            
            return allStats;
        }


        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetSpendsAndPaymentsAfterStatement(int len, DateTime predictionStartDate, List<DataFacturation> rowsFact,
                                                                  List<DataPaiements> rowsPaie, List<DataTransactions> rowsTran, int days)
        {
            var allStats = new List<double>();
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < len; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortTran = rowsTran.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();
                if (shortTran.Count <= 0) continue;
                shortTran = shortTran.OrderByDescending(e => e.TRANSACTION_DTTM).ToList();

                var shortPaie = null == rowsPaie ? new List<DataPaiements>() :
                                rowsPaie.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();
                if (shortPaie.Count <= 0) continue;
                shortPaie = shortPaie.OrderByDescending(e => e.TRANSACTION_DTTM).ToList();

                var shortFact = null == rowsFact ? new List<DataFacturation>() :
                                rowsFact.FindAll(e => (Start - TimeSpan.FromDays(days + 90)) < e.PERIODID_MY && e.PERIODID_MY <= Start).ToList();
                shortFact = shortFact.OrderByDescending(e => e.PERIODID_MY).ToList();
                double lastBalance = shortFact.Count<=0? 0: shortFact[0].CurrentTotalBalance;


                List<(DateTime date, double val)> chronological = new List<(DateTime date, double val)>();
                //chronological.Add((new DateTime(1000, 1, 1), lastBalance));
                foreach (var e in shortTran) chronological.Add((e.TRANSACTION_DTTM,  e.TRANSACTION_AMT));
                foreach (var e in shortPaie) chronological.Add((e.TRANSACTION_DTTM, -e.TRANSACTION_AMT));
                chronological = chronological.OrderBy(e => e.date).ToList(); // oldest first

                var vals = new List<double>();
                foreach (var e in chronological)
                {
                    lastBalance += e.val;
                    vals.Add(lastBalance);
                }


                double[] stats = NnRow.CreateArray(4);
                CalculateStatsAndCopyToArray(stats, vals);
                stats = NormalizeToOne(stats, -60000, 60000);
                allStats.AddRange(stats);
            }

            // !!! here we want a neutral balance to be 0.5, so we can have both +- values in the 0.0 ~ 1.0 output range
            double neutral = 0.5;
            double[] nbs = NnRow.CreateArray(len*4, neutral);
            for (int i = 0; i < nbs.Length && i < allStats.Count; i++)
                nbs[i] = allStats[i];
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCashBalance(int len, DateTime predictionStartDate, List<DataFacturation> list, int days)
        {
            double[] nbs = NnRow.CreateArray(len);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                if (shortList.Count <= 0) continue;
                double val = shortList.Max(e => e.CashBalance);
                nbs[i] = val;
            }
            nbs = NormalizeToOne(nbs);
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCurrentTotalBalance(int len, DateTime predictionStartDate, List<DataFacturation> list, int days)
        {
            double[] nbs = NnRow.CreateArray(len);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                if (shortList.Count <= 0) continue;
                double val = shortList.Max(e => e.CurrentTotalBalance);
                nbs[i] = val;
            }
            nbs = NormalizeToOne(nbs);
            return nbs;
        }
        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCreditLimit(int len, DateTime predictionStartDate, List<DataFacturation> list, int days,
                                double min, double max)
        {
            double[] nbs = NnRow.CreateArray(len);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                if (shortList.Count <= 0) continue;
                double val = shortList.Max(e => e.CreditLimit);
                nbs[i] = val;
            }
            nbs = NormalizeToOne(nbs, min, max);
            return nbs;
        }
        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCurrentTotalBalance(int len, DateTime predictionStartDate, List<DataFacturation> list, int days,
                                double min, double max)
        {
            double[] nbs = NnRow.CreateArray(len);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                if (shortList.Count <= 0) continue;
                double val = shortList.Max(e => e.CurrentTotalBalance);
                nbs[i] = val;
            }
            nbs = NormalizeToOne(nbs, min, max);
            return nbs;
        }
        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetFacturationCashBalance(int len, DateTime predictionStartDate, List<DataFacturation> list, int days,
                                double min, double max)
        {
            double[] nbs = NnRow.CreateArray(len);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataFacturation>(list);
            DateTime Start = predictionStartDate;
            DateTime End = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.PERIODID_MY && e.PERIODID_MY <= End).ToList();
                if (shortList.Count <= 0) continue;
                double val = shortList.Max(e => e.CashBalance);
                nbs[i] = val;
            }
            nbs = NormalizeToOne(nbs, min, max);
            return nbs;
        }

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetTransactions(int len, DateTime predictionStartDate, List<DataTransactions> list, int days)
        {
            double[] nbs = NnRow.CreateArray(len);
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

        /// <summary>
        /// Assumption: input data already sorted by most recent first.
        /// </summary>
        public static double[] GetTransactions_Max_PRIOR_CREDIT_LIMIT_AMT(int len, DateTime predictionStartDate, List<DataTransactions> list, int days)
        {
            double[] nbs = NnRow.CreateArray(len);
            if (uNet.IsNullOrEmpty(list)) return nbs;

            var tempList = new List<DataTransactions>(list);
            DateTime Start = predictionStartDate;
            DateTime End   = predictionStartDate;
            for (int i = 0; i < nbs.Length && 0 < tempList.Count; i++)
            {
                End   = Start;
                Start = End - TimeSpan.FromDays(days);
                var shortList = tempList.FindAll(e => Start < e.TRANSACTION_DTTM && e.TRANSACTION_DTTM <= End).ToList();
                if (shortList.Count <= 0) continue;
                double max = shortList.Max(e => e.PRIOR_CREDIT_LIMIT_AMT);
                nbs[i] = max;
            }
            nbs = NormalizeToOne(nbs, 0, 39709); // -48~+18044 for A, 0~+5427 for B, -5709~+39709 for C,
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
