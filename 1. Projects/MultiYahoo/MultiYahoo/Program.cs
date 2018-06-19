using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace MultiYahoo
{
    class Program
    {
        static void Main(string[] args)
        {
            int split = 10;
            for (int i = 0; i < split; i++)
            {
                Thread.Sleep(1000);
                Process.Start(MYControl.ApplicationAddress, split+ " " + i);
            }
        }
    }
}
