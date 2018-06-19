using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public static class Toolkit
    {
        public static DateTime TranslateTime(long seconds)
        {
            return Control.IBInitialTime.AddSeconds(seconds);
        }

        public static int RndReqId()
        {
            return new Random().Next(1000);
        }

        public static DateTime StringToDate(string _time)
        {
            return new DateTime(Convert.ToInt16(_time.Substring(0,4)),
                                Convert.ToInt16(_time.Substring(4,2)),
                                Convert.ToInt16(_time.Substring(6,2)));
        }

        public static int[] RandomSample(int minValue, int maxValue, int nSample)
        {
            HashSet<int> resultSample = new HashSet<int>();
            while (resultSample.Count < nSample)
            {
                resultSample.Add(new Random().Next(minValue, maxValue));
            }
            return resultSample.ToArray<int>();
        }
    }
}
