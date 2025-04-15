using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using Org.BouncyCastle.Crypto;

namespace UpdateMetadata.Y_DriveReader
{
    public static class Y_DriveKlvExtractionCompletionTest
    {
        public static bool CheckAll(string csv, string ts)
        {
            return CheckIfCSV_Video_Threshold(csv, ts) && DoesCsvMatchVideoId(csv, ts);
        }
        public static long GetFileSize(string filePath)
        {

            FileInfo fileInfo = new FileInfo(filePath);
            long fileSize = fileInfo.Length;
            return fileSize;
        }


        public static string ConvertFilePathToCSV(string filePath)
        {
            return Path.ChangeExtension(filePath, ".csv");
        }
        public static bool CheckIfCSV_Video_Threshold(string csv, string ts)
        {
            double csvSize = GetFileSize(csv);

            double[] range = CsvFileSizeRangeBuild(ts);
            if (range[0]== -1)
                return false;
            double lowLimit = range[0];
            double highLimit = range[1];

            if (csvSize <= highLimit && csvSize >= lowLimit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static double[] CsvFileSizeRangeBuild(string vidPath)
        {
            TimeSpan vidDur = GetVideoDurationFromFileMetadata(vidPath);
            if (vidDur.TotalHours == 5000)
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
        private static TimeSpan GetVideoDurationFromFileMetadata(string filePath)
        {
            try
            {
                var shellFile = ShellFile.FromFilePath(filePath);
                var durationValue = shellFile.Properties.System.Media.Duration.Value;
                if (durationValue != null)
                {
                    long durationTicks = (long)durationValue;
                    return TimeSpan.FromTicks(durationTicks);
                }
            }
            catch (Exception ex)
            {

            }

            return TimeSpan.FromHours(5000);
        }
        public static bool DoesCsvMatchVideoId(string csvFilePath, string videoId)
        {
            string csvFileName = Path.GetFileNameWithoutExtension(csvFilePath);
            string videoFileName = Path.GetFileNameWithoutExtension(videoId);
            
            return csvFileName.Equals(videoFileName, StringComparison.OrdinalIgnoreCase);
        }
        public static bool UtcTimeInEveryCsvRow(List<string[]> metadataFields)
        {
            if (metadataFields == null || metadataFields.Count == 0)
                return false;
                
            foreach (string[] line in metadataFields)
            {
                if (line == null || line.Length == 0 || string.IsNullOrEmpty(line[0]))
                    return false;
                    
                try
                {
                    DateTime.ParseExact(
                        line[0].Trim(),
                        "yyyy-MM-ddTHH:mm:ss.FFFFFFZ",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.AssumeUniversal);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
