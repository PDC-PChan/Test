using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConsoleApplication4;
using IBApi;

namespace ConsoleApplication6
{
    class OrderManager
    {
        private string OrderSheet;
        private IntBrokerSocket ibClient;
        private List<ExcelOrders> myOrders = new List<ExcelOrders>();
        private const double ContractPriceMin = 0.05;
        private const int ContractTimeSpan = 4;
        private List<Position_Struct> myPortfolio;
        private int nextID;

        public OrderManager(string _orderSheet)
        {
            ibClient = new IntBrokerSocket(0, OControl.connectionPort);
            OrderSheet = _orderSheet;
            nextID = ibClient.reqIDs();
        }

        public void DoAll()
        {
            ReadInOrderSheet();
            placeOrders();
        }

        public void ClosePositionsAuto()
        {
            // Get all positions
            myPortfolio = ibClient.reqPositions();
            myPortfolio = myPortfolio.Where(pos => (pos.contract.SecType == "OPT" && pos.account == OControl.defAccount)).ToList();
            List<CompleteTick_Struct> ContractPrices = IBPrice2DT();

            // Close positions
            for (int i = 0; i < myPortfolio.Count; i++)
            {
                Position_Struct ps = myPortfolio[i];
                DateTime lastTradingDate = DateTime.ParseExact(ps.contract.LastTradeDateOrContractMonth, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                if (ContractPrices[i].getTick(tickPreference.LastPreference) < ContractPriceMin 
                    && (lastTradingDate - DateTime.Today).Days > ContractTimeSpan
                    && ps.pos != 0)
                {
                    Contract myContract = ps.contract;
                    myContract.Exchange = "SMART";

                    // Construct order
                    Order myOrder = new Order();
                    myOrder.Account = OControl.defAccount;
                    myOrder.Action = (ps.pos > 0) ? "SELL" : "BUY";
                    myOrder.TotalQuantity = (int)(-ps.pos);
                    myOrder.OrderType = "LMT";
                    myOrder.LmtPrice = 0.01;
                    myOrder.Tif = "DAY";
                    myOrder.Transmit = true;

                    // Place order
                    nextID++;
                    ibClient.placeOrder(nextID, ps.contract, myOrder);

                    // Acknowledgement
                    Console.WriteLine(string.Format("Order {0} has been placed.", ps.contract.LocalSymbol));
                }
            }

            
        }

        private void ReadInOrderSheet()
        {
            Console.WriteLine(string.Format("{0} : Reading in order sheet ...",DateTime.Now.ToShortTimeString()));
            using (StreamReader sr = new StreamReader(OrderSheet))
            {
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(',');
                    ExcelOrders newOrder = new ExcelOrders();
                    newOrder.symbol = line[0];
                    newOrder.Expiry = line[1];
                    newOrder.strike = Convert.ToDouble(line[2]);
                    newOrder.price = Convert.ToDouble(line[3]);
                    newOrder.position = Convert.ToInt32(line[4]);
                    myOrders.Add(newOrder);
                }
            }
        }

        private void placeOrders()
        {
            Console.WriteLine(string.Format("{0} : Placing orders ...", DateTime.Now.ToShortTimeString()));
            
            ContractDetails conDetails;

            for (int i = 0; i < myOrders.Count; i++)
            {
                // Long for contract
                Contract orderContract = new Contract();
                orderContract.Symbol = myOrders[i].symbol;
                orderContract.SecType = "OPT";
                orderContract.Right = "PUT";
                orderContract.LastTradeDateOrContractMonth = myOrders[i].Expiry;
                orderContract.Strike = myOrders[i].strike;
                orderContract.Exchange = "SMART";
                conDetails = ibClient.reqContractDetails(1, orderContract, false)[0].contractDetails;
                orderContract = conDetails.Summary;

                // Construct order
                Order contractOrder = new Order();
                contractOrder.Account = OControl.defAccount;
                if (myOrders[i].position > 0)
                {
                    Console.WriteLine("Buy order not allowed!");
                    Environment.Exit(0);
                }
                contractOrder.Action = (myOrders[i].position > 0) ? "BUY" : "SELL";
                contractOrder.TotalQuantity = Math.Abs(myOrders[i].position);
                contractOrder.OrderType = "LMT";
                contractOrder.LmtPrice = Math.Ceiling(myOrders[i].price / conDetails.MinTick) * conDetails.MinTick;
                contractOrder.Tif = "DAY";
                contractOrder.Transmit = true;

                // Place Order
                nextID++;
                ibClient.placeOrder(nextID, orderContract, contractOrder);
                //contractOrder.Tif = "GTC"; // Way to hack around irresponsive to GTC order
                //ibClient.placeOrder(nextID, orderContract, contractOrder);

                // Acknowledgement
                Console.WriteLine(string.Format("Order {0} has been placed.", orderContract.LocalSymbol));
            }
        }

        private List<CompleteTick_Struct> IBPrice2DT()
        {
            Console.WriteLine("Reading in portfolio prices ...");
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

    struct ExcelOrders
    {
        public string symbol;
        public string Expiry;
        public double strike;
        public double price;
        public int position;

        public ExcelOrders(string _symbol,string _Expiry, double _strike, double _price, int _position)
        {
            symbol = _symbol;
            Expiry = _Expiry;
            strike = _strike;
            price = _price;
            position = _position;
        }
    }
}
