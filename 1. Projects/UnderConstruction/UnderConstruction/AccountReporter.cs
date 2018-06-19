using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication4;
using IBApi;
using System.IO;
using System.Threading;

namespace UnderConstruction
{
    class AccountReporter
    {
        private string AccountNumber;
        private IntBrokerSocket ibClient;
        private List<Position_Struct> myPortfolio;
        private List<OpenOrder_Struct> myOrders;
        private Dictionary<string, int> PositionCnt = new Dictionary<string, int>();
        private DateTime runDate;
        private double ContractPriceMin = 0.05;
        

        public AccountReporter(string _AccountNumber, int connectionPort)
        {
            // Variables
            runDate = DateTime.Now;
            AccountNumber = _AccountNumber;
            ibClient = new IntBrokerSocket(0, connectionPort);
        }

        public string RunProcedures()
        {
            // Request Data from IB
            Console.WriteLine("{0}: Obtaining position info ...", DateTime.Now.ToShortTimeString());

            string holdingsFile = UControl.HoldingsFolder + runDate.ToString("yyyyMMdd") + ".txt";
            myPortfolio = ibClient.reqPositions();
            myPortfolio = myPortfolio.Where(pos => (pos.contract.SecType == "OPT" && pos.account == AccountNumber)).ToList();

            // Get prices of contracts
            List<CompleteTick_Struct> ContractPrices = IBPrice2DT();


            // Filter invalid contracts
            for (int i = 0; i < myPortfolio.Count; i++)
            {
                Position_Struct ps = myPortfolio[i];
                if (ContractPrices[i].getTick(tickPreference.LastPreference) > ContractPriceMin)
                {
                    try
                    {
                        PositionCnt.Add(ps.contract.Symbol, 0);
                    }
                    catch (ArgumentException)
                    { }
                    PositionCnt[ps.contract.Symbol] -= (int)ps.pos;
                }
            }

            // Output position file
            using (StreamWriter sw = new StreamWriter(holdingsFile, false))
            {
                foreach (string symbol in PositionCnt.Keys)
                {
                    sw.WriteLine("{0},{1}", symbol, PositionCnt[symbol]);
                }
            }

            return (holdingsFile);
        }

        public void RemoveAllOpenOrders()
        {
            OrderAndStatus_Struct myOrders = ibClient.reqAllOpenOrders();
            Thread.Sleep(5000);

            // Loop and verify each order
            foreach (OpenOrder_Struct oos in myOrders.openOrder_Struct.ToList())
            {
                bool toCancel = false;

                // Check if SELL + OPT + PUT
                toCancel = (oos.order.Action == "SELL") && (oos.contract.SecType == "OPT") && (oos.contract.Right == "P");

                if (toCancel)
                {
                    ibClient.cancelOrder(oos.orderId);
                }
            }
        }

        private List<CompleteTick_Struct> IBPrice2DT()
        {
            //###########################################################################################
            //      Obtain option price
            //###########################################################################################
            List<Contract> AllContracts = new List<Contract>();
            List<CompleteTick_Struct> myMkt;


            // Create and send request
            foreach (Position_Struct pos in myPortfolio)
            {
                Contract tmpContract = ibClient.reqContractDetails(Toolkit.RndReqId(), pos.contract)[0].contractDetails.Summary;
                AllContracts.Add(tmpContract);
            }
            ibClient.SetMarketDataType(IntBrokerSocket.MarketDataType.Delayed);
            myMkt = ibClient.GetMktSnapShotData(AllContracts);


            return myMkt;
        }
    }
}
