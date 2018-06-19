using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiYahoo
{
    class MYControl
    {
        public static string MainDirectory = (Environment.UserName.ToUpper() == "CHAN" ? @"C:\Users\chan\Documents\dfkjdf\" : @"C:\Users\Samuel\Documents\dfkjdf\");

        public static string ApplicationAddress = MainDirectory + @"1. Projects\YahooHistoricalPrice\YahooHistoricalPrice\bin\Debug\YahooHistoricalPrice.exe";
    }
}
