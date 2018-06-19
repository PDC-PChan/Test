using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;
using System.Collections.Concurrent;

namespace ConsoleApplication4
{

    public class CompleteTick_Struct
    {
        private ConcurrentDictionary<string, TickClass_Parent> TickDict = new ConcurrentDictionary<string, TickClass_Parent>();

        public void AddToDictionary(TickClass_Parent _tick_Structure)
        {
            if (TickDict.ContainsKey(TickType.getField(_tick_Structure.field)))
            {
                TickDict[TickType.getField(_tick_Structure.field)] =  _tick_Structure;    
            }else
            {
                TickDict.AddOrUpdate(TickType.getField(_tick_Structure.field), _tick_Structure, (k, v) => _tick_Structure);
            }
        }

        public ConcurrentDictionary<string, TickClass_Parent> TickDictionary
        {
            get { return TickDict; }
        }

        public double getTick(int[] TickPref)
        {
            int[] bidOrder = TickPref;
            double tickValue = -1;
            TickClass_Parent resultTick;

            for (int i = 0; i < bidOrder.Length; i++)
            {
                if (this.TickDictionary.TryGetValue(TickType.getField(bidOrder[i]), out resultTick))
                {
                    tickValue = ((tickPrice_Struct)resultTick).price;
                    if (tickValue > 0)
                    {
                        break;
                    }
                    else
                    {
                        tickValue = -1;
                    }
                    
                }
            }

            return tickValue;
        }
    }

    public class CompleteTicksMonitor
    {
        private ConcurrentDictionary<int, bool> MonitorDict;
        private ConcurrentDictionary<int, CompleteTick_Struct> completeTickStructs;

        public CompleteTicksMonitor(ConcurrentDictionary<int,CompleteTick_Struct> _completeTickStructs)
        {
            completeTickStructs = _completeTickStructs;

            MonitorDict = new ConcurrentDictionary<int, bool>();

            foreach (int key in completeTickStructs.Keys)
            {
                MonitorDict.TryAdd(key, false);
            }
        }

        public void MarkCompleted(int i)
        {
            MonitorDict[i] = true;
        }

        public void ResetMonitor()
        {
            MonitorDict.Clear();
        }

        public bool AllComplete()
        {
            bool allComplete = true;
            foreach (int key in MonitorDict.Keys)
            {
                if (!MonitorDict[key])
                {
                    allComplete = false;
                    break;
                }
            }
            return allComplete;
        }

        public int[] GetIncompleteList()
        {
            List<int> inComplete = new List<int>();
            foreach (var key in MonitorDict.Keys)
            {
                if (!MonitorDict[key])
                {
                    inComplete.Add(key);
                }
            }
            return (inComplete).ToArray();
        }
    }

    public class TickClass_Parent
    {
        public int tickerId;
        public int field;

        public TickClass_Parent(int _tickerId, int _field)
        {
            tickerId = _tickerId;
            field = _field;
        }
    }

    public class tickPrice_Struct : TickClass_Parent
    {
        public double price;
        public int canAutoExecute;

