using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication4;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] symbolList = new string[] {
                                                "AAPL",
                                                "ACN",
                                                "ADBE",
                                                "ADP",
                                                "ADSK",
                                                "AMZN",
                                                "APD",
                                                "BA",
                                                "BAX",
                                                "BLK",
                                                "CAT",
                                                "CME",
                                                "CMI",
                                                "CRM",
                                                "CSCO",
                                                "CTSH",
                                                "DIS",
                                                "EBAY",
                                                "EL",
                                                "EA",
                                                "F",
                                                "GD",
                                                "GLW",
                                                "HON",
                                                "HP",
                                                "IBM",
                                                "INTC",
                                                "INTU",
                                                "ITW",
                                                "JNJ",
                                                "JPM",
                                                "KO",
                                                "MA",
                                                "MCD",
                                                "MDT",
                                                "MMM",
                                                "MRK",
                                                "MSFT",
                                                "NKE",
                                                "NVDA",
                                                "ORCL",
                                                "PFE",
                                                "PG",
                                                "PM",
                                                "PSA",
                                                "QCOM",
                                                "RHT",
                                                "RTN",
                                                "SNPS",
                                                "T",
                                                "TDG",
                                                "TEL",
                                                "TIF",
                                                "TXN",
                                                "TXT",
                                                "UNP",
                                                "V",
                                                "WLTW",
                                                "XOM",
                                                "XRX"
            };


            DashboardManager dbm = new DashboardManager();
            CalculationManager_v2 cm = new CalculationManager_v2(symbolList,dbm, "PUT");
            

            //DataManager cm = new DataManager(_symbol:symbol,
            //                                 _right: "PUT",
            //                                 _IncludeOppositeWing:true,
            //                                 _ReadInContract: false,
            //                                 _readInContractAdd: @"C:\Users\Samuel\Documents\Visual Studio 2013\Projects\ConsoleApplication5\"+ symbolList[0] + ".txt");

            ////cm.RefreshData();
            //SimulationManager sm = new SimulationManager(symbol);
            //sm.EvaluateSimulation();
            //sm.GetSimulatedPrices(new DateTime(2017, 12, 31));
            //Console.WriteLine(sm.Probability_OTM(60));
            //Console.WriteLine(sm.Probability_OTM(60,"CALL"));


        }
    }
}
