using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NASDAQ_Earning_Date
{
    class FileConnect
    {
        public enum RunAction
        {
            ReRun,
            Move,
            New,
            Unchange
        };

        public static DateTime GetRecordNextDate(string name)
        {
            DateTime NextEarning = new DateTime(2100, 12, 31);
            DateTime iDate;

            try
            {
                using (StreamReader sr = new StreamReader(NControl.mainDirectory + name + @"\FutureDates.csv"))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] lineItems = sr.ReadLine().Split(',');
                        if (lineItems[0] == "Earnings")
                        {
                            iDate = DateTime.ParseExact(lineItems[1], "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            NextEarning = (iDate < NextEarning) ? iDate : NextEarning;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return NextEarning;
        }

        public static RunAction generateAction(DateTime NasdaqDate, DateTime RecordDate)
        {
            RunAction action = RunAction.Unchange;

            if (NasdaqDate < DateTime.Today)
            {
                if (Math.Abs((RecordDate-NasdaqDate).Days) < NControl.separateDay ||
                    RecordDate < NasdaqDate)
                {
                    action = RunAction.ReRun;
                }
            }
            else
            {
                if ((NasdaqDate - RecordDate).Days > NControl.separateDay)
                {
                    action = RunAction.ReRun;
                }else
                {
                    if (NasdaqDate != RecordDate)
                    {
                        action = RunAction.Move;
                    }
                }
            }

            return action;
        }

        public static void ModifyShockMatrix(string symbol, DateTime fromDate, DateTime ToDate)
        {
            string ShockMatrixPath = NControl.mainDirectory + symbol + @"\ShockMatrix.csv";
            Dictionary<int, List<double>> ShockMatrix = new Dictionary<int, List<double>>();
            string[] lineItems;
            List<double> ZeroList;
            List<double>[] tmpListArray = new List<double>[2];
            int FromDateIndex = (fromDate - new DateTime(1970, 1, 1)).Days;
            int ToDateIndex = (ToDate - new DateTime(1970, 1, 1)).Days;

            // Read in ShockMatrix to Dictionary
            using (StreamReader sr = new StreamReader(ShockMatrixPath))
            {
                while (!sr.EndOfStream)
                {
                    List<double> tmpList = new List<double>();

                    lineItems = sr.ReadLine().Split(',');
                    for (int i = 1; i < lineItems.Length; i++)
                    {
                        tmpList.Add(Convert.ToDouble(lineItems[i]));
                    }

                    ShockMatrix.Add(Convert.ToInt16(lineItems[0]), tmpList);
                }
            }

            //Console.WriteLine(ShockMatrix[FromDateIndex][0]);
            //Console.WriteLine(ShockMatrix[FromDateIndex+1][0]);
            //Console.WriteLine(ShockMatrix[ToDateIndex][0]);
            //Console.WriteLine(ShockMatrix[ToDateIndex+1][0]);

            // Swap
            ZeroList = new List<double>(new double[ShockMatrix[ToDateIndex].Count]);
            tmpListArray[0] = new List<double>(ShockMatrix[FromDateIndex]);
            tmpListArray[1] = new List<double>(ShockMatrix[FromDateIndex+((fromDate.DayOfWeek == DayOfWeek.Friday)?3:1)]);
            ShockMatrix[FromDateIndex] = ZeroList;
            ShockMatrix[FromDateIndex+ ((fromDate.DayOfWeek == DayOfWeek.Friday) ? 3 : 1)] = ZeroList;
            ShockMatrix[ToDateIndex] = tmpListArray[0];
            ShockMatrix[ToDateIndex + ((ToDate.DayOfWeek == DayOfWeek.Friday) ? 3 : 1)] = tmpListArray[1];

            //Console.WriteLine(ShockMatrix[FromDateIndex][0]);
            //Console.WriteLine(ShockMatrix[FromDateIndex + 1][0]);
            //Console.WriteLine(ShockMatrix[ToDateIndex][0]);
            //Console.WriteLine(ShockMatrix[ToDateIndex + 1][0]);

            // Write Shock matrix
            List<int> SortedKey = ShockMatrix.Keys.ToList();
            SortedKey.Sort();
            using (StreamWriter sw = new StreamWriter(ShockMatrixPath,append:false))
            {
                for (int i = 0; i < SortedKey.Count; i++)
                {
                    sw.WriteLine("{0},{1}", SortedKey[i], string.Join(",", ShockMatrix[SortedKey[i]].ToArray()));
                }
                
            }
        }

        public static void UpdateDatesConfig(string symbol, DateTime fromDate, DateTime ToDate)
        {
            string AllLines;
            string dateFormat = "M/d/yyyy";
            string FutureDateFile = NControl.mainDirectory + symbol + @"\FutureDates.csv";

            using (StreamReader sr = new StreamReader(FutureDateFile))
            {
                AllLines = sr.ReadToEnd();
                AllLines = AllLines.Replace("Earnings," + fromDate.ToString(dateFormat), 
                    "Earnings," + ToDate.ToString(dateFormat));
            }

            using (StreamWriter sw = new StreamWriter(FutureDateFile,append:false))
            {
                sw.Write(AllLines);
            }
        }

    }
}
