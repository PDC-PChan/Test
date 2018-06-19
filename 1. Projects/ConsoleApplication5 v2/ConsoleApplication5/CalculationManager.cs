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

namespace ConsoleApplication5
{
    public class CalculationManager
    {

        //##################################################################
        //                  Variables and initialization
        //##################################################################
        #region Variables and initialization
        private SimulationManager this_SimulationManager;
        private DataManager this_DataManager;
        private string mySymbol;
        private string right;
        private DataTable OptionTable;
        private string ResultOutputAddress;
        private DashboardManager dbm;

        public CalculationManager(int clientId, string _symbol,string _right = "PUT")
        {
            mySymbol = _symbol;
            right = _right.ToUpper();
            ResultOutputAddress = Control.ResultOutputFolder + _symbol + "\\" + DateTime.Today.ToString("yyyyMMdd") + "-" + mySymbol + "-" + right + "-Results.txt";
            this_SimulationManager = new SimulationManager(mySymbol);
            this_DataManager = new DataManager(DashboardMgr: dbm,
                                                _symbol:mySymbol,
                                               _right: right, 
                                               _IncludeOppositeWing: false,
                                               _clientId: clientId,
                                               _ReadInContract: false);
            InitializeManagers();
        }

        public void InitializeManagers()
        {
            //############ Data Manager ###################
            Task DM_Setup = Task.Run(() =>
            {
                this_DataManager.CreateTable();
                this_DataManager.FillInData();
            });

            Task SM_Setup = Task.Run(() =>
            {
                this_SimulationManager.EvaluateSimulation();
            });

            Task.WaitAll(DM_Setup, SM_Setup);

            //############ Simulation Manager ###################
            OptionTable = this_DataManager.OptionTable;
            this.Others_Fillin();
        }
        #endregion


        //##################################################################
        //                  Integration
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
                    dr[SimuMgrCon.colNames[0]] = this_SimulationManager.ExProfit(strike,price,right);
                    // S[PnL
                    dr[SimuMgrCon.colNames[1]] = this_SimulationManager.SDProfit(strike, price, right);
                    // Sharpe
                    dr[SimuMgrCon.colNames[2]] = this_SimulationManager.Sharpe(strike, price, right);
                    // Prob
                    dr[SimuMgrCon.colNames[3]] = this_SimulationManager.Probability_OTM(strike,right);
                    // LGL
                    dr[SimuMgrCon.colNames[4]] = this_SimulationManager.Cond_Loss(strike, right);


                    // E[Return]
                    dr[CalMgrCon.colNames[0]] = this_SimulationManager.ExProfit(strike, price, right) * 100 * 7.8 / margin;
                    // Ann[Return]
                    dr[CalMgrCon.colNames[1]] = this_SimulationManager.ExProfit(strike, price, right) * 100 * 7.8 / margin/ ((expiryDT - DateTime.Today).TotalDays/365.25);


                    cnt++;
                    //Console.WriteLine("{0}: {1}", cnt, string.Join(",", dr.ItemArray.Select(field => field.ToString())));
                    Console.Write("\r {0:0%} Done", ((double)cnt)  / OptionTable.Rows.Count);
                }
            }
            Console.WriteLine();
        }

        #endregion

        //##################################################################
        //                  Display
        //##################################################################
        #region Display
        public void Display()
        {
            StringBuilder sb = new StringBuilder();

            // Sort the table
            DataView tmpDV = OptionTable.DefaultView;
            tmpDV.Sort = string.Format("{0} ASC, {1} ASC", DataMgrCon.colNames[0], DataMgrCon.colNames[1]);
            OptionTable = tmpDV.ToTable();

            sb.AppendLine(string.Join(";", this_DataManager.ColumnNames));

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
        public void CleanUp()
        {
            this_SimulationManager.CleanUp();
        }
        #endregion
    }
}
