using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace UnderConstructionController
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start TWS Live
            Console.WriteLine("{0}: Starting TWS ...", DateTime.Now.ToShortTimeString());
            Process IBGProcess = new System.Diagnostics.Process();
            IBGProcess.StartInfo.FileName = "IBControllerStart - live.bat";
            IBGProcess.StartInfo.WorkingDirectory = @"C:\Users\Samuel\Documents\IBController";
            IBGProcess.Start();
            Thread.Sleep(1 * 60 * 1000);

            // Remove all open SELL OPT PUT Order


            // Run UnderContructor
            Process UnderConstructor = new System.Diagnostics.Process();
            Console.WriteLine("{0}: Starting Under Constructor ...", DateTime.Now.ToShortTimeString());
            UnderConstructor.StartInfo.FileName = "UnderConstruction.exe";
            UnderConstructor.StartInfo.WorkingDirectory = @"C:\Users\Samuel\source\repos\UnderConstruction\UnderConstruction\bin\Debug";
            UnderConstructor.Start();
        }
    }
}
