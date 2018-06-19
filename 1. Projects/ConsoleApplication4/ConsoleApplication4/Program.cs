using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using IBApi;
using System.Collections.Concurrent;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            //IntBrokerSocket ibClient = new IntBrokerSocket();
            //CompleteTick_Struct mymkt;
            //Contract contract = new Contract();

            ////contract.Symbol = "EUR";
            ////contract.SecType = "Cash";
            ////contract.Currency = "GBP";
            ////contract.Exchange = "IDEALPRO";

            ////contract.Symbol = "MSFT";
            ////contract.SecType = "OPT";
            ////contract.LastTradeDateOrContractMonth = "201705";
            ////contract.Strike = 60;
            ////contract.Right = "PUT";
            ////contract.Currency = "USD";

            ////List<ContractDetails_Struct> conDet = ibClient.reqContractDetails(new Random().Next(10), contract);

            ////ibClient.TurnOnMktData(1, conDet[0].contractDetails.Summary, "");
            
            ////while (true)
            ////{
            ////    mymkt = ibClient.GetMktData();
            ////    Console.WriteLine("############### {0} ###############", Toolkit.TranslateTime(ibClient.reqCurrentTime()).ToLongTimeString());
            ////    foreach (string key in mymkt.TickDictionary.Keys)
            ////    {
            ////        Console.WriteLine(key + " : " + mymkt.TickDictionary[key].ToString());
            ////    }
            ////    Thread.Sleep(3000);
            ////}

            ////Console.WriteLine(ibClient.GetRequiredMargin(conDet[0].contractDetails.Summary, "SELL", 20000));

            //ConcurrentDictionary<ExpiryStrikePair, Contract> dict = ibClient.getUniqueOptionChain("MSFT","P");

            //// Required Margin for each contract
            //List<double> reqMargin = new List<double>();
            //foreach (var key in dict.Keys)
            //{
            //    double sdff = ibClient.GetRequiredMargin(dict[key], "SELL", 1);
            //    reqMargin.Add(sdff);
            //    Console.WriteLine(sdff);
            //}

            //Console.Read();
        }
    }
}
