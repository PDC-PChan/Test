using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace YahooLatestPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            string symbolListAdd = YControl.MainDirectory + YControl.symbolList;
            Dictionary<string, double> symbolList = new Dictionary<string, double>();

            // Read in symbols
            using (StreamReader sr = new StreamReader(symbolListAdd))
            {
                while (!sr.EndOfStream)
                {
                    symbolList.Add(sr.ReadLine(),0);
                }
            }

            // Get Price
            foreach (string symbol in symbolList.Keys)
            {
                symbolList[symbol] = GetPriceManager.GetPrice(symbol);
                Console.WriteLine("{0} is done.", symbol);
            }

            // Append Price
            foreach (string symbol in symbolList.Keys)
            {
                GetPriceManager.AppendToPriceHistory(symbol, symbolList[symbol]);
                Console.WriteLine("{0} appended.", symbol);
            }
        }
    }
}
