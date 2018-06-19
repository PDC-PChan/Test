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
using System.Drawing;

namespace ConsoleApplication5
{
    public class CalculationManager_v2
    {

        //##################################################################
        //                  Variables and initialization
        //##################################################################
        #region Variables and initialization
        private SimulationManager this_SimulationManager;
        private ConcurrentQueue<DataManager> DataManagerQueue = new ConcurrentQueue<DataManager>();
        private string right;
        private DataTable OptionTable;
        private int ProcessedCount = 0;
        private string[] symbolList;
        private Dictionary<string, int> clienIDs = new Dictionary<string, int>();
        private string RErrorLog;
        private string ReportOutputMain;

        public CalculationManager_v2(string[] _symbolList, DashboardManager dbm, string _right = "PUT")
        {
            for (int i = 0; i < _symbolList.Length; i++)
            {
                clienIDs.Add(_symbolList[i], i);
            }

            right = _right.ToUpper();
            symbolList = _symbolList;
            ReportOutputMain = Control.ResultOutputFolder + "1.Reports Output\\" + DateTime.Today.ToString("yyyyMMdd");
            RErrorLog = Control.ErrorLogAddress + DateTime.Today.ToString("yyyyMMdd") + ".txt";

            // Create directory and write to log file
            Directory.CreateDirectory(ReportOutputMain);
            using (StreamWriter sw = new StreamWriter(Control.UnderContructionConfig,append:false))
            {
                sw.Write(ReportOutputMain.Replace(@"\\",@"\"));
            }

            Task Cal_MangerLauncher = Task.Run(() =>
            {
                LaunchManager(dbm);
            });
            Task DB_ManagerLauncher = Task.Run(() =>
            {
                dbm.createForm(symbolList);
            });
            Task.WaitAny(Cal_MangerLauncher, DB_ManagerLauncher);
        }

        private void LaunchManager(DashboardManager dbm)
        {
            //############ Data Manager ###################
            Task DataManagerFleet = Task.Run(() =>
            {
                Parallel.ForEach(symbolList, new ParallelOptions { MaxDegreeOfParallelism = Control.maxNumThreads }, (symbol) =>
                {
                    DataManager dm = new DataManager(DashboardMgr:dbm,
                                                _symbol: symbol,
                                               _right: right,
                                               _IncludeOppositeWing: false,
                                               _clientId: clienIDs[symbol],
                                               _ReadInContract: false,
                                               _secType: "STK");
                    dm.CreateTable();
                    dm.FillInData();
                    DataManagerQueue.Enqueue(dm);
                    dm.disconnect();
                });
            });

            Task IntegrationWaiter = Task.Run(() => {
                while (ProcessedCount < symbolList.Length)
                {
                    DataManager iDm;
                    if (DataManagerQueue.TryDequeue(out iDm))
                    {
                        try
                        {
                            Integration(iDm);
                        }
                        catch (Exception)
                        {
                            using (StreamWriter sw = new StreamWriter(RErrorLog, append: true))
                            {
                                sw.WriteLine(iDm.Symbol);
                                Console.WriteLine("Error in {0}", iDm.Symbol);
                            }
                        }
                        Console.WriteLine("{0} Done", iDm.Symbol);
                        dbm.ChangeCellColor(Array.IndexOf(symbolList, iDm.Symbol),Color.Green);
                        ProcessedCount++;
                    }
                }
            });

            Task.WaitAll(DataManagerFleet, IntegrationWaiter);
        }
        #endregion


        //##################################################################
        //                  Filling in
        //##################################################################
        #region Calculation

        private DataRow[] GetRows(long _Expiry)
        {
            string Query = string.Format("{0} = {1}",
                DataMgrCon.colNames[0], _Expiry);
            DataRow[] resultRows = OptionTable.Select(Query);

            return (resultRows);
        }

        private void Others_Fillin()
        {
            HashSet<long> ExpiryList = new HashSet<long>(OptionTable.AsEnumerable().Select(r => r.Field<long>(DataMgrCon.colNames[0])).ToArray());
            int cnt = 0;

            foreach (long _expiry in ExpiryList)
            {
                DataRow[] ExpiryDRs = GetRows(_expiry);
                DateTime expiryDT = DateTime.ParseExact(_expiry.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                this_SimulationManager.GetSimulatedPrices(expiryDT);

                foreach (DataRow dr in ExpiryDRs)
                {
                    double strike = Convert.ToDouble(dr[DataMgrCon.colNames[1]].ToString());
                    double margin = Convert.ToDouble(dr[DataMgrCon.colNames[2]].ToString());
                    double price = Convert.ToDouble(dr[DataMgrCon.colNames[6]].ToString());

                    // E[PnL]
                    dr[SimuMgrCon.colNames[0]] = this_SimulationManager.ExProfit(strike, price, right);
                    // S[PnL
                    dr[SimuMgrCon.colNames[1]] = this_SimulationManager.SDProfit(strike, price, right);
                    // Sharpe
                    dr[SimuMgrCon.colNames[2]] = this_SimulationManager.Sharpe(strike, price, right);
                    // Prob
                    dr[SimuMgrCon.colNames[3]] = this_SimulationManager.Probability_OTM(strike, right);
                    // LGL
                    dr[SimuMgrCon.colNames[4]] = this_SimulationManager.Cond_Loss(strike, right);


                    // E[Return]
                    dr[CalMgrCon.colNames[0]] = this_SimulationManager.ExProfit(strike, price, right) * 100 * 7.8 / margin;
                    // Ann[Return]
                    dr[CalMgrCon.colNames[1]] = this_SimulationManager.ExProfit(strike, price, right) * 100 * 7.8 / margin / ((expiryDT - DateTime.Today).TotalDays / 365.25);


                    cnt++;
                    //Console.WriteLine("{0}: {1}", cnt, string.Join(",", dr.ItemArray.Select(field => field.ToString())));
                    Console.Write("\r {0:0%} Done", ((double)cnt) / OptionTable.Rows.Count);
                }
            }
            Console.WriteLine();
        }

        #endregion

        //##################################################################
        //                  Display
        //##################################################################
        #region Display
        private void Display(DataManager dm)
        {
            StringBuilder sb = new StringBuilder();
            string symbol = dm.Symbol;
            string ResultOutputAddress = ReportOutputMain + "\\" + DateTime.Today.ToString("yyyyMMdd") + "-" + symbol + "-" + right + "-Results.txt";

            // Sort the table
            DataView tmpDV = OptionTable.DefaultView;
            tmpDV.Sort = string.Format("{0} ASC, {1} ASC", DataMgrCon.colNames[0], DataMgrCon.colNames[1]);
            OptionTable = tmpDV.ToTable();

            sb.AppendLine(string.Join(";", dm.ColumnNames));

            // Display Table
            foreach (DataRow dr in OptionTable.Rows)
            {
                sb.AppendLine(string.Join(";", dr.ItemArray));
            }

            using (StreamWriter sw = new StreamWriter(ResultOutputAddress, false))
            {
                sw.Write(sb.ToString());
            }
        }
        #endregion

        //##################################################################
        //                  CleanUp
        //##################################################################
        #region CleanUp
        private void CleanUp()
        {
            this_SimulationManager.CleanUp();
        }
        #endregion


        //##################################################################
        //                  Integration
        //##################################################################
        private void Integration(DataManager dm)
        {
            string symbol = dm.Symbol;

            // Start simulation manager
            this_SimulationManager = new SimulationManager(symbol);
            this_SimulationManager.EvaluateSimulation();

            // Integration
            OptionTable = dm.OptionTable;
            this.Others_Fillin();

            // Display and Cleanup
            Display(dm);
            CleanUp();
        }
    }
}
