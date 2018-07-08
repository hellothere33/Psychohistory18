using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Csv;
using Util.Net;

namespace LoadCsv
{
    public class FileFacturation
    {
        // Properties
        public string                path = string.Empty;
        public List<DataFacturation> rows = null;

        // Constructors
        private FileFacturation(string _path) => path = _path;
        public static FileFacturation LoadCsvFile(string _path)
        {
            var file = new FileFacturation(_path);
            file.LoadCsv();
            return file;
        }

        // Methods
        private void LoadCsv()
        {
            rows = uCsv.ReadFromCsv<DataFacturation>(path, delimiter: ",");
        }
    }
    public class DataFacturation
    {//ID_CPTE,PERIODID_MY,StatementDate,CurrentTotalBalance,CashBalance,CreditLimit,DelqCycle
        public string ID_CPTE             { get; set; }
        public string PERIODID_MY         { get; set; }
        public string StatementDate       { get; set; }
        public string CurrentTotalBalance { get; set; }
        public string CashBalance         { get; set; }
        public string CreditLimit         { get; set; }
        public string DelqCycle           { get; set; }

        public DataFacturation() {
            ID_CPTE              = "";
            PERIODID_MY          = "";
            StatementDate        = "";
            CurrentTotalBalance  = "";
            CashBalance          = "";
            CreditLimit          = "";
            DelqCycle            = "";
        }
        public bool IsAMatch(DataFacturation r1)
        {
            if (string.IsNullOrEmpty(r1.ID_CPTE))  return false;
            return r1.ID_CPTE == r1.ID_CPTE;
        }
        public override string ToString()
        {
            string uh = $"{ID_CPTE}: {PERIODID_MY}, ${CurrentTotalBalance}, ${CashBalance}, ${CreditLimit}, Delq{DelqCycle}";
            return uh;
        }
    }
}
