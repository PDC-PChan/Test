using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using System.IO;

namespace ConsoleApplication5
{
    public class SimulationManager
    {
        //##################################################################
        //                  Variables and initialization
        //##################################################################
        #region Variables and initialization
        private REngine engine;
        private string this_symbol;

        public string Symbol
        {
            get { return this_symbol; }
            set { this_symbol = value; }
        }

        public void CleanUp() 
        {
            this.engine.Evaluate("rm(list=ls(all=TRUE))");
        }

        public SimulationManager(string _symbol)
        {
            this_symbol = _symbol;
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();

            declarePackages();

            SetWorkingDirectory();
        }

        private void declarePackages()
        {
            engine.Evaluate(string.Format("source('{0}')",RCodeControl.RequireFilesAddress));
        }


        private void SetWorkingDirectory()
        {
            engine.Evaluate(string.Format("setwd('{0}')",RCodeControl.DataFilesAddress + this_symbol));
        }

        public void EvaluateSimulation()
        {
            engine.Evaluate("PrintSimulation = F");
            engine.Evaluate(string.Format("source('{0}')", RCodeControl.SimulationAddress));
        }
        #endregion


        //##################################################################
        //                  Functions useful for simulation
        //##################################################################
        #region User functions
        public double[] GetSimulatedPrices(DateTime Expiry)
        {
            NumericVector Fitted_Expiry_Prices;
            engine.Evaluate(string.Format("ExpiryDate = as.Date('{0}','%Y-%m-%d')", Expiry.ToString("yyyy-MM-dd")));
            Fitted_Expiry_Prices = engine.Evaluate(string.Format("Fitted.Expiry.Prices = Calculate_Fitted.Expiry.Prices(ExpiryDate)")).AsNumeric();
            return (Fitted_Expiry_Prices.ToArray());
        }

        public double Probability_OTM(double K, string Right = "Put")
        {
            engine.Evaluate(string.Format("K = {0}",K));
            engine.Evaluate(string.Format("Right = '{0}'", Right.ToUpper()));
            return (engine.Evaluate("tmp1234=Probability.OTM(K,Right)").AsNumeric().First());
        }

        public double Cond_Loss(double K,string Right = "Put")
        {
            engine.Evaluate(string.Format("K = {0}", K));
            engine.Evaluate(string.Format("Right = '{0}'", Right.ToUpper()));
            return (engine.Evaluate("tmp1234=Cond.Loss(K,Right)").AsNumeric().First());
        }

        public double ExProfit(double K, double Premium,string Right = "Put")
        {
            engine.Evaluate(string.Format("K = {0}", K));
            engine.Evaluate(string.Format("Premium = {0}", Premium));
            engine.Evaluate(string.Format("Right = '{0}'", Right.ToUpper()));
            return (engine.Evaluate("tmp1234=ExReturn(K,Premium, Right)").AsNumeric().First());
        }

        public double SDProfit(double K, double Premium, string Right = "Put")
        {
            engine.Evaluate(string.Format("K = {0}", K));
            engine.Evaluate(string.Format("Premium = {0}", Premium));
            engine.Evaluate(string.Format("Right = '{0}'", Right.ToUpper()));
            return (engine.Evaluate("tmp1234=SDReturn(K,Premium, Right)").AsNumeric().First());
        }

        public double Sharpe(double K, double Premium, string Right = "Put")
        {
            return (ExProfit(K, Premium, Right) / SDProfit(K, Premium, Right));
        }
        #endregion
    }
}