        public tickPrice_Struct(int _tickerId, int _field, double _price, int _canAutoExecute)
            : base(_tickerId, _field)
        {
            price = _price;
            canAutoExecute = _canAutoExecute;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} : {2} ({3})", tickerId, field, price,canAutoExecute);
        }
    }

    public class tickSize_Struct : TickClass_Parent
    {
        public int size;

        public tickSize_Struct(int _tickerId, int _field, int _size)
            : base(_tickerId, _field)
        {
            size = _size;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} : {2} ", tickerId, field, size);
        }
    }

    public class tickGeneric_Struct
    {
        public int tickerId;
        public int tickType;
        public double value;

        public tickGeneric_Struct(int _tickerId, int _tickType, double _value)
        {
            tickerId = _tickerId;
            tickType = _tickType;
            value = _value;
        }
    }

    public class tickString_Struct
    {
        public int tickerId;
        public int field;
        public string value;

        public tickString_Struct(int _tickerId, int _field, string _value)
        {
            tickerId = _tickerId;
            field = _field;
            value = _value;
        }
    }

    public class OptionComputation_Struct
    {
        public int tickerId;
        public int field;
        public double impliedVol;
        public double delta;
        public double optPrice;
        public double pvDividend;
        public double gamma;
        public double vega;
        public double theta;
        public double undPrice;

        public OptionComputation_Struct(int _tickerId, int _field, double _impliedVol, double _delta, double _optPrice, double _pvDividend,
            double _gamma, double _vega, double _theta, double _undPrice)
        {
            tickerId = _tickerId;
            field = _field;
            impliedVol = _impliedVol;
            delta = _delta;
            optPrice = _optPrice;
            pvDividend = _pvDividend;
            gamma = _gamma;
            vega = _vega;
            theta = _theta;
            undPrice = _undPrice;

        }
    }

    public class OpenOrder_Struct
    {
        public int orderId;
        public Contract contract;
        public Order order;
        public OrderState orderState;

        public OpenOrder_Struct(int _orderId, Contract _contract, Order _order, OrderState _orderState)
        {
            orderId = _orderId;
            contract = _contract;
            order = _order;
            orderState = _orderState;
        }
    }

    public class OrderStatus_Struct
    {
        public int orderId;
        public string status;
        public double filled;
        public double remaining;
        public double avgFillPrice;
        public int permId;
        public int parentId;
        public double lastFillPrice;
        public int clientId;
        public String whyHeld;
        public bool completed;

        public OrderStatus_Struct(int _orderId, string _status, double _filled, double _remaining, double _avgFillPrice, int _permId,
            int _parentId, double _lastFillPrice, int _clientId, String _whyHeld) //int orderId, Contract contract, Order order, OrderState orderState 
        {
            orderId = _orderId;
            status = _status;
            filled = _filled;
            remaining = _remaining;
            avgFillPrice = _avgFillPrice;
            permId = _permId;
            parentId = _parentId;
            lastFillPrice = _lastFillPrice;
            clientId = _clientId;
            whyHeld = _whyHeld;
            completed = true;
        }
    }

    public class OrderAndStatus_Struct
    {
        public List<OpenOrder_Struct> openOrder_Struct;
        public OrderStatus_Struct orderStatus_Struct;

        public OrderAndStatus_Struct(List<OpenOrder_Struct> _openOrder_Struct, OrderStatus_Struct _orderStatus_Struct)
        {
            openOrder_Struct = _openOrder_Struct;
            orderStatus_Struct = _orderStatus_Struct;
        }
    }

    public class AccountValue_Struct
    {
        public string key;
        public string value;
        public string currency;
        public string accountName;

        public AccountValue_Struct(string _key, string _value, string _currency, string _accountName)
        {
            key = _key;
            value = _value;
            currency = _currency;
            accountName = _accountName;
        }
    }

    public class Portfolio_Struct
    {
        public Contract contract;
        public double position;
        public double marketPrice;
        public double marketValue;
        public double averageCost;
        public double unrealizedPNL;
        public double realizedPNL;
        public string accountName;

        public Portfolio_Struct(Contract _contract, double _position, double _marketPrice, double _marketValue,
            double _averageCost, double _unrealizedPNL, double _realizedPNL, string _accountName)
        {
            contract = _contract;
            position = _position;
            marketPrice = _marketPrice;
            marketValue = _marketValue;
            averageCost = _averageCost;
            unrealizedPNL = _unrealizedPNL;
            realizedPNL = _realizedPNL;
            accountName = _accountName;
        }
    }

    public class AccountValueAndPortfolio_Struct
    {
        public string AccountTime;
        public List<AccountValue_Struct> accountValue_Struct_List;
        public Portfolio_Struct portfolio_Struct;

        public AccountValueAndPortfolio_Struct(string _AccountTime, List<AccountValue_Struct> _accountValue_Struct_List, Portfolio_Struct _portfolio_Struct)
        {
            AccountTime = _AccountTime;
            accountValue_Struct_List = _accountValue_Struct_List;
            portfolio_Struct = _portfolio_Struct;
        }
    }

    public class AccountSummary_Struct
    {
        public int reqId;
        public string account;
        public string tag;
        public string value;
        public string currency;

        public AccountSummary_Struct(int _reqId, string _account, string _tag, string _value, string _currency)
        {
            reqId = _reqId;
            account = _account;
            tag = _tag;
            value = _value;
            currency = _currency;
        }
    }

    public class Position_Struct
    {
        public string account;
        public Contract contract;
        public double pos;
        public double avgCost;

        public Position_Struct(string _account, Contract _contract, double _pos, double _avgCost)
        {
            account = _account;
            contract = _contract;
            pos = _pos;
            avgCost = _avgCost;
        }

        public string ToString(bool IsOption)
        {
            if (IsOption)
            {
                return (string.Format("Name: {0} {1} {2:N1} {3,-5};Position: {4,-3};Cost: {5:N2}", 
                    contract.Symbol, 
                    contract.LastTradeDateOrContractMonth, 
                    contract.Strike,
                    (contract.Right == "P" ? "PUT": "CALL"), 
                    pos, 
                    avgCost));
            }
            else
            {
                return this.ToString();
            }
            
        }
    }

    public class ExecDetails_Struct
    {
        public int reqId;
        public Contract contract;
        public Execution execution;

        public ExecDetails_Struct(int _reqId, Contract _contract, Execution _execution)
        {
            reqId = _reqId;
            contract = _contract;
            execution = _execution;
        }
    }

    public class ExecDeatilAndCommReport_Struct
    {
        public List<ExecDetails_Struct> execDetails_Struct_List;
        public CommissionReport commissionReport;

        public ExecDeatilAndCommReport_Struct(List<ExecDetails_Struct> _execDetails_Struct_List, CommissionReport _commissionReport)
        {
            execDetails_Struct_List = _execDetails_Struct_List;
            commissionReport = _commissionReport;
        }
    }

    public class ContractDetails_Struct
    {
        public int ReqId;
        public ContractDetails contractDetails;

        public ContractDetails_Struct(int _ReqId, ContractDetails _contractDetails)
        {
            ReqId = _ReqId;
            contractDetails = _contractDetails;
        }
    }

    public class MktDepth_Struct
    {
        public int tickerId;
        public int position;
        public int operation;
        public int side;
        public double price;
        public int size;

        public MktDepth_Struct(int _tickerId, int _position, int _operation, int _side, double _price, int _size)
        {
            tickerId = _tickerId;
            position = _position;
            operation = _operation;
            side = _side;
            price = _price;
            size = _size;
        }
    }

    public class MktDepth_StructL2
    {
        public int tickerId;
        public int position;
        public string marketMaker;
        public int operation;
        public int side;
        public double price;
        public int size;

        public MktDepth_StructL2(int _tickerId, int _position, string _marketMaker, int _operation,
            int _side, double _price, int _size
)
        {
            tickerId = _tickerId;
            position = _position;
            marketMaker = _marketMaker;
            operation = _operation;
            side = _side;
            price = _price;
            size = _size;

        }
    }

    public class MktDepthCombined_Struct
    {
        public MktDepth_Struct mktDepth_Struct;
        public MktDepth_StructL2 mktDepth_StructL2;

        public MktDepthCombined_Struct(MktDepth_Struct _mktDepth_Struct, MktDepth_StructL2 _mktDepth_StructL2)
        {
            mktDepth_Struct = _mktDepth_Struct;
            mktDepth_StructL2 = _mktDepth_StructL2;
        }
    }

    public class scannerData_Struct
    {
        public int reqId;
        public int rank;
        public ContractDetails contractDetails;
        public string distance;
        public string benchmark;
        public string projection;
        public string legsStr;

        public scannerData_Struct(int _reqId, int _rank, ContractDetails _contractDetails, string _distance,
            string _benchmark, string _projection, string _legsStr)
        {
            reqId = _reqId;
            rank = _rank;
            contractDetails = _contractDetails;
            distance = _distance;
            benchmark = _benchmark;
            projection = _projection;
            legsStr = _legsStr;

        }
    }

    public class historcalData_Struct
    {
        public int reqId;
        public string date;
        public double open;
        public double high;
        public double low;
        public double close;
        public int volume;
        public int count;
        public double WAP;
        public bool hasGaps;

        public historcalData_Struct(int _reqId, string _date, double _open, double _high,
            double _low, double _close, int _volume, int _count, double _WAP, bool _hasGaps)
        {
            reqId = _reqId;
            date = _date;
            open = _open;
            high = _high;
            low = _low;
            close = _close;
            volume = _volume;
            count = _count;
            WAP = _WAP;
            hasGaps = _hasGaps;
        }
    }

    public class NewsBulletin_Struct
    {
        public int msgId;
        public int msgType;
        public String message;
        public String origExchange;

        public NewsBulletin_Struct(int _msgId, int _msgType, String _message, String _origExchange)
        {
            msgId = _msgId;
            msgType = _msgType;
            message = _message;
            origExchange = _origExchange;
        }
    }

    public class ManagedAccount_Struct
    {
        public string accountsList;

        public ManagedAccount_Struct(string _accountsList)
        {
            accountsList = _accountsList;
        }
    }

    public class FA_Struct
    {
        public int faDataType;
        public string faXmlData;

        public FA_Struct(int _faDataType, string _faXmlData)
        {
            faDataType = _faDataType;
            faXmlData = _faXmlData;
        }
    }

    public class RealTimeBars_Struct
    {
        public int reqId;
        public long time;
        public double open;
        public double high;
        public double low;
        public double close;
        public long volume;
        public double WAP;
        public int count;

        public RealTimeBars_Struct(int _reqId,
        long _time,
        double _open,
        double _high,
        double _low,
        double _close,
        long _volume,
        double _WAP,
        int _count
        )
        {
            reqId = _reqId;
            time = _time;
            open = _open;
            high = _high;
            low = _low;
            close = _close;
            volume = _volume;
            WAP = _WAP;
            count = _count;
        }
    }

    public class FundamentalData_Struct
    {
        public int reqId;
        public string data;

        public FundamentalData_Struct(int _reqId, string _data)
        {
            reqId = _reqId;
            data = _data;
        }
    }

    public class DisplayGroup_Struct
    {
        public int reqId;
        public string groups;

        public DisplayGroup_Struct(int _reqId, string _groups)
        {
            reqId = _reqId;
            groups = _groups;
        }
    }

    public class DisplayGroupUpdate_Struct
    {
        public int reqId;
        public string contractInfo;

        public DisplayGroupUpdate_Struct(int _reqId, string _contractInfo)
        {
            reqId = _reqId;
            contractInfo = _contractInfo;
        }
    }

    public class PositionMulti_Struct
    {
        public int requestId;
        public string account;
        public string modelCode;
        public Contract contract;
        public double pos;
        public double avgCost;

        public PositionMulti_Struct(int _requestId, string _account, string _modelCode, Contract _contract, double _pos, double _avgCost)
        {
            requestId = _requestId;
            account = _account;
            modelCode = _modelCode;
            contract = _contract;
            pos = _pos;
            avgCost = _avgCost;
        }
    }

    public class AccountUpdateMulti_Struct
    {
        public int requestId;
        public string account;
        public string modelCode;
        public string key;
        public string value;
        public string currency;

        public AccountUpdateMulti_Struct(int _requestId, string _account, string _modelCode, string _key, string _value, string _currency)
        {
            requestId = _requestId;
            account = _account;
            modelCode = _modelCode;
            key = _key;
            value = _value;
            currency = _currency;
        }
    }

    public class securityDefinitionOptionParameter_Struct
    {
        public int reqId;
        public string exchange;
        public int underlyingConId;
        public string tradingClass;
        public string multiplier;
        public HashSet<string> expirations;
        public HashSet<double> strikes;

        public securityDefinitionOptionParameter_Struct(int _reqId, string _exchange, int _underlyingConId, string _tradingClass, string _multiplier, HashSet<string> _expirations, HashSet<double> _strikes)
        {
            reqId = _reqId;
            exchange = _exchange;
            underlyingConId = _underlyingConId;
            tradingClass = _tradingClass;
            multiplier = _multiplier;
            expirations = _expirations;
            strikes = _strikes;
        }
    }

    public struct ExpiryStrikePair
    {
        public string Expiry;
        public double Strike;

        public ExpiryStrikePair(string _Expiry,double _Strike)
        {
            Expiry = _Expiry;
            Strike = _Strike;
        }
    }
}
