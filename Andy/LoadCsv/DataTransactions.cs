using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Csv;

namespace LoadCsv
{
    public class FileTransactions
    {
        // Properties
        public string              path = string.Empty;
        public List<DataTransactions> rows = null;

        // Constructors
        private FileTransactions(string _path) => path = _path;
        public static FileTransactions LoadCsvFile(string _path)
        {
            var file = new FileTransactions(_path);
            file.LoadCsv();
            return file;
        }

        // Methods
        private async Task LoadCsv()
        {
            rows = uCsv.ReadFromCsv<DataTransactions>(path, delimiter: ",");
        }

        public List<DataTransactions> GetClientRows(string clientId)
        {
            var clientRows = new List<DataTransactions>();
            if (string.IsNullOrWhiteSpace(clientId)) return clientRows;
            clientRows = rows.FindAll(r => r.ID_CPTE == clientId);
            clientRows = clientRows.OrderByDescending(a => a.TRANSACTION_DTTM).ToList();
            return clientRows;
        }
    }
    [Serializable]
    public class DataTransactions
    {//ID_CPTE,MERCHANT_CATEGORY_XCD,MERCHANT_CITY_NAME,MERCHANT_COUNTRY_XCD,DECISION_XCD,PRIOR_CREDIT_LIMIT_AMT,
     //TRANSACTION_AMT,TRANSACTION_CATEGORY_XCD,TRANSACTION_DTTM,TRANSACTION_TYPE_XCD,SICGROUP
        public string ID_CPTE                  { get; set; }
        public string MERCHANT_CATEGORY_XCD    { get; set; }
        public int    MERCHANT_CITY_NAME       { get; set; }
        public string MERCHANT_COUNTRY_XCD     { get; set; }
        public string DECISION_XCD             { get; set; }
        public double PRIOR_CREDIT_LIMIT_AMT   { get; set; }
        public double TRANSACTION_AMT          { get; set; }
        public string TRANSACTION_CATEGORY_XCD { get; set; }
        public DateTime TRANSACTION_DTTM         { get; set; }
        public string TRANSACTION_TYPE_XCD     { get; set; }
        public string SICGROUP                 { get; set; }

        public DataTransactions() { }
        public bool IsAMatch(DataTransactions r1)
        {
            if (string.IsNullOrEmpty(r1.ID_CPTE))  return false;
            return r1.ID_CPTE == r1.ID_CPTE;
        }
        public override string ToString()
        {
            string uh = $"{ID_CPTE}: ${TRANSACTION_AMT}, Cat{TRANSACTION_CATEGORY_XCD}, {TRANSACTION_DTTM}";
            return uh;
        }
    }
}
