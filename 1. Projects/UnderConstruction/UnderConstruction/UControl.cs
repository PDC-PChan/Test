﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderConstruction
{
    class UControl
    {
        public static string MasterDirectory = Environment.UserName.ToUpper() == "CHAN" ? @"C:\Users\chan\Documents\dfkjdf\" : @"C:\Users\Samuel\Documents\dfkjdf\";
        public static string MainDirectory = MasterDirectory + "0.VolAnalysis";
        public static string ConfigFileAdd = MainDirectory + @"\UnderConfig\Config.txt";
        public static string HoldingsFolder = MainDirectory + @"\UnderConfig\Holdings\";
        public static string SuggestionsFolder = MainDirectory + @"\UnderConfig\Suggestions\";
        public static string resultViewAdd = MainDirectory + @"\Result Viewer (3).xlsm";
    }
}
