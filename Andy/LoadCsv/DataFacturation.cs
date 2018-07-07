using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Net;

namespace LoadCsv
{
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
            string uh = $"{ID_CPTE}: {PERIODID_MY}, {CurrentTotalBalance}, {CashBalance}, {CreditLimit}, {DelqCycle}";
            return uh;
        }
    }
}
