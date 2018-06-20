using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Net;
using System.Data;
using System.Threading;

namespace NASDAQ_Earning_Date
{
    class Program
    {

        static void Main(string[] args)
        {
            string symbol = "";
            List<string> symbolList = new List<string>();
            List<string> NewbieList = new List<string>();
            ConcurrentDictionary<string, DateTime> SymbolEarningDate = new ConcurrentDictionary<string, DateTime>();
            ConcurrentDictionary<string, DateTime> SymbolRecordDate = new ConcurrentDictionary<string, DateTime>();
            DataTable DateTable = new DataTable();
            DataColumn dc;
            DataRow dr;
            StringBuilder sb = new StringBuilder();
            NASDAQConnect nc = new NASDAQConnect();
            RerunManager rm = new RerunManager(nc);
            //NasdaqSelenium ns = new NasdaqSelenium();


            //#################################################################################
            //                      Initialize Datatable
            //#################################################################################
            dc = new DataColumn("Symbol", typeof(string));
            DateTable.Columns.Add(dc);
            dc = new DataColumn("NasdaqDate", typeof(DateTime));
            DateTable.Columns.Add(dc);
            dc = new DataColumn("RecordDate", typeof(DateTime));
            DateTable.Columns.Add(dc);
            dc = new DataColumn("Action", typeof(string));
            DateTable.Columns.Add(dc);


            //#################################################################################
            //                      Read Symbols
            //#################################################################################
            using (StreamReader sr = new StreamReader(NControl.symbolListAdd))
            {
                while (!sr.EndOfStream)
                {
                    symbol = sr.ReadLine();
                    symbolList.Add(symbol);
                    dr = DateTable.NewRow();
                    dr["Symbol"] = symbol;
                    DateTable.Rows.Add(dr);
                }
            }
            

            //#################################################################################
            //                      Get Dates and Compare
            //#################################################################################
            Console.WriteLine("Symbol,NasdaqDate,RecordDate,Action");
            foreach (DataRow idr in DateTable.Rows)
            {
                DateTime nasdaqDate = nc.GetNextEarningDate(idr["Symbol"].ToString());
                idr["NasdaqDate"] = nasdaqDate;
                try
                {
                    DateTime RecordDate = FileConnect.GetRecordNextDate(idr["Symbol"].ToString());
                    idr["RecordDate"] = RecordDate;
                    idr["Action"] = FileConnect.generateAction(nasdaqDate, RecordDate);
                }
                catch (Exception e)
                {
                    if (e is DirectoryNotFoundException || e is FileNotFoundException)
                    {     
                        NewbieList.Add(idr["Symbol"].ToString());
                        idr["Action"] = FileConnect.RunAction.New;
                    }
                }
                Console.WriteLine(string.Join(" ; ", idr.ItemArray));

            }

            //#################################################################################
            //                      Take Action for New joiners
            //#################################################################################
            if (NewbieList.Count > 0)
            {
                Console.WriteLine("Take action for new joiners? (Y/N)");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    foreach (DataRow idr in DateTable.Rows)
                    {
                        if (idr["Action"].ToString() == "New")
                        {
                            NewbieManager nb = new NewbieManager(idr.Field<string>("Symbol"),nc);
                            nb.CreateFiles();
                            idr["Action"] = FileConnect.RunAction.ReRun;
                        }
                    }
                }
            }


            //#################################################################################
            //                      Force Rerun Settings
            //#################################################################################
            //DateTable.Select("Symbol = 'JPM'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'BLK'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'CSCO'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'MCD'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'KO'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'INTC'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'HP'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'DIS'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'AMZN'")[0]["Action"] = FileConnect.RunAction.ReRun;
            //DateTable.Select("Symbol = 'MDT'")[0]["Action"] = FileConnect.RunAction.Unchange;
            //DateTable.Select("Symbol = 'PSA'")[0]["Action"] = FileConnect.RunAction.Unchange;


            //#################################################################################
            //                      Take Action
            //#################################################################################
            Console.WriteLine("Continue to take action? (Y/N)");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                foreach (DataRow idr in DateTable.Rows)
                {
                    switch (idr["Action"].ToString())
                    {
                        case "Move":
                            FileConnect.ModifyShockMatrix(idr.Field<string>("Symbol"), idr.Field<DateTime>("RecordDate"), idr.Field<DateTime>("NasdaqDate"));
                            FileConnect.UpdateDatesConfig(idr.Field<string>("Symbol"), idr.Field<DateTime>("RecordDate"), idr.Field<DateTime>("NasdaqDate"));
                            Console.WriteLine("Modified Matrix - {0}", idr.Field<string>("Symbol"));
                            break;
                        case "ReRun":
                            rm.AddSymbol(idr.Field<string>("Symbol"));
                            break;
                        case "Unchange":
                            NewbieManager nm = new NewbieManager(idr.Field<string>("Symbol"),nc);
                            nm.CreateFiles();
                            break;
                        case "New":
                            rm.AddSymbol(idr.Field<string>("Symbol"));
                            break;
                        default:
                            break;
                    }
                }
                if (rm.symbolCount() > 0)
                {
                    rm.Rerun();
                }
            }
            

            Console.Write("Press key to continue ...");
            Console.Read();
        }
    }
}
