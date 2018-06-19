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
    public class EWrapperImpl : EWrapper
    {
        #region Variables

        //########################################################
        //      Variables
        //########################################################
        
        // Public
        public readonly EReaderSignal signal;

        // Private
        private EClientSocket clientSocket;

        #region Connection and Server
        public int order_ID;
        public long this_CurrentTime;
        public bool connectionClosedCheck = false;
        #endregion

        #region Market Data
        public tickPrice_Struct this_tickPrice_Struct;
        public tickSize_Struct this_tickSize_Struct;
        public CompleteTick_Struct this_CompleteTick_Struct = new CompleteTick_Struct();
        public ConcurrentDictionary<int, CompleteTick_Struct> this_CompleteTick_Structs = new ConcurrentDictionary<int, CompleteTick_Struct>();
        public CompleteTicksMonitor this_CompleteTicksMonitor;
        public tickGeneric_Struct this_tickGeneric_Struct;
        public tickString_Struct this_tickString_Struct;
        public OptionComputation_Struct this_OptionComputation_Struct;
        public OrderAndStatus_Struct this_orderAndStatus;
        public List<OpenOrder_Struct> this_OpenOrder_List = new List<OpenOrder_Struct>();
        public OrderStatus_Struct tmp_OrderStatus;
        public bool OrderAndStatus_Active = true;
        public bool orderPassThrough = false;
        public bool DuplicateOrderId = false;
        public bool ProcessRequestFail = false;
        public bool SnapShotActive = true;
        public bool toResend = false;
        #endregion

        #region Account and Portfolio
        public List<AccountValue_Struct> this_AccountValue_List = new List<AccountValue_Struct>();
        public bool AccountValueActive = true;
        public string this_AccountTime;
        public Portfolio_Struct this_portfolio_Struct;
        public bool AccountSummary_Active = true;
        public List<AccountSummary_Struct> this_AccountSummary_List = new List<AccountSummary_Struct>();
        public List<AccountUpdateMulti_Struct> this_AccountUpdateMulti_List = new List<AccountUpdateMulti_Struct>();
        public bool AccountUpdateMultiActive = true;

        public List<Position_Struct> this_position_List = new List<Position_Struct>();
        public bool Position_Active = true;
        public List<PositionMulti_Struct> this_positionMultiList = new List<PositionMulti_Struct>();
        public bool PositionMultiActive = true;
        #endregion

        #region Executions
        public List<ExecDetails_Struct> this_ExecDetails_List = new List<ExecDetails_Struct>();
        public bool ExecDetailsActive = true;
        public CommissionReport this_CommissionReport;
        #endregion

        #region Contract Details
        public List<ContractDetails_Struct> this_ContractDetails_Struct_List = new List<ContractDetails_Struct>();
        public bool ContractDetailsActive = true;
        public bool ContractNotFound = false;
        public List<securityDefinitionOptionParameter_Struct> this_SecDefOptionPara_List = new List<securityDefinitionOptionParameter_Struct>();
        public bool SecDefOptionParaActive = true;
        #endregion

        #region Market Depth
        public MktDepth_Struct this_MktDepth_Struct;
        public MktDepth_StructL2 this_MktDepth_StructL2;
        #endregion

        #region Market Scanners
        public string this_ScannerParameters;
        public List<scannerData_Struct> this_ScannerData_List = new List<scannerData_Struct>();
        public bool scannerDataActive = true;
        #endregion

        #region Historical Data
        public List<historcalData_Struct> this_HistoricalData_List = new List<historcalData_Struct>();
        public bool historicalDataActive = true;
        #endregion

        #region News Bulletins
        public NewsBulletin_Struct this_NewsBulletin;
        #endregion

        #region Financial Advisors
        public ManagedAccount_Struct this_ManagedAccount_Struct;
        public FA_Struct this_FA_Struct;
        #endregion

        #region Real Time Bars
        public RealTimeBars_Struct this_RealTimeBars;
        #endregion

        #region Fundamental Data
        public FundamentalData_Struct this_FundamentalData;
        #endregion

        #region Display Groups
        public DisplayGroup_Struct this_DisplayGroup;
        public DisplayGroupUpdate_Struct this_DisplayGroupUpdates;
        #endregion

        #endregion
        


        //########################################################
        //      Initialization and Settings
        //########################################################
        #region Initialization
        public EWrapperImpl()
        {
            signal = new EReaderMonitorSignal();
            clientSocket = new EClientSocket(this, signal);
        }

        public EClientSocket ClientSocket
        {
            get { return clientSocket; }
            set { clientSocket = value; }
        }
        #endregion


        //########################################################
        //      Errors Settings
        //########################################################
        #region Errors Settings
        public virtual void error(Exception e)
        {
            Console.WriteLine("API ERROR:" + e.Message);
        }

        public virtual void error(string str)
        {
            Console.WriteLine("ERROR: " + str);
        }

        public virtual void error(int id, int errorCode, string errorMsg)
        {
            bool displayMsg = true;
            // Specific error handling
            switch (errorCode)
            {
                case 200:
                    ContractNotFound = true;
                    displayMsg = false;
                    break;
                case 103:
                    DuplicateOrderId = true;
                    displayMsg = false;
                    break;
                case 322:
                    ProcessRequestFail = true;
                    displayMsg = false;
                    break;
                case 10090:
                    displayMsg = false;
                    break;
                case 10167: //Data Subscription - Delayed Data is used
                    displayMsg = false;
                    break;
                case 354: //Data Subscription - Delayed Data is used
                    displayMsg = false;
                    break;
                case 2108:
                    toResend = true;
                    break;    
                default:
                    break;
            }

            if (displayMsg)
            {
                Console.WriteLine("TWS ERROR: Error id: {0}, Error Code: {1}, Error Message: {2}", id, errorCode, errorMsg);
            }

        }
        #endregion



        //########################################################
        //      Connection and Server
        //########################################################
        #region Connection and Server
        public virtual void currentTime(long time)
        {
            this_CurrentTime = time;
        }

        public virtual void connectionClosed()
        {
            connectionClosedCheck = true;
            Console.WriteLine("API Disconnected.");
        }

        public virtual void connectAck()
        {
            Console.WriteLine("Connection Successful!");
        }
        #endregion



        //########################################################
        //      Market Data
        //########################################################
        #region Market Data
        public virtual void marketDataType(int _reqId, int _marketDataType)
        {
            //Console.WriteLine("EWrapper.marketDataType Not Implemented");
        }

        public virtual void tickPrice(int _tickerId, int _field, double _price, int _canAutoExecute)
        {
            //Console.WriteLine(_field + " : " + _price);
            //this_CompleteTick_Struct.AddToDictionary(new tickPrice_Struct(_tickerId, _field, _price, _canAutoExecute));
            this_CompleteTick_Structs[_tickerId].AddToDictionary(new tickPrice_Struct(_tickerId, _field, _price, _canAutoExecute));

        }

        public virtual void tickSize(int _tickerId, int _field, int _size)
        {
            //Console.WriteLine(_field + " : " + _size);
            this_CompleteTick_Struct.AddToDictionary(new tickSize_Struct(_tickerId, _field, _size));
            this_CompleteTick_Structs[_tickerId].AddToDictionary(new tickSize_Struct(_tickerId, _field, _size));
        }

        public virtual void tickGeneric(int _tickerId, int _tickType, double _value)
        {
            //this_tickGeneric_Structure = new tickGeneric_Structure(_tickerId, _tickType, _value);
            //Console.WriteLine("EWrapper.TickGeneric not implemented : {0},  {1},  {2}", _tickerId,_tickType,_value);
        }

        public virtual void tickString(int _tickerId, int _field, string _value)
        {
            //this_tickString_Structure = new tickString_Structure(_tickerId, _field, _value);
            //Console.WriteLine("EWrapper.tickString Not Implemented: {0},  {1},  {2}", _tickerId, _field, _value);
        }

        public virtual void tickOptionComputation(int _tickerId, int _field, double _impliedVol, double _delta, double _optPrice,
            double _pvDividend, double _gamma, double _vega, double _theta, double _undPrice)
        {
            this_OptionComputation_Struct = new OptionComputation_Struct(_tickerId, _field, _impliedVol, _delta, _optPrice, _pvDividend, _gamma, _vega, _theta, _undPrice);
        }

        public virtual void tickSnapshotEnd(int _tickerId)
        {
            //SnapShotActive = false;
            this_CompleteTicksMonitor.MarkCompleted(_tickerId);
        }

        public virtual void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
        {
            Console.WriteLine("tickEFP not implemented");
        }

        public virtual void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            Console.WriteLine("deltaNeutralValidation not implemented");
        }
        #endregion



        //########################################################
        //      Orders
        //########################################################
        #region Orders

        public virtual void nextValidId(int _orderId)
        {
            order_ID = _orderId;
        }

        public virtual void openOrder(int _orderId, Contract _contract, Order _order, OrderState _orderState)
        {
            this_OpenOrder_List.Add(new OpenOrder_Struct(_orderId, _contract, _order, _orderState));
            orderPassThrough = true;
        }

        public virtual void orderStatus(int _orderId, string _status, double _filled, double _remaining, double _avgFillPrice,
            int _permId, int _parentId, double _lastFillPrice, int _clientId, string _whyHeld)
        {
            tmp_OrderStatus = new OrderStatus_Struct(_orderId, _status, _filled, _remaining, _avgFillPrice,
            _permId, _parentId, _lastFillPrice, _clientId, _whyHeld);
            checkOrderAndStatus();
        }

        private void checkOrderAndStatus()
        {
            if (tmp_OrderStatus.completed)
            {
                this_orderAndStatus=new OrderAndStatus_Struct(this_OpenOrder_List, tmp_OrderStatus);
                tmp_OrderStatus.completed = false;
                OrderAndStatus_Active = true;
            }
        }

        public virtual void openOrderEnd()
        {
            OrderAndStatus_Active = false;
        }

        #endregion



        //########################################################
        //      Account and Portfolio
        //########################################################
        #region Account and Portfolio

        public virtual void updateAccountValue(string key, string value, string currency, string accountName)
        {
            this_AccountValue_List.Add(new AccountValue_Struct(key, value, currency, accountName));
        }

        public virtual void accountDownloadEnd(string account)
        {
            AccountValueActive = false;
        }

        public virtual void updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName)
        {
            this_portfolio_Struct = new Portfolio_Struct(contract, position, marketPrice, marketValue, averageCost, unrealizedPNL, realizedPNL, accountName);
        }

        public virtual void updateAccountTime(string timestamp)
        {
            this_AccountTime = timestamp;
        }

        public virtual void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            this_AccountSummary_List.Add(new AccountSummary_Struct(reqId, account, tag, value, currency));
        }

        public virtual void accountSummaryEnd(int reqId)
        {
            AccountSummary_Active = false;
        }

        public virtual void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
        {
            this_AccountUpdateMulti_List.Add(new AccountUpdateMulti_Struct(requestId, account, modelCode, key, value, currency));
        }

        public virtual void accountUpdateMultiEnd(int requestId)
        {
            AccountUpdateMultiActive = false;
        }

        public virtual void positionEnd()
        {
            Position_Active = false;
        }

        public virtual void position(string account, Contract contract, double pos, double avgCost)
        {
            this_position_List.Add(new Position_Struct(account, contract, pos, avgCost));
        }

        public virtual void positionMulti(int requestId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            this_positionMultiList.Add(new PositionMulti_Struct(requestId, account, modelCode, contract, pos, avgCost));
        }

        public virtual void positionMultiEnd(int requestId)
        {
            PositionMultiActive = false;
        }

        #endregion



        //########################################################
        //      Executions
        //########################################################
        #region Executions

        public virtual void execDetailsEnd(int reqId)
        {
            ExecDetailsActive = false;
        }

        public virtual void commissionReport(CommissionReport commissionReport)
        {
            this_CommissionReport = commissionReport;
        }

        public virtual void execDetails(int reqId, Contract contract, Execution execution)
        {
            this_ExecDetails_List.Add(new ExecDetails_Struct(reqId, contract, execution));
        }
        #endregion




        //########################################################
        //      Contract Details
        //########################################################
        #region Contract Details

        public virtual void contractDetails(int ReqId, ContractDetails contractDetails)
        {
            ContractNotFound = false;
            this_ContractDetails_Struct_List.Add(new ContractDetails_Struct(ReqId, contractDetails));
        }

        public virtual void contractDetailsEnd(int reqId)
        {
            ContractDetailsActive = false;
        }

        public virtual void bondContractDetails(int reqId, ContractDetails contract)
        {
            Console.WriteLine("bondContractDetails Not Implemented");
        }

        public virtual void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            this_SecDefOptionPara_List.Add(new securityDefinitionOptionParameter_Struct(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes));
        }

        public virtual void securityDefinitionOptionParameterEnd(int reqId)
        {
            SecDefOptionParaActive = false;
        }

        public virtual void softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            Console.WriteLine("softDollarTiers Not Implemented");
        }
        #endregion


        //########################################################
        //      Market Depth
        //########################################################
        #region Market Depth

        public virtual void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            this_MktDepth_Struct = new MktDepth_Struct(tickerId, position, operation, side, price, size);
        }

        public virtual void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            this_MktDepth_StructL2 = new MktDepth_StructL2(tickerId, position, marketMaker, operation, side, price, size);
        }
        #endregion

        //########################################################
        //      News Bulletins
        //########################################################
        #region News Bulletins
        public virtual void updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
            this_NewsBulletin = new NewsBulletin_Struct(msgId, msgType, message, origExchange);
        }
        #endregion


        //########################################################
        //      Financial Advisors
        //########################################################
        #region Financial Advisors

        public virtual void managedAccounts(string accountsList)
        {
            this_ManagedAccount_Struct = new ManagedAccount_Struct(accountsList);
        }

        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            this_FA_Struct = new FA_Struct(faDataType, faXmlData);
        }

        #endregion

        //########################################################
        //      Market Scanners
        //########################################################
        #region Market Scanners
        public virtual void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            this_ScannerData_List.Add(new scannerData_Struct(reqId, rank, contractDetails, distance, benchmark, projection, legsStr));
        }

        public virtual void scannerDataEnd(int reqId)
        {
            scannerDataActive = false;
        }

        public virtual void scannerParameters(string xml)
        {
            this_ScannerParameters = xml;
        }

        #endregion


        //########################################################
        //      Historical Data
        //########################################################
        #region Historical Data

        public virtual void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
            this_HistoricalData_List.Add(new historcalData_Struct(reqId, date, open, high, low, close, volume, count, WAP, hasGaps));
        }

        public virtual void historicalDataEnd(int reqId, string start, string end)
        {
            historicalDataActive = false;
        }
        #endregion


        //########################################################
        //      Real Time Bars
        //########################################################
        #region Real Time Bars

        public virtual void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            this_RealTimeBars = new RealTimeBars_Struct(reqId, time, open, high, low, close, volume, WAP, count);
        }
        #endregion


        //########################################################
        //      Fundamental Data
        //######################################################## 
        #region Fundamental Data

        public virtual void fundamentalData(int reqId, string data)
        {
            this_FundamentalData = new FundamentalData_Struct(reqId, data);
        }
        #endregion

        //########################################################
        //      Display Groups
        //######################################################## 
        #region Display Groups

        public virtual void displayGroupList(int reqId, string groups)
        {
            this_DisplayGroup = new DisplayGroup_Struct(reqId, groups);
        }

        public virtual void displayGroupUpdated(int reqId, string contractInfo)
        {
            this_DisplayGroupUpdates = new DisplayGroupUpdate_Struct(reqId, contractInfo);
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
