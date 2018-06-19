using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace UnderConstruction
{
    class ResultViewer
    {
        private Excel.Application xlApp;
        private Excel.Workbook xlWb;
        private string holdingsFile;
        private DateTime runDate;

        public ResultViewer(string _holdingsFile)
        {
            runDate = DateTime.Now;
            holdingsFile = _holdingsFile;
            xlApp = new Excel.Application();
            xlWb = xlApp.Workbooks.Open(UControl.resultViewAdd);

        }
        
        public void RunProcedures()
        {
            ImportHldings();
            RunFilter();
        }

        private void ImportHldings()
        {
            Console.WriteLine("{0}: Importing Holdings ...", DateTime.Now.ToShortTimeString());
            xlApp.Run("ImportHoldings", holdingsFile);
            xlApp.Visible = true;
        }

        private void RunFilter()
        {
            Console.WriteLine("{0}: Import and Filtering ...", DateTime.Now.ToShortTimeString());
            // Get Folder Address
            string resultFolder;
            using (StreamReader sr = new StreamReader(UControl.ConfigFileAdd))
            {
                resultFolder = sr.ReadLine();
            }

            // Run Macro
            xlApp.Run("MassReporting", resultFolder);
        }

        private string CreateSuggestionFiles()
        {
            Console.WriteLine("{0}: Generating Suggestions ...", DateTime.Now.ToShortTimeString());
            string suggestionFileAdd = UControl.SuggestionsFolder + runDate.ToString("yyyyMMdd") + ".txt";
            xlApp.Run("generateOrderSheet", suggestionFileAdd);

            xlWb.Close(true);
            xlApp.Quit();

            return suggestionFileAdd;
        }

        public void SendSuggestions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Symbol,Expiry,Strike,Price,Contract"));

            using (StreamReader sr = new StreamReader(CreateSuggestionFiles()))
            {
                while (!sr.EndOfStream)
                {
                    sb.AppendLine(sr.ReadLine());
                }
            }

            Console.WriteLine("{0}: Sending Suggestions ...",DateTime.Now.ToShortTimeString());
            UTools.SendEmail("patrickchan2222@hotmail.com", DateTime.Now.ToString("yyyyMMdd") + " - Investment Memo", sb.ToString());
        }
    }
}
