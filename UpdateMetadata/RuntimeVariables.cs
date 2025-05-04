using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata
{
    public static class RuntimeVariables
    {
        public static int delayMonitorVideoPlayerState = 1500;

        public const int maxFrameRate = 65;
        public const int minFrameRate = 5;

        public const int limitOfPlayerStateIsStartingLoop = 20;
        public const long limitOfMillisecondsNoFrameArrived = 15000;

        public const long minFileSizeForTS = 10000; // ~900kb for rgb 480x640 jpg (no KLV) 
        public const string UtcFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
    }
}
