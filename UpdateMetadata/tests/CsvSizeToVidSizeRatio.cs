using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidateKlvExtraction.Tests
{
    public static class CsvSizeToVidSizeRatio
    {
        public static async Task<bool> CheckIfCSV_Video_Threshold(string csv, string ts)
        {
            double csvSize = GetFileSize(csv);
            double[] range = await CsvFileSizeRangeBuild(ts);
            if (range[0] == -1)
                return false;

            double lowLimit = range[0];
            double highLimit = range[1];

            if (csvSize <= highLimit
                && csvSize >= lowLimit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static async Task<double[]> CsvFileSizeRangeBuild(string vidPath)
        {
            TimeSpan vidDur;
            bool videoCorupted = false;
            (videoCorupted, vidDur) = 
                await VideoDurationExtractor.ExtractDuration(vidPath);

            if (videoCorupted || vidDur.TotalHours == 5000)
            {
                return new double[]
                {
                    -1
                };
            }

            double seconds = vidDur.TotalSeconds;
            int minKlvRowSize_Bytes = 36;
            int maxKlvRowSize_Bytes = 412;
            int highestFps = 60;
            int lowestFps = 15;

            double csvLowLimit =
                minKlvRowSize_Bytes * lowestFps * seconds;
            double csvHighLimit =
                maxKlvRowSize_Bytes * highestFps * seconds;

            double[] results = new double[2];
            results[0] = csvLowLimit;
            results[1] = csvHighLimit;
            return results;
        }
        private static long GetFileSize(string filePath)
        {
             if (!VideoCorupted.CheckFile_Corrupted(filePath))
                return new FileInfo(filePath).Length;
            else
            {
                return 0;
            }
        }
    }
}
