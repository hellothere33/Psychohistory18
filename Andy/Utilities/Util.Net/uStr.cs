using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Util.Net
{
    public static class uStr
    {
        public static string ToAlphaNumericOnly(this string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, "");
        }

        public static string ToAlphaOnly(this string input)
        {
            Regex rgx = new Regex("[^a-zA-Z]");
            return rgx.Replace(input, "");
        }

        public static string ToNumericOnly(this string input)
        {
            Regex rgx = new Regex("[^0-9]");
            return rgx.Replace(input, "");
        }
    }
}
