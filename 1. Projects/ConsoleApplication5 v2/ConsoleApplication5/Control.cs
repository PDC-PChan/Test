using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    public class Control
    {
        public static int connectionPort = 4002;
        public static DateTime simulationEndDate = DateTime.Today.AddMonths(4);
        public static int maxNumThreads = 8;
        public static string ResultOutputFolder = @"C:\Users\Samuel\Documents\dfkjdf\";
        public static string ErrorLogAddress = @"C:\Users\Samuel\Documents\Visual Studio 2013\Projects\ConsoleApplication5 v2\Error Log\";
        public static string UnderContructionConfig = ResultOutputFolder + @"0.VolAnalysis\UnderConfig\Config.txt";
    }

    public class RCodeControl
    {
        public static string RequireFilesAddress = @"C:\Users\Samuel\Documents\Visual Studio 2013\Projects\ConsoleApplication5\R Codes\R_RequireFiles.R".Replace("\\","/");
        public static string DataFilesAddress = @"C:\Users\Samuel\Documents\dfkjdf\".Replace("\\", "/");
        public static string SimulationAddress = @"C:\Users\Samuel\Documents\dfkjdf\0.VolAnalysis\gfs\gf - Copy.R".Replace("\\", "/");
    }

    public class DataMgrCon
    {
        public static string[] colNames = new string[] { "Expiry", "Strike", "ReqMar", "Bid", "Ask", "Last/Close", "Premium" };
        public static Type[] colTypes = new Type[] { typeof(long), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };
        public static string[] FormatCode = new string[] { "D8", "N1", "N2", "N2", "N2", "N2", "N2" };
    }

    public class SimuMgrCon
    {
        public static string[] colNames = new string[] { "E[PnL]", "S[PnL]", "Sharpe", "Prob(%)", "LGL" };
        public static Type[] colTypes = new Type[] { typeof(double),typeof(double), typeof(double),typeof(double),typeof(double)};
        public static string[] FormatCode = new string[] { "N2", "N2", "N3", "P2", "N2"};
    }

    public class CalMgrCon
    {
        public static string[] colNames = new string[] { "E[Return](%)", "Ann[Return](%)" };
        public static Type[] colTypes = new Type[] {  typeof(double), typeof(double) };
        public static string[] FormatCode = new string[] { "P2", "P2"};
    }
}
