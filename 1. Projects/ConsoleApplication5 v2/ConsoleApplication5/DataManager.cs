using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Concurrent;
using ConsoleApplication4;
using IBApi;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace ConsoleApplication5
{
    public class DataManager
    {
        //##################################################################
        //                  Variables and initialization
        //##################################################################
        #region Variables and initialization

        private string this_symbol;
        private DataTable outputDataTable = new DataTable();
        private string[] colnames = DataMgrCon.colNames.Concat(
                                    SimuMgrCon.colNames.Concat(
                                        CalMgrCon.colNames)).ToArray();
        private Type[] colTypes = DataMgrCon.colTypes.Concat(
                                    SimuMgrCon.colTypes.Concat(
                                        CalMgrCon.colTypes)).ToArray();
        private ConcurrentDictionary<ExpiryStrikePair, Contract> UniqueOptChain = new ConcurrentDictionary<ExpiryStrikePair, Contract>();
        private IntBrokerSocket ibClient;
        private DashboardManager dbm;
        private string readInContractAdd = "";
        private bool ReadInContract;
        private string right;
        private bool IncludeOppositeWing;
        private double symbolPrice = -1;
        private double ExecutableP = 0.3;
        private int clientID;
        public string Symbol
        {
            get { return this_symbol; }
            set { this_symbol = value; }
        }

        public DataTable OptionTable
        {
            get { return outputDataTable; }
        }

        public string[] ColumnNames
        {
            get { return colnames; }
        }

        public DataManager(DashboardManager DashboardMgr, string _symbol, string _right, bool _IncludeOppositeWing, int _clientId, bool _ReadInContract, string _secType = "STK",String _readInContractAdd = "")
        {
            dbm = DashboardMgr;
            clientID = _clientId;
            ibClient = new IntBrokerSocket(clientID, Control.connectionPort);
            this_symbol = _symbol;
            initiateDataTable();
            ReadInContract = _ReadInContract;
            readInContractAdd = _readInContractAdd;
            right = _right;
            IncludeOppositeWing = _IncludeOppositeWing;
            symbolPrice = getSymbolPrice(_secType);
        }


        private double getSymbolPrice(string secType)
        {
            dbm.WriteLine(clientID,"Obtaining undelying price ...", true);

            // get Underlying Contract
            Contract underlying = new Contract();
            underlying.Symbol = this_symbol;
            underlying.SecType = secType;
            underlying = ibClient.reqContractDetails(1, underlying)[0].contractDetails.Summary;

            // Request Price
            do
            {
                ibClient.SetMarketDataType(IntBrokerSocket.MarketDataType.Delayed);
                CompleteTick_Struct mymkt = ibClient.GetMktSnapShotData(new List<Contract>(new Contract[] { underlying }))[0];
                symbolPrice = mymkt.getTick(tickPreference.LastPreference);
            } while (symbolPrice <= 0);

            return symbolPrice;
        }

        private void initiateDataTable()
        {
            // Initiate Columns
            for (int i = 0; i < colnames.Length; i++)
            {
                DataColumn column = new DataColumn();
                column.DataType = colTypes[i];
                column.ColumnName = colnames[i];
                outputDataTable.Columns.Add(column);
            }

            // Make Expiry + Strike primary key
            outputDataTable.PrimaryKey = new DataColumn[] { outputDataTable.Columns["Expiry"], outputDataTable.Columns["Strike"] };



        }

        #endregion


        //##################################################################
        //                 Method to obtain contract scope
        //##################################################################
        #region Method to obtain contract scope

        //Read in from file
        private void ReadInContract_FromFile()
        {
            using (StreamReader sr = new StreamReader(readInContractAdd))
            {
                string line;
                string[] lineItems;
                int cnt = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    lineItems = line.Split(new char[] { ';' });
                    if (DateTime.ParseExact(lineItems[0], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) < Control.simulationEndDate)
                    {
                        Contract tmpContract = new Contract();
                        tmpContract.ConId = Convert.ToInt32(lineItems[2]);
                        tmpContract = ibClient.reqContractDetails(new Random().Next(10), tmpContract)[0].contractDetails.Summary;
                        UniqueOptChain.TryAdd(new ExpiryStrikePair(lineItems[0], Convert.ToDouble(lineItems[1])), tmpContract);
                        cnt++;
                        Console.Write("\r {0} contract obtained", cnt);
                    }

                }
            }
        }

        // read in from TWS
        private void ReadInContract_FromTWS(bool write2File)
        {
            UniqueOptChain = ibClient.getUniqueOptionChain(this_symbol, right);

            if (write2File)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in UniqueOptChain.Keys)
                {
                    sb.AppendLine(string.Format("{0};{1};{2}", key.Expiry, key.Strike, UniqueOptChain[key].ConId));
                }
                using (StreamWriter sw = new StreamWriter(readInContractAdd, false))
                {
                    sw.Write(sb.ToString());
                }
            }

        }

        private void ReadInContract_FromTWS2(bool write2File)
        {
            List<ContractDetails_Struct> OptChain = new List<ContractDetails_Struct>();
            HashSet<ExpiryStrikePair> allExpiryStrikes = new HashSet<ExpiryStrikePair>();
            List<string> allControlMonths = new List<string>();
            DateTime iExpiry = DateTime.Now.AddDays(5);

            // Get list of expiry months of interested
            while (iExpiry <=  Control.simulationEndDate) // new DateTime(2017,10,31)) //
            {
                allControlMonths.Add(iExpiry.ToString("yyyyMM"));
                iExpiry = iExpiry.AddMonths(1);
            }

            // Get all contracts
            foreach (string ExpiryMonth in allControlMonths)
            {
                dbm.WriteLine(clientID, string.Format("Obtaining option list {0} - {1} ...", this_symbol, ExpiryMonth),true);
                Contract tmpCon = new Contract();
                tmpCon.SecType = "OPT";
                tmpCon.Symbol = this_symbol;
                tmpCon.Right = right;
                tmpCon.LastTradeDateOrContractMonth = ExpiryMonth;
                OptChain.AddRange(ibClient.reqContractDetails(1, tmpCon, false));
            }

            // Remove duplicates and consolidate output
            foreach (ContractDetails_Struct item in OptChain)
            {
                UniqueOptChain.TryAdd(new ExpiryStrikePair(item.contractDetails.Summary.LastTradeDateOrContractMonth, item.contractDetails.Summary.Strike)
                    , item.contractDetails.Summary);
            }


            // Write to file
            if (write2File)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in UniqueOptChain.Keys)
                {
                    sb.AppendLine(string.Format("{0};{1};{2}", key.Expiry, key.Strike, UniqueOptChain[key].ConId));
                }
                using (StreamWriter sw = new StreamWriter(readInContractAdd, false))
                {
                    sw.Write(sb.ToString());
                }
            }
        }

        // Filtering
        private void FilterOptionScope()
        {
            foreach (var key in UniqueOptChain.Keys)
            {
                if (DateTime.ParseExact(key.Expiry, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) < Control.simulationEndDate &&
                    DateTime.ParseExact(key.Expiry, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now &&
                    ((right == "CALL") == (key.Strike > symbolPrice) || IncludeOppositeWing))
                {
                    DataRow newRow = outputDataTable.NewRow();
                    newRow["Expiry"] = key.Expiry;
                    newRow["Strike"] = key.Strike;
                    outputDataTable.Rows.Add(newRow);
                }
            }
        }

        //public ExpiryStrikePair[] GetUniqueExpiry
        #endregion


        //##################################################################
        //           Create Table
        //##################################################################
        public void CreateTable()
        {
            // Obtain unique option list
            if (ReadInContract)
                ReadInContract_FromFile();
            else
            {
                ReadInContract_FromTWS2(false);
                if (UniqueOptChain.Count == 0)
                {
                    ReadInContract_FromTWS(false);
                }
            }


            // Filter and put into datatable
            FilterOptionScope();

        }


        //##################################################################
        //          Refresh table data procedures
        //##################################################################
        #region Margin and Price to datatable

        //Margin
        private void FillInMargin()
        {
            dbm.WriteLine(clientID, "Obtaining margin requirements ...", true);
            int cnt = 0;
            foreach (DataRow dr in outputDataTable.Rows)
            {
                string _Expiry = dr["Expiry"].ToString();
                double _Strike = Convert.ToDouble(dr["Strike"].ToString());
                Contract tmpContract = UniqueOptChain[new ExpiryStrikePair(_Expiry, _Strike)];
                double _margin = 0;
                try
                {
                    _margin = ibClient.GetRequiredMargin(tmpContract, "SELL", 1);
                }
                catch (ProcessingRequestException)
                {
                    _margin = double.NaN;
                }

                dr["ReqMar"] = _margin;

                cnt++;
                //Console.WriteLine("{0}: {1}", cnt, string.Join(",", dr.ItemArray.Select(field => field.ToString())));
                dbm.WriteLine(clientID,string.Format("\r {0:0%} Done", ((double)cnt) / outputDataTable.Rows.Count),false);
            }
            dbm.WriteLine(clientID, "", true);
        }

        private void FillInPrices()
        {
            dbm.WriteLine(clientID, "Obtaining contract price info ...", true);
            int cnt = 0;
            List<Contract> ContractList = new List<Contract>();
            List<CompleteTick_Struct> ReturnedTickList;


            // Initiate option list
            foreach (DataRow dr in outputDataTable.Rows)
            {
                string _Expiry = dr["Expiry"].ToString();
                double _Strike = Convert.ToDouble(dr["Strike"].ToString());
                ContractList.Add(UniqueOptChain[new ExpiryStrikePair(_Expiry, _Strike)]);
            }


            //  Request snapshot
            ibClient.SetMarketDataType(IntBrokerSocket.MarketDataType.DelayedFrozen);
            ReturnedTickList = ibClient.GetMktSnapShotData(ContractList);


            // Consolidate data to datatable
            foreach (DataRow dr in outputDataTable.Rows)
            {
                string _Expiry = dr["Expiry"].ToString();
                double _Strike = Convert.ToDouble(dr["Strike"].ToString());

                CompleteTick_Struct mymkt = ReturnedTickList[ContractList.IndexOf(UniqueOptChain[new ExpiryStrikePair(_Expiry, _Strike)])];

                dr[colnames[3]] = mymkt.getTick(tickPreference.BidPreference);
                dr[colnames[4]] = mymkt.getTick(tickPreference.AskPreference);
                dr[colnames[5]] = mymkt.getTick(tickPreference.LastPreference);

                cnt++;
                //Console.WriteLine("{0}: {1}", cnt, string.Join(",",dr.ItemArray.Select(field=>field.ToString())));
                dbm.WriteLine(clientID,string.Format( "\r {0:0%} Done", ((double)cnt) / outputDataTable.Rows.Count),false);
            }
            dbm.WriteLine(clientID, "", true);
        }

        private void CalculatePrice()
        {
            dbm.WriteLine(clientID, "Calculating executable Price  ...",true);
            int cnt = 0;
            foreach (DataRow dr in outputDataTable.Rows)
            {
                double _Bid = Convert.ToDouble(dr[DataMgrCon.colNames[3]].ToString());
                double _Ask = Convert.ToDouble(dr[DataMgrCon.colNames[4]].ToString());
                double _Last = Convert.ToDouble(dr[DataMgrCon.colNames[5]].ToString());
                double[] priceArray = new double[] { _Bid, _Ask, _Last };

                if (_Ask == -1)
                {
                    dr[DataMgrCon.colNames[6]] = -1;
                }
                else
                {
                    dr[DataMgrCon.colNames[6]] = (_Bid == -1 ? 0 : _Bid) * (1 - ExecutableP) + _Ask * ExecutableP ;
                }
                

                cnt++;
                //Console.WriteLine("{0}: {1}", cnt, string.Join(",", dr.ItemArray.Select(field => field.ToString())));
                dbm.WriteLine(clientID,string.Format("\r {0:0%} Done", ((double)cnt) / outputDataTable.Rows.Count),false);
            }
            dbm.WriteLine(clientID, "", true);
        }

        private void UpdateDashboard()
        {
            dbm.ChangeCellColor(clientID, Color.Orange);
        }

        public void FillInData(bool IsFillinMargin = true)
        {
            // Obtain Consolidate require margin
            if (IsFillinMargin)
            {
                FillInMargin();
            }

            // Get Prices
            FillInPrices();

            //Calculate Price
            CalculatePrice();
        }

        public void disconnect()
        {
            ibClient.eDisconnect();
        }
        #endregion
    }
}
