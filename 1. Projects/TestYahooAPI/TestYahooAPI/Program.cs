using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using YahooFinanceAPI;
using YahooFinanceAPI.Models;

namespace TestYahooAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Downloader tc = new Downloader();
            Console.WriteLine("Download Start...");
            tc.LoopDownload();
            while (!tc.IsComplete)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Complete");
        }
    }

    public class Downloader
    {
        public bool IsComplete = false;
        private string mainDirectory = (Environment.UserName.ToUpper() == "CHAN" ? @"C:\Users\chan\Documents\dfkjdf\" : @"C:\Users\Samuel\Documents\dfkjdf v2\Test\");
        private string symbolListAdd = @"0.VolAnalysis\SymbolList.txt";
        private List<string> symbolList = new List<string>();

        public Downloader()
        {
            using (StreamReader sr  = new StreamReader(mainDirectory + symbolListAdd))
            {
                while (!sr.EndOfStream)
                {
                    symbolList.Add(sr.ReadLine());
                }
            }
        }

        public async void LoopDownload()
        {
            foreach (string symbol in symbolList)
            {
                Console.WriteLine("Downloading {0}", symbol);
                await WriteHistoricalPrice(symbol);
            }
            IsComplete = true;
        }

        private async Task WriteHistoricalPrice(string symbol)
        {
            List<HistoryPrice> hps;

            //first get a valid token from Yahoo Finance
            while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
            {
                await Token.RefreshAsync().ConfigureAwait(false);
            }

            hps = await Historical.GetPriceAsync(symbol, new DateTime(2010,1,1), DateTime.Now).ConfigureAwait(false);

            //Write to csv
            using (StreamWriter sw = new StreamWriter(mainDirectory + symbol + @"\Prices.csv",false))
            {
                sw.WriteLine("Date,Open,High,Low,Close,Adj Close,Volume");
                foreach (HistoryPrice hp in hps)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                        hp.Date.ToString("yyyy-MM-dd"),
                                        hp.Open,hp.High,hp.Low,hp.Close,hp.AdjClose,hp.Volume));
                }
            }
        }


    }
}
