using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RDotNet;

namespace NASDAQ_Earning_Date
{
    class RerunManager
    {
        private List<string> symbolList;
        private REngine engine;
        private NASDAQConnect nc;

        public RerunManager(NASDAQConnect _nc)
        {
            nc = _nc;
            symbolList = new List<string>();
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
        }

        public void AddSymbol(string _symbol)
        {
            symbolList.Add(_symbol);
        }

        public int symbolCount()
        {
            return symbolList.Count();
        }

        public void Rerun()
        {
            ModifyDateConfigs();

            // Construct symbol List cmd
            string symbolListCmd = string.Format("symbols = c('{0}')", string.Join("','", symbolList));
            engine.Evaluate(symbolListCmd);
            engine.Evaluate(string.Format("source('{0}')", NControl.RerunCode.Replace('\\','/')));
        }

        private void ModifyDateConfigs()
        {
            foreach (string symbol in symbolList)
            {
                ModifyDatesConfig(symbol);
            }
        }

        public void ModifyDatesConfig(string symbol)
        {
            string dateFormat = "M/d/yyyy";
            string DateFile = NControl.mainDirectory + symbol + @"\Dates.csv";
            string FutureDateFile = NControl.mainDirectory + symbol + @"\FutureDates.csv";
            List<string[]> AllOldDates = new List<string[]>();
            List<string[]> AllFutureDates = new List<string[]>();
            List<DateTime> OldDates = new List<DateTime>();
            List<DateTime> FutureDates = new List<DateTime>();
            List<DateTime> ZackDates;
            List<DateTime> CombinedDates = new List<DateTime>();
            StringBuilder sb = new StringBuilder();

            // Read in Dates, Future Dates Config Files
            using (StreamReader sr = new StreamReader(DateFile))
            {
                string[] lineItems;
                DateTime iDate = new DateTime();
                while (!sr.EndOfStream)
                {
                    lineItems = sr.ReadLine().Split(',');
                    AllOldDates.Add(lineItems);
                    if (lineItems[0] == "Earnings")
                    {
                        iDate = DateTime.ParseExact(lineItems[1], dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                        OldDates.Add(iDate);
                    }
                }
            }
            OldDates.Sort();

            using (StreamReader sr = new StreamReader(FutureDateFile))
            {
                string[] lineItems;
                DateTime iDate = new DateTime();
                while (!sr.EndOfStream)
                {
                    lineItems = sr.ReadLine().Split(',');
                    AllFutureDates.Add(lineItems);
                    if (lineItems[0] == "Earnings")
                    {
                        iDate = DateTime.ParseExact(lineItems[1], dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                        FutureDates.Add(iDate);
                    }
                }
            }
            FutureDates.Sort();


            // Scrape Zack's 
            ZackDates = nc.GetAllDates(symbol);
            ZackDates.Sort();


            // Create Combined List
            CombinedDates = new List<DateTime>(OldDates);
            CombinedDates.AddRange(FutureDates);
            for (int i = 0; i < CombinedDates.Count; i++)
            {
                DateTime iDate = CombinedDates[i];
                foreach (DateTime jDate in ZackDates)
                {
                    if (Math.Abs((jDate - iDate).Days) < NControl.separateDay)
                    {
                        CombinedDates[i] = jDate;
                        break;
                    }
                }
            }
            

            // Modify Old Date Lists Configs
            foreach (string[] DateCombo in AllOldDates)
            {
                if (DateCombo[0] != "Earnings")
                {
                    sb.AppendLine(string.Join(",", DateCombo));
                }
            }
            foreach (DateTime datee in CombinedDates)
            {
                if (datee < DateTime.Today)
                {
                    sb.AppendLine(string.Format("Earnings,{0}", datee.ToString(dateFormat)));
                }
                else
                {
                    break;
                }
            }
            using (StreamWriter sw = new StreamWriter(DateFile, append: false))
            {
                sw.Write(sb.ToString());
            }
            sb.Clear();


            // Modify Future Date Lists Configs
            foreach (string[] DateCombo in AllFutureDates)
            {
                if (DateCombo[0] != "Earnings")
                {
                    sb.AppendLine(string.Join(",", DateCombo));
                }
            }
            foreach (DateTime datee in CombinedDates)
            {
                if (datee >= DateTime.Today)
                {
                    sb.AppendLine(string.Format("Earnings,{0}", datee.ToString(dateFormat)));
                }
            }
            using (StreamWriter sw = new StreamWriter(FutureDateFile, append: false))
            {
                sw.Write(sb.ToString());
            }
            sb.Clear();
        }
    }
}
