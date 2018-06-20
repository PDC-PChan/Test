using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualBasic;

namespace NASDAQ_Earning_Date
{
    public class NASDAQConnect
    {
        private IWebDriver driver;

        public NASDAQConnect()
        {
            driver = new ChromeDriver(NControl.mainDirectory + "0.VolAnalysis");
        }

        public DateTime GetNextEarningDate(string symbol)
        {
            DateTime SeleniumDate = SeleniumGetNextEarningDate(symbol);
            DateTime ZackDate = ZacksGetNextEarningDate(symbol);

            return SeleniumDate.Year < 2000 ? ZackDate: SeleniumDate;
        }

        private DateTime SeleniumGetNextEarningDate(string symbol)
        {
            string nasdaqLink = "https://www.optionslam.com/"; //"http://www.nasdaq.com/earnings/report/" + symbol;
            string rawText = "";
            string[] names = System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames;
            string[] yearInterested = new string[] { "2018", "2019", "2020" };
            int DateStartAnchor = -1, DateEndAnchor = -1;
            DateTime resultDate = new DateTime();
            bool shortMonthFlag = false;

            // Go to new ip
            driver.Url = nasdaqLink;
            //Thread.Sleep(3000);

            driver.FindElement(By.CssSelector("body > table:nth-child(8) > tbody > tr:nth-child(1) > td:nth-child(3) > form > input[type=\"text\"]:nth-child(2)")).Clear();
            driver.FindElement(By.CssSelector("body > table:nth-child(8) > tbody > tr:nth-child(1) > td:nth-child(3) > form > input[type=\"text\"]:nth-child(2)")).SendKeys(symbol);
            driver.FindElement(By.CssSelector("body > table:nth-child(8) > tbody > tr:nth-child(1) > td:nth-child(3) > form > input.red-button")).Click();

            // Wait for page to load and get date
            rawText = driver.FindElement(By.XPath("//*[contains(text(), 'Next Earnings Date')]")).Text;
            rawText = rawText.Split('\r')[0];
            rawText = rawText.Substring(rawText.LastIndexOf(":"));

            // Analyse raw string
            // Try long month names
            foreach (string mthNames in names)
            {
                if (mthNames != "" && rawText.IndexOf(mthNames) != -1)
                {
                    DateStartAnchor = rawText.IndexOf(mthNames);
                    break;
                }
            }

            // Try short month names
            if (DateStartAnchor == -1)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (rawText.IndexOf(System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i + 1)) != -1)
                    {
                        DateStartAnchor = rawText.IndexOf(System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i + 1));
                        shortMonthFlag = true;
                        break;
                    }
                }
            }

            if (DateStartAnchor != -1)
            {
                foreach (string yearName in yearInterested)
                {
                    if (rawText.IndexOf(yearName, DateStartAnchor) != -1) // ArgumentOutOfRangeException
                    {
                        DateEndAnchor = rawText.IndexOf(yearName, DateStartAnchor) + 4;
                    }
                }

                rawText = rawText.Substring(DateStartAnchor, DateEndAnchor - DateStartAnchor);
                try
                {
                    resultDate = DateTime.ParseExact(rawText, (shortMonthFlag) ? "MMM. d, yyyy" : "MMMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (System.FormatException)
                {
                    rawText = rawText.Replace("Sept", "Sep");
                    shortMonthFlag = true;
                    resultDate = DateTime.ParseExact(rawText, (shortMonthFlag) ? "MMM. d, yyyy" : "MMMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return (resultDate);
        }

        private DateTime ZacksGetNextEarningDate(string symbol)
        {
            string url = string.Format(@"https://www.zacks.com/stock/research/{0}/earnings-announcements", symbol) ;
            string HTMLCode = GetHTMLCode(url);
            DateTime resultDate = new DateTime();

            string track1 = "\"earnings_announcements_earnings_table\"";
            string track2 = "\"";

            int local1 = 0;
            int targetStart = 0;
            int targetEnd = 0;

            local1 = HTMLCode.IndexOf(track1, local1 + 1);
            local1 = HTMLCode.IndexOf(track1, local1 + 1);
            
            targetStart = HTMLCode.IndexOf(track2, local1 + track1.Length);
            targetEnd = HTMLCode.IndexOf(track2, targetStart+1);

            DateTime.TryParseExact(HTMLCode.Substring(targetStart+1,targetEnd-targetStart-1), "M/d/yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None,out resultDate);

            //Console.WriteLine(symbol + "," + resultDate.ToShortDateString());

            return resultDate;
        }


        public List<DateTime> GetAllDates(string symbol)
        {
            string url = string.Format(@"https://www.zacks.com/stock/research/{0}/earnings-announcements", symbol);
            string HTMLCode = GetHTMLCode(url);
            List<DateTime> resultDates = new List<DateTime>();
            DateTime iDate;
            DateTime NextEarningsDate = GetNextEarningDate(symbol);

            string track1 = "\"earnings_announcements_earnings_table\"";
            string track2 = "[ \"";
            string track3 = "\"";
            string trackEnd = "earnings_announcements_webcasts_table";

            int local1 = 0;
            int targetStart = 0;
            int targetEnd = 0;
            int local2 = 0;

            local1 = HTMLCode.IndexOf(track1, local1 + 1);
            local1 = HTMLCode.IndexOf(track1, local1 + 1);
            local2 = HTMLCode.IndexOf(trackEnd, local1 + 1);

            targetStart = HTMLCode.IndexOf(track2, local1 + track1.Length);
            targetEnd = HTMLCode.IndexOf(track3, targetStart + track2.Length);

            while (targetStart < local2)
            {
                iDate = DateTime.ParseExact(HTMLCode.Substring(targetStart + track2.Length, targetEnd - targetStart - track2.Length), 
                    "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
                if (DateAndTime.DatePart("yyyy", iDate) + DateAndTime.DatePart("q",iDate) ==
                    DateAndTime.DatePart("yyyy", NextEarningsDate) + DateAndTime.DatePart("q", NextEarningsDate))
                {
                    iDate = NextEarningsDate;
                }
                resultDates.Add(iDate);

                //Console.WriteLine(iDate);
                targetStart = HTMLCode.IndexOf(track2, targetEnd + track3.Length);
                targetEnd = HTMLCode.IndexOf(track3, targetStart + track2.Length);
            }

            return resultDates;
        }


        public static string GetHTMLCode(string urlAddress)
        {
            string data = "";
            //var handler = new HttpClientHandler()
            //{
            //    AllowAutoRedirect = true,
            //    MaxAutomaticRedirections = 1000,
            //    UseDefaultCredentials = true,
            //    CookieContainer = new CookieContainer()
            //};
            //HttpClient client = new HttpClient(handler);

            //using (HttpResponseMessage response = client.GetAsync(urlAddress).Result)
            //{
            //    using (HttpContent content = response.Content)
            //    {
            //        data = content.ReadAsStringAsync().Result;
            //    }
            //}


            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            //request.UseDefaultCredentials = true;
            //request.Method = "GET";
            //request.CookieContainer = new CookieContainer();
            //request.AllowAutoRedirect = false;

            //try
            //{

            //    //request.Method = "GET";
            //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //    {
            //        if (response.StatusCode == HttpStatusCode.OK)
            //        {
            //            Stream receiveStream = response.GetResponseStream();
            //            StreamReader readStream = null;

            //            if (response.CharacterSet == null)
            //            {
            //                readStream = new StreamReader(receiveStream);
            //            }
            //            else
            //            {
            //                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            //            }

            //            data = readStream.ReadToEnd();

            //            response.Close();
            //            readStream.Close();
            //        }
            //    }
            //}
            //catch (System.Net.WebException e)
            //{
            //    throw;
            //}

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
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


            return data;
        }

    }



}
