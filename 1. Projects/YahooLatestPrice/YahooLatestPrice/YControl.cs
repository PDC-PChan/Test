using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahooLatestPrice
{
    class YControl
    {
        public static string MainDirectory = Environment.UserName.ToUpper()=="CHAN"? @"C:\Users\chan\Documents\dfkjdf\" : @"C:\Users\Samuel\Documents\dfkjdf\";

        public static string symbolList = @"0.VolAnalysis\SymbolList.txt";

        public static string yahooAddress = "https://uk.finance.yahoo.com/quote/";
    }
}
