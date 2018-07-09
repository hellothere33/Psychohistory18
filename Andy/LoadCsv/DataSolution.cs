using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Csv;

namespace LoadCsv
{
    public class FileSolution
    {
        // Properties
        public string path = string.Empty;
        public List<DataSolution> rows = null;

        // Constructors
        private FileSolution(string _path) => path = _path;
        public static FileSolution LoadCsvFile(string _path)
        {
            var file = new FileSolution(_path);
            file.LoadCsv();
            return file;
        }

        // Methods
        private async Task LoadCsv()
        {
            rows = uCsv.ReadFromCsv<DataSolution>(path, delimiter: ",");
        }

        public List<DataSolution> GetClientRows(string clientId)
        {
            var clientRows = new List<DataSolution>();
            if (string.IsNullOrWhiteSpace(clientId)) return clientRows;
            clientRows = rows.FindAll(r => r.ID_CPTE == clientId);
            return clientRows;
        }
    }
    public class DataSolution
    {//ID_CPTE,Default
        public string ID_CPTE { get; set; }
        public int    Default { get; set; }

        public DataSolution(){ }
        public bool IsAMatch(DataSolution r1)
        {
            if (string.IsNullOrEmpty(r1.ID_CPTE)) return false;
            return r1.ID_CPTE == r1.ID_CPTE;
        }
        public override string ToString()
        {
            string uh = $"{ID_CPTE}: Def{Default}";
            return uh;
        }
    }
}
