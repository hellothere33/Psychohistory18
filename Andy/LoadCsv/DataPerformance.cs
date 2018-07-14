using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Csv;

namespace LoadCsv
{
    public class FilePerformance
    {
        // Properties
        public string                path = string.Empty;
        public List<DataPerformance> rows = null;

        // Constructors
        private FilePerformance(string _path) => path = _path;
        public static FilePerformance LoadCsvFile(string _path)
        {
            var file = new FilePerformance(_path);
            file.LoadCsv();
            return file;
        }

        // Methods
        private void LoadCsv()
        {
            rows = uCsv.ReadFromCsv<DataPerformance>(path, delimiter: ",");
        }

        public List<DataPerformance> GetClientRows(string clientId)
        {
            var clientRows = new List<DataPerformance>();
            if (string.IsNullOrWhiteSpace(clientId)) return clientRows;
            clientRows = rows.FindAll(r => r.ID_CPTE == clientId);
            //clientRows = Utils.SortMostRecentFirst(clientRows); // we DON'T want to re-order the performance table
            return clientRows;
        }
    }
    [Serializable]
    public class DataPerformance
    {//ID_CPTE,PERIODID_MY,Default
        public string   ID_CPTE     { get; set; }
        public DateTime PERIODID_MY { get; set; }
        public int?     Default     { get; set; }

        public DataPerformance() { }
        public bool IsAMatch(DataPerformance r1)
        {
            if (string.IsNullOrEmpty(r1.ID_CPTE))  return false;
            return r1.ID_CPTE == r1.ID_CPTE;
        }
        public override string ToString()
        {
            string uh = $"{ID_CPTE}: {PERIODID_MY}, Def{Default}";
            return uh;
        }
    }
}
