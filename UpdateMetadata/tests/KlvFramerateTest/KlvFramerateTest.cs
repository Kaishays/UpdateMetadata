using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata.tests.VidPlayerTests;

namespace UpdateMetadata.tests.KlvFramerateTest
{
    public static class KlvFramerateTest
    {
        public static async Task<bool> CheckKlvFrameRate(List<string[]> csvMetadataFields, string vidPath)
        {
            int klvRowCount = csvMetadataFields.Count();
            double klvFrameRate = await GetKlvFrameRate(klvRowCount, vidPath);

            if (klvFrameRate <= RuntimeVariables.maxFrameRate
                    && klvFrameRate >= RuntimeVariables.minFrameRate)
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

            if (!videoCorupted)
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
