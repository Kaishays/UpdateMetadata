using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.CompressKlvMetadata
{
    public static class BinMetadataCalc
    {
        // for slant range 
        // there will be increments of 50 from 0 - 20,000 m 
        // 20,000 / 50 = 400 total bins 
        // each bin will have an index from 0-399
        // when 29 consecutive frames within 1 bin are counted, 
        // add 1 row to SL column with correct index 

        // for UTC Time
        // count when there are 29(odd to avoide even splits) conescutive frames within 1yr or 1 month or 1 day or 1 hour 
        // year Column, 0-3000 
        // month Column,0-11
        // day Column 0-30
        // hour 0-23


        // Bins Table:
        // | UniqueVidID | 00:00:00:000000 | year | Month | day | Hour | SL_BinIndex | SensorName |
        // 

        // average 29 frame Change in klv fields 
        // | UniqueVidID | 00:00:00:000000 | chg_year | chg_Month | chg_day | chg_Hour | chg_SL_BinIndex | chg_SensorName | (~total rows / 29)

        // max sequence (average 29 frame Change in klv fields) (independent for each field) (~17,000 rows)


        // example  query: 
        // "find videos with more than n frames with these parameters: ([value, +/- allowed drift] [2025,  +/-0], [jan,  +/-0],  [23,  +/-4], ["SX8" Sensor,  +/-0], [slant range of 1500 meters",  +/-100m)
        // d = unique change threshold set for each parameter

        // narrow scope
        // 1) find videos with max/min </> each field (2025 ,jan, nighttime, "SX8" Sensor, slant rnage of 1500 meters)
        // returns vidIDList
        // 2) find videos with >= n frames with d (fields are independent) 
        // returns vidIDList

        // find overlap: 
        // 3) find videos with overlap of >= n frames with low change in each field (fields are dependent) 
        // returns list of uniquevidIDs, and starting timstamp for each.

        // 4) find videos were first timestamp is within '+/- 4 bins' of intial query
        // returns vidIDList



        // potential issues: 
        // how to define low change? 
        // how to combat drift? 

        // compressed change table: 
        // "find a timespan in video where the total change <= d"
        // if change f'(x) 
        // and total change T(x) = f(x) = s[f'(x)]dx from [a to b]
        // find the greatest b - a where T(x) <= d

        // f'(x) = [n0, n1, n3, ... ne)
        // T(x) for a given interval = sum(na > nb)
        // T(x) = 
        private static class MaxLowChangeTimespan
        {
            public static int indexA;
            public static int indexB;
            public static int count = indexB - indexA;

        }
        public static void CalculateLongestLowChangeTimespan(List<double> klvList)
        {
            double maxChangeAllowed = 100.0;
            int indexA = 0;
            int indexB = 0;
            double currentTotal = 0;
            int longestTimespan = 0;
            foreach (double klv in klvList)
            {
                indexB++;

                currentTotal += klv;
                int current
                if (longestTimespan)
                {

                }
                if (currentTotal >= maxChangeAllowed)
                {
                    indexA = indexB;

                }
            }
        }
        


        // if T(x) = f'(x+1) 
        // Does |f(x+1) - f(x)| <= 
        // Does |SRn - SR0 | <= d 

        // 


        // for multi-klv-field consecutive queries, find 

        // General: 
        // mark -1 if not consecutive
        // an example query: 
        // "find videos with more than 100 frames with these parameters: 2025 ,jan, nighttime, "SX8" Sensor, slant rnage of 1500 meters"


        // this method will only compress total rows by factor of 1/29. 
        // Could count total number of consecutive frames at any given index 
        // Count_Bins Table:



        // this only accounts for allined parameters for 30 consecutive frames. Is there a way to count with aligned parameters for more than 30 frames?

        // 


        // could use max/min, sensor name to accelerate search

        // Hih -sta- re-sa////////
    }
}
