using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication6
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Variable declaration
            string orderSheetAdd = "";

            // Get Order Sheet Address
            if (args.Length>0)
            {
                orderSheetAdd = args[0];
            }
            else
            {
                Console.Write("Use default order sheet ? (Y/N)");
                if (Console.ReadLine().ToString().ToUpper() == "Y")
                {
                    orderSheetAdd = OControl.DefaultOrderSheet;
                }
                else
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    ofd.Multiselect = false;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        orderSheetAdd = ofd.FileName;
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            

            // Start Order Manager
            OrderManager OM = new OrderManager(orderSheetAdd);
            OM.DoAll();

            // Clean up low value position
            Console.WriteLine("Close low value position ? (Y/N)");
            if (Console.ReadLine().ToString().ToUpper() == "Y")
            {
                OM.ClosePositionsAuto();
            }

            
        }
    }
}
