using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;

namespace ConsoleApplication4
{
    public class IntBrokerSocket
    {
        //########################################################
        //      Variables
        //########################################################
        #region Variables
        // Public
        public readonly EReaderSignal signal;

        // Private
        private EClientSocket clientSocket;
        private EWrapperImpl testImpl;
        private int clientId;
        private int connectionPort;
        #endregion


        //########################################################
        //      Initialization and Settings
        //########################################################
        #region Initialization
        public IntBrokerSocket(int _clientId, int _connectionPort)
        {
            clientId = _clientId;
            testImpl = new EWrapperImpl();
            clientSocket = testImpl.ClientSocket;
            connectionPort = _connectionPort;

            // Request permission to connect
            //if (connectionPort == Control.LiveTradingPort)
            //{
            //    Console.Write("You are connecting to the live trading account.  Do you want to continue ? (Y/N)");
            //    if (Console.ReadLine().ToUpper() == "N")
            //    {
            //        Environment.Exit(0);
            //    }
            //}

            // Connect to TWS
            clientSocket.eConnect("127.0.0.1", connectionPort, clientId);
            var reader = new EReader(clientSocket, testImpl.signal);
            reader.Start();
            new Thread(() =>
            {
                while (clientSocket.IsConnected())
                {
                    testImpl.signal.waitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }.Start();
        }
        #endregion



        //########################################################
        //      Connection and Server
        //########################################################
        #region Connection and Server

        public long reqCurrentTime()
        {
            clientSocket.reqCurrentTime();
            while (testImpl.this_CurrentTime == 0) { }
            return testImpl.this_CurrentTime;
        }

        public bool eDisconnect()
        {
            clientSocket.eDisconnect();
            return testImpl.connectionClosedCheck;
        }
        #endregion



        //########################################################
        //      Market Data
        //########################################################
        #region Market Data
        public enum MarketDataType { Live = 1, Frozen, Delayed, DelayedFrozen };

        public void SetMarketDataType(MarketDataType _marketDataType)
        {
            clientSocket.reqMarketDataType((int)_marketDataType);
        }


        //public CompleteTick_Struct GetMktSnapShotData(int _tickerId, Contract contract)
        //{
        //    testImpl.this_CompleteTick_Struct.TickDictionary.Clear();
        //    clientSocket.reqMktData(_tickerId, contract, "", true, new List<TagValue>());
        //    while (testImpl.SnapShotActive) { }
        //    testImpl.SnapShotActive = true;
        //    return testImpl.this_CompleteTick_Struct;
        //}


        public List<CompleteTick_Struct> GetMktSnapShotData(List<Contract> contract)
        {
            int ContractCnt = contract.Count;
            int[] tickerIds = new int[ContractCnt];
            List<CompleteTick_Struct> ResultCompleteStructList = new List<CompleteTick_Struct>();

            // Initialization
            testImpl.this_CompleteTick_Structs.Clear();
            tickerIds = Toolkit.RandomSample(1000, 9999, ContractCnt);
            for (int i = 0; i < ContractCnt; i++)
            {
                testImpl.this_CompleteTick_Structs.TryAdd(tickerIds[i], new CompleteTick_Struct());
            }
            testImpl.this_CompleteTicksMonitor = new CompleteTicksMonitor(testImpl.this_CompleteTick_Structs);


            // Send Request
            for (int i = 0; i < ContractCnt; i++)
            {
                clientSocket.reqMktData(tickerIds[i], contract[i], "", true, new List<TagValue>());
                Thread.Sleep(20);
            }

            // Wait for reply and resend order if necessary
            while (!testImpl.this_CompleteTicksMonitor.AllComplete())
            {
                if (testImpl.toResend)
                {
                    int[] resendList = testImpl.this_CompleteTicksMonitor.GetIncompleteList();
                    for (int i = 0; i < resendList.Length; i++)
                    {
                        clientSocket.reqMktData(resendList[i], contract[Array.IndexOf(tickerIds, resendList[i])], "", true, new List<TagValue>());
                        Thread.Sleep(20);
                    }

                    testImpl.toResend = false;
                }
            }

            testImpl.this_CompleteTicksMonitor.ResetMonitor();


            // Rebuild reply
            for (int i = 0; i < ContractCnt; i++)
            {
                ResultCompleteStructList.Add(testImpl.this_CompleteTick_Structs[tickerIds[i]]);
            }
            return ResultCompleteStructList;
        }


        public OptionComputation_Struct calculateImpliedVolatility(int _reqId, Contract contract, double _optionPrice, double _underPrice)
        {
            clientSocket.calculateImpliedVolatility(_reqId, contract, _optionPrice, _underPrice, new List<TagValue>());

            return (testImpl.this_OptionComputation_Struct);
        }

        public OptionComputation_Struct calculateOptionPrice(int _reqId, Contract contract, double _volatility, double _underPrice)
        {
            clientSocket.calculateOptionPrice(_reqId, contract, _volatility, _underPrice, new List<TagValue>());
            return (testImpl.this_OptionComputation_Struct);
        }

        #endregion


        //########################################################
        //      Orders
        //########################################################
        #region Orders

        public int reqIDs()
        {
            clientSocket.reqIds(1);
            while (testImpl.order_ID == 0) { }
            return testImpl.order_ID;
        }

        public void placeOrder(int _id, Contract _contract, Order _order)
        {
            testImpl.orderPassThrough = false;
            testImpl.DuplicateOrderId = false;
            testImpl.ProcessRequestFail = false;

            clientSocket.placeOrder(_id, _contract, _order);
            while (!testImpl.orderPassThrough)
            {
                if (testImpl.DuplicateOrderId)
                {
                    throw new DuplicatedOrderIdException();
                }
                if (testImpl.ProcessRequestFail)
                {
                    throw new ProcessingRequestException();
                }
            }
        }

        public void cancelOrder(int _orderId)
        {
            clientSocket.cancelOrder(_orderId);
        }


        public OrderAndStatus_Struct reqOpenOrders()
        {
            testImpl.this_orderAndStatus = null;
            clientSocket.reqOpenOrders();
            while (testImpl.this_orderAndStatus == null) { }
            testImpl.OrderAndStatus_Active = true;
            return testImpl.this_orderAndStatus;
        }

        public OrderAndStatus_Struct reqAllOpenOrders()
        {
            testImpl.this_orderAndStatus = null;
            clientSocket.reqAllOpenOrders();
            while (testImpl.this_orderAndStatus == null) { }
            testImpl.OrderAndStatus_Active = true;
            return testImpl.this_orderAndStatus;
        }

        public OrderAndStatus_Struct reqAutoOpenOrders(bool _autoBind)
        {
            testImpl.this_orderAndStatus = null;
            clientSocket.reqAutoOpenOrders(_autoBind);
            while (testImpl.this_orderAndStatus == null) { }
            testImpl.OrderAndStatus_Active = true;
            return testImpl.this_orderAndStatus;
        }

        public double GetRequiredMargin(Contract _contract, string action, long quantity)
        {
            List<OpenOrder_Struct> AllOrders;
            Order marginCheck = new Order();
            int orderID = this.reqIDs();
            double maintMargin0 = 0;
            double maintMargin1 = 0;


            // Check original maintanence margin
            maintMargin0 = Convert.ToDouble(this.reqAccountSummary(100, "All", "MaintMarginReq")[0].value);


            // Set up the order
            marginCheck.Action = action;
            marginCheck.TotalQuantity = quantity;
            marginCheck.OrderType = "MKT";
            marginCheck.WhatIf = true;


            // Place the order
            testImpl.this_OpenOrder_List.Clear();
            try
            {
                do
                {
                    try
                    {
                        this.placeOrder(orderID, _contract, marginCheck);
                    }
                    catch (DuplicatedOrderIdException)
                    {
                        orderID++;
                        Thread.Sleep(5);
                    }

                } while (!testImpl.orderPassThrough);
            }
            catch (ProcessingRequestException e)
            {
                throw e;
            }

            Thread.Sleep(Control.MarginQueryPlaceOrderPause);
            while (testImpl.this_OpenOrder_List.Count == 0) { }
            AllOrders = testImpl.this_OpenOrder_List;


            // Check the order
            foreach (OpenOrder_Struct item in AllOrders)
            {
                if (item.orderId == orderID)
                {
                    maintMargin1 = Convert.ToDouble(item.orderState.MaintMargin);
                    break;
                }
            }

            return maintMargin1 - maintMargin0;
        }


        #endregion


        //########################################################
        //      Account and Portfolio
        //########################################################
        #region Account and Portfolio

        public AccountValueAndPortfolio_Struct reqAccountUpdates(bool subscribe, string acctCode)
        {
            testImpl.this_AccountValue_List.Clear();
            clientSocket.reqAccountUpdates(subscribe, acctCode);
            while (testImpl.AccountValueActive) { }
            testImpl.AccountValueActive = true;
            return new AccountValueAndPortfolio_Struct(testImpl.this_AccountTime, testImpl.this_AccountValue_List, testImpl.this_portfolio_Struct);
        }

        public List<AccountSummary_Struct> reqAccountSummary(int reqId, string group, string tags)
        {
            testImpl.this_AccountSummary_List.Clear();
            clientSocket.reqAccountSummary(reqId, group, tags);
            while (testImpl.AccountSummary_Active) { }
            testImpl.AccountSummary_Active = true;
            return testImpl.this_AccountSummary_List;
        }

        public List<AccountUpdateMulti_Struct> reqAccountUpdatesMulti(int requestId, string account, string modelCode, bool ledgerAndNLV)
        {
            testImpl.this_AccountUpdateMulti_List.Clear();
            clientSocket.reqAccountUpdatesMulti(requestId, account, modelCode, ledgerAndNLV);
            while (testImpl.AccountUpdateMultiActive) { }
            testImpl.AccountUpdateMultiActive = true;
            return (testImpl.this_AccountUpdateMulti_List);
        }

        public List<Position_Struct> reqPositions()
        {
            testImpl.this_position_List.Clear();
            clientSocket.reqPositions();
            while (testImpl.Position_Active) { }
            testImpl.Position_Active = true;
            return testImpl.this_position_List;
        }

        public List<PositionMulti_Struct> reqPositionsMulti(int requestId, string account, string modelCode)
        {
            testImpl.this_positionMultiList.Clear();
            clientSocket.reqPositionsMulti(requestId, account, modelCode);
            while (testImpl.PositionMultiActive) { }
            testImpl.PositionMultiActive = true;
            return testImpl.this_positionMultiList;
        }

        #endregion


        //########################################################
        //      Executions
        //########################################################
        #region Executions
        public ExecDeatilAndCommReport_Struct reqExecutions(int reqId, ExecutionFilter filter)
        {
            testImpl.this_ExecDetails_List.Clear();
            testImpl.this_CommissionReport = null;
            clientSocket.reqExecutions(reqId, filter);
            while (testImpl.ExecDetailsActive || testImpl.this_CommissionReport.Equals(null)) { }
            return (new ExecDeatilAndCommReport_Struct(testImpl.this_ExecDetails_List, testImpl.this_CommissionReport));
        }
        #endregion


        //########################################################
        //      Contract Details
        //########################################################
        #region Contract Details
        public List<ContractDetails_Struct> reqContractDetails(int reqId, Contract contract, bool DelayRequest = false)
        {
            testImpl.this_ContractDetails_Struct_List.Clear();
            clientSocket.reqContractDetails(reqId, contract);
            while (testImpl.ContractDetailsActive && !testImpl.ContractNotFound) { }
            if (DelayRequest)
            {
                Thread.Sleep(Control.optChainReqWaitTime);
            }
            testImpl.ContractDetailsActive = true;
            testImpl.ContractNotFound = false;
            return testImpl.this_ContractDetails_Struct_List;
        }

        public List<securityDefinitionOptionParameter_Struct> reqSecDefOptParams(int reqId, string underlyingSymbol,
            string futFopExchange, string underlyingSecType, int underlyingConId)
        {
            testImpl.this_SecDefOptionPara_List.Clear();
            clientSocket.reqSecDefOptParams(reqId, underlyingSymbol, futFopExchange, underlyingSecType, underlyingConId);
            while (testImpl.SecDefOptionParaActive) { }
            testImpl.SecDefOptionParaActive = true;
            return (testImpl.this_SecDefOptionPara_List);
        }


        public ConcurrentDictionary<ExpiryStrikePair, Contract> getUniqueOptionChain(string underlyingSymbol, string _Right)
        {
            int sameCntStreak = 0;
            int LastListCnt = 0;
            Contract underlying = new Contract();
            Contract ProposedOpt = new Contract();
            HashSet<string> AllExpiry = new HashSet<string>();
            HashSet<double> AllStrikes = new HashSet<double>();
            Contract iOpt = new Contract();
            List<ContractDetails_Struct> returnOptChain;
            List<securityDefinitionOptionParameter_Struct> OptChainByEx;
            securityDefinitionOptionParameter_Struct OptChain;
            ConcurrentDictionary<ExpiryStrikePair, Contract> outputOptionChain = new ConcurrentDictionary<ExpiryStrikePair, Contract>();

            // Get the contract
            underlying.Symbol = underlyingSymbol;
            underlying.SecType = "STK";
            returnOptChain = this.reqContractDetails(Toolkit.RndReqId(), underlying, true);
            underlying = returnOptChain[0].contractDetails.Summary;


            // Get the option chain
            OptChainByEx = reqSecDefOptParams(Toolkit.RndReqId(), underlying.Symbol, "", underlying.SecType, underlying.ConId);

            // Filter option chain

            foreach (securityDefinitionOptionParameter_Struct sdop in OptChainByEx)
            {
                AllExpiry.UnionWith(sdop.expirations);
                AllStrikes.UnionWith(sdop.strikes);
            }

            int cnt = 0;
            int Total = AllExpiry.Count * AllStrikes.Count;

            foreach (double _strike in AllStrikes)
            {
                foreach (string _expiry in AllExpiry)
                {
                    ProposedOpt = new Contract();
                    ProposedOpt.Symbol = underlyingSymbol;
                    ProposedOpt.SecType = "OPT";
                    ProposedOpt.Right = _Right;
                    ProposedOpt.LastTradeDateOrContractMonth = _expiry;
                    //ProposedOpt.Strike = _strike;
                    ProposedOpt.Currency = Control.optionCurrency;

                    returnOptChain = this.reqContractDetails(Toolkit.RndReqId(), ProposedOpt);
                    LastListCnt = outputOptionChain.Count;
                    foreach (ContractDetails_Struct iCDS in returnOptChain)
                    {
                        outputOptionChain.TryAdd(new ExpiryStrikePair(iCDS.contractDetails.Summary.LastTradeDateOrContractMonth,
                                                                        iCDS.contractDetails.Summary.Strike),
                                                iCDS.contractDetails.Summary);
                    }

                    if (LastListCnt == outputOptionChain.Count)
                    {
                        sameCntStreak++;
                    }
                    else
                    {
                        sameCntStreak = 0;
                    }

                    //Thread.Sleep(Control.OptChainQueryPauseTime);

                    cnt++;
                    Console.Write("\r {0:0%} Done", ((double)cnt) / Total);

                    //if (((double)cnt / Total > .5 && sameCntStreak > 40) || sameCntStreak > 100)
                    //{
                    //    Console.WriteLine();
                    //    Console.WriteLine("Saturation reached.");
                    //    goto Saturation;
                    //}
                }
            }

            //Saturation:
            return outputOptionChain;
        }

        #endregion


        //########################################################
        //      Market Depth
        //########################################################
        #region Market Depth

        public MktDepthCombined_Struct reqMarketDepth(int tickerId, Contract contract, int numRows)
        {
            clientSocket.reqMarketDepth(tickerId, contract, numRows, new List<TagValue>());
            return new MktDepthCombined_Struct(testImpl.this_MktDepth_Struct, testImpl.this_MktDepth_StructL2);
        }
        #endregion

        //########################################################
        //      News Bulletins
        //########################################################
        #region News Bulletins
        public NewsBulletin_Struct reqNewsBulletins(bool allMessages)
        {
            clientSocket.reqNewsBulletins(allMessages);
            return testImpl.this_NewsBulletin;
        }
        #endregion


        //########################################################
        //      Financial Advisors
        //########################################################
        #region Financial Advisors
        public ManagedAccount_Struct reqManagedAccts()
        {
            clientSocket.reqManagedAccts();
            return testImpl.this_ManagedAccount_Struct;
        }

        public FA_Struct requestFA(int faDataType)
        {
            clientSocket.requestFA(faDataType);
            return testImpl.this_FA_Struct;
        }

        public FA_Struct replaceFA(int faDataType, String xml)
        {
            clientSocket.replaceFA(faDataType, xml);
            return testImpl.this_FA_Struct;
        }
        #endregion

        //########################################################
        //      Market Scanners
        //########################################################
        #region Market Scanners
        public List<scannerData_Struct> reqScannerSubscription(int reqId, ScannerSubscription subscription)
        {
            testImpl.this_ScannerData_List.Clear();
            clientSocket.reqScannerSubscription(reqId, subscription, new List<TagValue>());
            while (testImpl.scannerDataActive) { }
            return testImpl.this_ScannerData_List;
        }

        public string reqScannerParameters()
        {
            testImpl.this_ScannerParameters = "";
            clientSocket.reqScannerParameters();
            while (testImpl.this_ScannerParameters == "") { }
            return testImpl.this_ScannerParameters;
        }
        #endregion


        //########################################################
        //      Historical Data
        //########################################################
        #region Historical Data
        public List<historcalData_Struct> reqHistoricalData(int tickereId, Contract contract, string endDateTime, string durationString, string barSizeSetting,
            string whatToShow, int useRTH, int formatDate, List<TagValue> chartOptions)
        {
            testImpl.this_HistoricalData_List.Clear();
            clientSocket.reqHistoricalData(tickereId, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRTH, formatDate, chartOptions);
            while (testImpl.historicalDataActive) { }
            testImpl.historicalDataActive = true;
            return testImpl.this_HistoricalData_List;
        }

        #endregion


        //########################################################
        //      Real Time Bars
        //########################################################
        #region Real Time Bars
        public RealTimeBars_Struct reqRealTimeBars(int tickerId, Contract contract, int barSize, string whatToShow, bool useRTH, List<TagValue> realTimeBarOptions)
        {
            testImpl.this_RealTimeBars = null;
            clientSocket.reqRealTimeBars(tickerId, contract, barSize, whatToShow, useRTH, realTimeBarOptions);
            while (testImpl.this_RealTimeBars.Equals(null)) { }
            return testImpl.this_RealTimeBars;
        }
        #endregion


        //########################################################
        //      Fundamental Data
        //######################################################## 
        #region Fundamental Data
        public FundamentalData_Struct reqFundamentalData(int reqId, Contract contract, String reportType)
        {
            testImpl.this_FundamentalData = null;
            clientSocket.reqFundamentalData(reqId, contract, reportType, new List<TagValue>());
            while (testImpl.this_FundamentalData.Equals(null)) { }
            return testImpl.this_FundamentalData;
        }
        #endregion

        //########################################################
        //      Display Groups
        //######################################################## 
        #region Display Groups
        public DisplayGroup_Struct queryDisplayGroups(int reqId)
        {
            testImpl.this_DisplayGroup = null;
            clientSocket.queryDisplayGroups(reqId);
            while (testImpl.this_DisplayGroup.Equals(null)) { }
            return testImpl.this_DisplayGroup;
        }

        public DisplayGroupUpdate_Struct subscribeToGroupEvents(int reqId, int groupId)
        {
            testImpl.this_DisplayGroupUpdates = null;
            clientSocket.subscribeToGroupEvents(reqId, groupId);
            while (testImpl.this_DisplayGroupUpdates.Equals(null)) { }
            return testImpl.this_DisplayGroupUpdates;
        }

        public DisplayGroupUpdate_Struct updateDisplayGroup(int reqId, String contractInfo)
        {
            testImpl.this_DisplayGroupUpdates = null;
            clientSocket.updateDisplayGroup(reqId, contractInfo);
            while (testImpl.this_DisplayGroupUpdates.Equals(null)) { }
            return testImpl.this_DisplayGroupUpdates;
        }
        #endregion

        //########################################################
        //      Deprecated Functions
        //######################################################## 
        #region Deprecated Functions
        public virtual void verifyMessageAPI(string apiData) { }
        public virtual void verifyCompleted(bool isSuccessful, string errorText) { }
        public virtual void verifyAndAuthMessageAPI(string apiData, string xyzChallenge) { }
        public virtual void verifyAndAuthCompleted(bool isSuccessful, string errorText) { }
        #endregion
    }
}
