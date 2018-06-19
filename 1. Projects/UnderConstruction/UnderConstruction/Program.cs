using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderConstruction
{
    class Program
    {
        static void Main(string[] args)
        {
            string AccountNum = "U2252524";
            int connectionPort = 7496;

            // Get portfolio holdings
            AccountReporter ar = new AccountReporter(AccountNum, connectionPort);

            // Remove all open SELL OPT PUT orders
            //ar.RemoveAllOpenOrders();
            //ar.RemoveAllOpenOrders();

            // Run result Viewer
            ResultViewer rv = new ResultViewer(ar.RunProcedures());
            rv.RunProcedures();

            // Send trading suggestions
            rv.SendSuggestions();
        }

    }
    
}
