using System;
using System.Collections.Generic;
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
    public static class Utils
    {
        public static double GetNbOfDelinquencies(List<DataFacturation> rowsFact)
        {
            if (uNet.IsNullOrEmpty(rowsFact)) return 0d;
            double d = 0;
            rowsFact.ForEach(e =>
            {
                d += e.DelqCycle;
            });
            return d;
        }

        public static void LoadTrainSets(out FileFacturation fileFactTrain, out FilePaiements filePaieTrain,
                                         out FilePerformance filePerfTrain, out FileTransactions fileTranTrain)
        {
            fileFactTrain = FileFacturation.LoadCsvFile(@"Data/facturation_train.csv");
            filePaieTrain = FilePaiements.LoadCsvFile(@"Data/paiements_train.csv");
            filePerfTrain = FilePerformance.LoadCsvFile(@"Data/performance_train.csv");
            fileTranTrain = FileTransactions.LoadCsvFile(@"Data/transactions_train.csv");
        }

        public static void LoadTestSets(out FileFacturation fileFactTest, out FilePaiements filePaieTest,
                                        out FilePerformance filePerfTest, out FileTransactions fileTranTest)
        {
            fileFactTest = FileFacturation.LoadCsvFile(@"Data/facturation_test.csv");
            filePaieTest = FilePaiements.LoadCsvFile(@"Data/paiements_test.csv");
            filePerfTest = FilePerformance.LoadCsvFile(@"Data/performance_test.csv");
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
                                               out DicPerf dicPerf, out DicTran dicTran, bool trueTrainFalseTest, bool useFullDontSave)
        {
            FileFacturation fileFact = null; FilePaiements    filePaie = null;
            FilePerformance filePerf = null; FileTransactions fileTran = null;

            if (trueTrainFalseTest) LoadTrainSets(out fileFact, out filePaie, out filePerf, out fileTran);
            else                    LoadTestSets (out fileFact, out filePaie, out filePerf, out fileTran);

            // Temporarily remove clients not in transactions file
            var listUniqueClients = new List<string>();
            fileTran.rows.ForEach(e => listUniqueClients.Add(e.ID_CPTE));
            listUniqueClients = listUniqueClients.Distinct().ToList();
            if (!useFullDontSave)
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
            if (!useFullDontSave)
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
            dicFact = dicFact.ToDictionary(d => d.Key, d => (List<DataFacturation>)d.Value.OrderByDescending(v => v.StatementDate).ToList());
            
            dicPaie = new DicPaie();
            foreach (var row in filePaie.rows)
            {
                if (!dicPaie.ContainsKey(row.ID_CPTE)) dicPaie.Add(row.ID_CPTE, new List<DataPaiements>());
                dicPaie[row.ID_CPTE].Add(row);
            }
            dicPaie = dicPaie.ToDictionary(d => d.Key, d => (List<DataPaiements>)d.Value.OrderByDescending(v => v.TRANSACTION_DTTM).ToList());

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
            dicTran = dicTran.ToDictionary(d => d.Key, d => (List<DataTransactions>)d.Value.OrderByDescending(v => v.TRANSACTION_DTTM).ToList());
        }
    }
}
