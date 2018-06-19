using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Console5Controller
{
    class Program
    {
        static void Main(string[] args)
        {
            int wakeUpTime = 2;
            int wakeupDuration = 5; // (in minutes)
            string MainDirectory = Environment.UserName.ToUpper() == "CHAN" ? @"C:\Users\chan\Documents\dfkjdf\" : @"C:\Users\Samuel\Documents\dfkjdf\";

            // Start IBG Paper
            Console.WriteLine("{0}: Starting IBGateway ...", DateTime.Now.ToShortTimeString());
            Process IBGProcess = new System.Diagnostics.Process();
            IBGProcess.StartInfo.FileName = "IBControllerGatewayStart.bat";
            IBGProcess.StartInfo.WorkingDirectory = MainDirectory.Replace("dfkjdf\\", "IBController");
            IBGProcess.Start();
            Thread.Sleep(1 * 60 * 1000);


            // Wake Console 5
            Process Console5 = new System.Diagnostics.Process();
            for (int i = 0; i < wakeUpTime; i++)
            {
                Console.WriteLine("{0}: Starting Console 5 Alarm - Trial {1}", DateTime.Now.ToShortTimeString(),  i + 1);
                Console5.StartInfo.FileName = "ConsoleApplication5.exe";
                Console5.StartInfo.WorkingDirectory = MainDirectory + @"ConsoleApplication5 v2\ConsoleApplication5\bin\Debug";
                Console5.Start();
                Thread.Sleep(wakeupDuration * 60* 1000);
                Console5.Kill();
            }

            // Start Console 5
            Console.WriteLine("{0}: Starting Console 5 ...", DateTime.Now.ToShortTimeString());
            Console5.StartInfo.FileName = "ConsoleApplication5.exe";
            Console5.StartInfo.WorkingDirectory = MainDirectory + @"ConsoleApplication5 v2\ConsoleApplication5\bin\Debug";
            Console5.Start();

        }
    }
}
