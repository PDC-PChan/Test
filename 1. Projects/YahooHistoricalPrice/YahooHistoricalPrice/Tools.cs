using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace YahooHistoricalPrice
{
    class Tools
    {
        public static void GetHTMLCode(string urlAddress)
        {
            string data = "";
            string crumb = "";
            string crumbSearchString = "{\"crumb\":\"";
            int  crumbStartIndex= 0;
            int crumbEndIndex ;

            // Read in html code

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
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


            // Analyze html code to get crumb
            do
            {
                crumbStartIndex = data.IndexOf(crumbSearchString, crumbStartIndex) + crumbSearchString.Length;
            } while (data.Substring(crumbStartIndex,1) == "{");
            crumbEndIndex = data.IndexOf("\"", crumbStartIndex);
            crumb = data.Substring(crumbStartIndex, crumbEndIndex - crumbStartIndex);


            urlAddress = "https://query1.finance.yahoo.com/v7/finance/download/TIF?period1=1493583717&period2=1496175717&interval=1d&events=history&crumb=" + crumb;
            request = (HttpWebRequest)WebRequest.Create(urlAddress);
            response = (HttpWebResponse)request.GetResponse();
        }
    }
}
