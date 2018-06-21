using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;

namespace YahooHistoricalPrice
{
    class Program
    {

        static void Main(string[] args)
        {
            int split = Convert.ToInt16(args[0]);
            int ProcessCode = Convert.ToInt16(args[1]);

            NeedToMakeThisForSomeReason(split, ProcessCode);
        }

        static void NeedToMakeThisForSomeReason(int split = 1, int ProcessCode = 0)
        {
            string MainDirectory = (Environment.UserName.ToUpper() == "CHAN" ? @"C:\Users\chan\Documents\dfkjdf\" : @"C:\Users\Samuel\Documents\dfkjdf v2\Test\");
            string symbolListAdd = MainDirectory + @"0.VolAnalysis\SymbolList.txt";
            List<string> symbolList = new List<string>();
            bool SomeWrong = false;
            int startIndex;
            int endIndex;
            int cnt = 0;

            //#### Read in list of symbols #######
            using (StreamReader sr = new StreamReader(symbolListAdd))
            {
                while (!sr.EndOfStream)
                {
                    symbolList.Add(sr.ReadLine());
                }
            }


            // #### Refine Symbol List
            startIndex = Convert.ToInt32(ProcessCode * Math.Ceiling((double)symbolList.Count / split));
            endIndex = Convert.ToInt32(Math.Min((ProcessCode + 1) * Math.Ceiling((double)symbolList.Count / split) - 1,
                                        symbolList.Count - 1));
            symbolList = symbolList.GetRange(startIndex, endIndex - startIndex + 1);


            //#### Create tasks #######
            foreach (string symbol in symbolList)
            {
                DownloadPageAsync(symbol, new DateTime(2010, 1, 1), DateTime.Today, MainDirectory + symbol + @"\Prices.csv");
                Console.WriteLine("{0} of {1} Done - {2}", ++cnt, symbolList.Count, symbol);
                Thread.Sleep(10000);
            }


            //#### Checking and rerun #######
            do
            {
                SomeWrong = false;

                foreach (string symbol in symbolList)
                {
                    try
                    {
                        StreamReader sr = new StreamReader(MainDirectory + symbol + @"\Prices.csv");
                        if (sr.ReadToEnd().IndexOf("{") != -1)
                        {
                            sr.Close();
                            DownloadPageAsync(symbol, new DateTime(2010, 1, 1), DateTime.Today, MainDirectory + symbol + @"\Prices.csv");
                            SomeWrong = true;
                            Thread.Sleep(10000);
                        }
                        else
                        {
                            sr.Close();
                        }
                    }
                    catch (Exception)
                    {
                        SomeWrong = true;
                    }

                }

            } while (SomeWrong);

        }

        static async void DownloadPageAsync(string symbol, DateTime DateStart, DateTime DateEnd, string filePath)
        {
            // ... Target page.
            string page = string.Format("https://finance.yahoo.com/quote/{0}/history?p={0}", symbol);
            string downloadLink = string.Format("https://query1.finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history&crumb=",
                symbol,
                (DateStart - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds,
                (DateEnd - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);

            string crumb = "";
            string crumbSearchString = "{\"crumb\":\"";
            int crumbStartIndex = 0;
            int crumbEndIndex;
            string line = "";

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();


                // Analyze html code to get crumb
                do
                {
                    crumbStartIndex = result.IndexOf(crumbSearchString, crumbStartIndex) + crumbSearchString.Length;
                } while (result.Substring(crumbStartIndex, 1) == "{");
                crumbEndIndex = result.IndexOf("\"", crumbStartIndex);
                crumb = result.Substring(crumbStartIndex, crumbEndIndex - crumbStartIndex);

                HttpResponseMessage response2 = await client.GetAsync(downloadLink + crumb);
                HttpContent content2 = response2.Content;
                //Console.WriteLine(content2.ReadAsStringAsync());
                using (StreamReader sr = new StreamReader(await content2.ReadAsStreamAsync()))
                {
                    line = sr.ReadLine();
                    using (StreamWriter sw = new StreamWriter(filePath, false))
                    {
                        sw.Write("");
                    }
                    while (line != null)
                    {
                            using (StreamWriter sw = new StreamWriter(filePath, true))
                            {
                                sw.WriteLine(line);
                            }
                        
                        line = sr.ReadLine();
                    }
                }

            }
        }
    }
}
