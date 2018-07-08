using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Csv;

namespace LoadCsv
{
    public class FilePaiements
    {
        // Properties
        public string              path = string.Empty;
        public List<DataPaiements> rows = null;

        // Constructors
        private FilePaiements(string _path) => path = _path;
        public static FilePaiements LoadCsvFile(string _path)
        {
            var file = new FilePaiements(_path);
            file.LoadCsv();
            return file;
        }

        // Methods
        private void LoadCsv()
        {
            rows = uCsv.ReadFromCsv<DataPaiements>(path, delimiter: ",");
        }
    }
    public class DataPaiements
    {//ID_CPTE,TRANSACTION_AMT,TRANSACTION_DTTM,PAYMENT_REVERSAL_XFLG
        public string ID_CPTE               { get; set; }
        public string TRANSACTION_AMT       { get; set; }
        public string TRANSACTION_DTTM      { get; set; }
        public string PAYMENT_REVERSAL_XFLG { get; set; }

        public DataPaiements() {
            ID_CPTE               = "";
            TRANSACTION_AMT       = "";
            TRANSACTION_DTTM      = "";
            PAYMENT_REVERSAL_XFLG = "";
        }
        public bool IsAMatch(DataPaiements r1)
        {
            if (string.IsNullOrEmpty(r1.ID_CPTE))  return false;
            return r1.ID_CPTE == r1.ID_CPTE;
        }
        public override string ToString()
        {
            string uh = $"{ID_CPTE}: ${TRANSACTION_AMT}, {TRANSACTION_DTTM}, {PAYMENT_REVERSAL_XFLG}";
            return uh;
        }
    }
}
