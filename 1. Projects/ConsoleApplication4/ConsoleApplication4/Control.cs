using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public static class Control
    {
        public static DateTime IBInitialTime = new DateTime(1970, 1, 1);
        public static int LiveTradingPort = 7496; // <<----Do not change this
        public static int OptChainQueryPauseTime = 20;
        public static int MarginQueryPlaceOrderPause = 22;
        public static int optChainReqWaitTime = 2000;
        public static string optionCurrency = "USD";
    }

    public class tickPreference
    {
        //############## Bid Preference ####################
        // 1:BID > 66:DELAYED_BID > 4:LAST> 68:DELAYED_LAST > 9:CLOSE > 75:DELAYED_CLOSE
        public static int[] BidPreference = new int[] { 1, 66, 4, 68, 9, 75 };


        //############## Ask Preference ####################
        // 1:ASK > 66:DELAYED_ASK > 4:LAST> 68:DELAYED_LAST > 9:CLOSE > 75:DELAYED_CLOSE
        public static int[] AskPreference = new int[] { 2, 67, 4, 68, 9, 75 };


        //############## Last Preference ####################
        // 1:ASK > 66:DELAYED_ASK > 4:LAST> 68:DELAYED_LAST > 9:CLOSE > 75:DELAYED_CLOSE
        public static int[] LastPreference = new int[] { 4, 68, 9, 75 };
    }

}
