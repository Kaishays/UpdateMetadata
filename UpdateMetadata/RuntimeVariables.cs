using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata
{
    public static class RuntimeVariables
    {
        public const string failKlvValidationCsvPath = @"S:\Projects\AltiCam Vision\Software (MSP & GUI)\KLV_Metadata_Extraction\ReextractKlv.csv";
        public const string countErrorsCsvPath = @"S:\Projects\AltiCam Vision\Software (MSP & GUI)\KLV_Metadata_Extraction\CountFailCheck.csv";

        public static int delayMonitorVideoPlayerState = 1500;

        public const int maxFrameRate = 65;
        public const int minFrameRate = 5;

        public const int limitOfPlayerStateIsStartingLoop = 20;
        public const long limitOfMillisecondsNoFrameArrived = 15000;

        public const long minFileSizeForTS = 512000; // 500kb
        public const string UtcFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
    }
}
