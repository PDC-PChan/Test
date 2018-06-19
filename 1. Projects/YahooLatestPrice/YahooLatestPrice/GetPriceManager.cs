using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace YahooLatestPrice
{
    public class GetPriceManager
    {
        public static void AppendToPriceHistory(string symbol,double price)
        {
            string PriceFileAdd = YControl.MainDirectory + symbol + @"\Prices.csv";
            using (StreamWriter sw = new StreamWriter(PriceFileAdd, append: true))
            {
                sw.Write(string.Format("{0},,,,{1}", DateTime.Today.ToString("yyyy-MM-dd"), price));
            }
        }

        public static double GetPrice(string symbol)
        {
            string HTMLAddress = YControl.yahooAddress + symbol;
            string htmlCode = GetHTMLCode(HTMLAddress);

            string track1 = "<!-- react-text: 36 -->";
            string track2 = "<";
            int index1 = 0;
            int index2 = 0;
            string resultString;

            for (int i = 0; i < 2; i++)
            {
                index1 = htmlCode.IndexOf(track1,index1 + track1.Length);
            }
            index2 = htmlCode.IndexOf(track2, index1 + track1.Length);

            resultString = htmlCode.Substring(index1+track1.Length, index2 - index1 - track1.Length);

            return (Convert.ToDouble(resultString));
        }

        

        private static string GetHTMLCode(string urlAddress)
        {
            string data = "";
            bool TryAgain = true;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

            while (TryAgain)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;

                        if (response.CharacterSet == null)
                        {
                            readStream = new StreamReader(receiveStream);
                        }
                        else
                        {
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        }

                        data = readStream.ReadToEnd();

                        response.Close();
                        readStream.Close();
                    }
                    TryAgain = false;
                }
                catch (System.Net.WebException)
                {}
            }
            
            return data;
        }
    }
}
