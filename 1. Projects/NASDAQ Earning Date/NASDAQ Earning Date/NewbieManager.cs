using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using RDotNet;


namespace NASDAQ_Earning_Date
{
    class NewbieManager
    {
        private string symbol;
        private List<DateTime> OtherDates;
        private List<DateTime> ZackEarningDates;
        private DataTable AllDates;
        private int NumPredictDates = 2;
        private string SymbolFolder;
        private string dateFormat = "M/d/yyyy";
        private REngine engine;
        private NASDAQConnect nc;

        public NewbieManager(string _symbol,NASDAQConnect _nc)
        {
            symbol = _symbol;
            nc = _nc;
            OtherDates = new List<DateTime>();
            InitializeTable();
            SymbolFolder = NControl.mainDirectory + symbol + "\\";
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Evaluate("require('bizdays')");
            engine.Evaluate("load_quantlib_calendars('UnitedStates/NYSE', from = '1970-01-01', to = '2021-12-31')");
        }

        private void InitializeTable()
        {
            DataColumn dc;
            AllDates = new DataTable();

            dc = new DataColumn();
            dc.ColumnName = "Type";
            dc.DataType = typeof(string);
            AllDates.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "Date";
            dc.DataType = typeof(DateTime);
            AllDates.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "Remarks";
            dc.DataType = typeof(string);
            AllDates.Columns.Add(dc);
        }

        public void CreateFiles()
        {
            consolidateDates();

            using (StreamWriter sw = new StreamWriter(SymbolFolder + "Dates.csv",false))
            {
                sw.WriteLine("Type,Date,Remarks");
                foreach (DataRow dr in AllDates.Rows)
                {
                    if (dr.Field<DateTime>("Date") < DateTime.Today)
                    {
                        sw.WriteLine(string.Format("{0},{1},{2}", dr.Field<string>("Type"),
                                         dr.Field<DateTime>("Date").ToString(dateFormat),
                                         dr.Field<string>("Remarks")));
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(SymbolFolder + "FutureDates.csv", false))
            {
                sw.WriteLine("Type,Date,Remarks");
                foreach (DataRow dr in AllDates.Rows)
                {
                    if (dr.Field<DateTime>("Date") >= DateTime.Today)
                    {
                        sw.WriteLine(string.Format("{0},{1},{2}", dr.Field<string>("Type"),
                                         dr.Field<DateTime>("Date").ToString(dateFormat),
                                         dr.Field<string>("Remarks")));
                    }
                }
            }
        }

        private void consolidateDates()
        {
            List<DateTime> NextNDates = PredictNextN(NumPredictDates);

            getOtherDates();

            // Consolidate Zacks Dates
            foreach (DateTime datee in ZackEarningDates)
            {
                if (datee > NControl.StartDate)
                {
                    DataRow dr = AllDates.NewRow();
                    dr["Type"] = "Earnings";
                    dr["Date"] = datee;
                    AllDates.Rows.Add(dr);
                }
            }

            // Consolidate Predict Dates
            foreach (DateTime datee in NextNDates)
            {
                DataRow dr = AllDates.NewRow();
                dr["Type"] = "Earnings";
                dr["Date"] = datee;
                AllDates.Rows.Add(dr);
            }
        }

        private void getOtherDates()
        {
            using (StreamReader sr = new StreamReader(NControl.OtherShocksDate))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    DataRow dr = AllDates.NewRow();
                    string line = sr.ReadLine();
                    string[] lineItems = line.Split(',');

                    dr["Type"] = lineItems[0];
                    dr["Date"] = DateTime.ParseExact(lineItems[1], dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    try
                    {
                        dr["Remarks"] = lineItems[2];
                    }
                    catch (IndexOutOfRangeException)
                    {}
                    AllDates.Rows.Add(dr);
                }
            }
        }

        private List<DateTime> PredictNextN(int n)
        {
            List<int>[] QuarterBucketList = new List<int>[4];
            List<DateTime> NextNDates = new List<DateTime>();
            DateTime lastZackEarning;


            ZackEarningDates = nc.GetAllDates(symbol);
            lastZackEarning = ZackEarningDates.Max();

            foreach (DateTime item in ZackEarningDates)
            {
                int QuarterIndex = item.Month / 3 + (((item.Month % 3) > 0) ? 1 : 0) - 1;
                if (QuarterBucketList[QuarterIndex] is null)
                {
                    QuarterBucketList[QuarterIndex] = new List<int>();
                }
                QuarterBucketList[QuarterIndex].Add((item - new DateTime(item.Year, 1, 1)).Days);
            }

            for (int i = 0; i < n; i++)
            {
                DateTime theDate;
                int theYear;
                int theQuarter;

                theYear = lastZackEarning.AddMonths(3 * (i + 1)).Year;
                theQuarter = lastZackEarning.AddMonths(3 * (i + 1)).Month;
                theQuarter = theQuarter / 3 + (((theQuarter % 3) > 0) ? 1 : 0);

                theDate = new DateTime(theYear, 1, 1).AddDays(QuarterBucketList[theQuarter - 1].Average());

                NextNDates.Add(NextBusinessDay(theDate));
            }

            return NextNDates;
        }

        private DateTime NextBusinessDay(DateTime dt)
        {
            double result;
            engine.Evaluate(string.Format("dt = '{0}'",dt.AddDays(-1).ToString("yyyy-MM-dd")));
            engine.Evaluate("NextBusinessDay = function(x){return (following(as.Date(x, ' %Y-%m-%d'), 'QuantLib/UnitedStates/NYSE'))}");
            result = engine.Evaluate(string.Format("Dummy = NextBusinessDay(dt)")).AsNumeric().First();
            return (new DateTime(1970, 1, 1).AddDays(result));
        }
    }
}
