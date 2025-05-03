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
        private const int maxFrameRate = 65;
        private const int minFrameRate = 5;
        public static async Task<bool> CheckIfCSV_Video_Threshold(List<string[]> csvMetadataFields, string vidPath)
        {
            int klvRowCount = csvMetadataFields.Count();
            double klvFrameRate = await GetKlvFrameRate(klvRowCount, vidPath);

            if (klvFrameRate <= maxFrameRate
                    && klvFrameRate >= minFrameRate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static async Task<double> GetKlvFrameRate(int csvRowCount, string vidPath)
        {
            TimeSpan vidDur_sec;
            bool videoCorupted = false;
            (videoCorupted, vidDur_sec) =
               await VideoDurationExtractor.ExtractDuration(vidPath);

            if (!videoCorupted || vidDur_sec.TotalHours != 5000)
            {
                double framerate = csvRowCount / vidDur_sec.TotalSeconds;
                return framerate;
            }else
            {
                return -1;
            }
        }
    }
}
