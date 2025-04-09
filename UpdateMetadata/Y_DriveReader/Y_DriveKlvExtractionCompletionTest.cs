using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.Y_DriveReader
{
    public static class Y_DriveKlvExtractionCompletionTest
    {
        private static double highRatioOfCSV_ToTS_Size = .5;
        private static double lowRatioOfCSV_ToTS_Size = .01;
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
            double tsSize = GetFileSize(ts);
            double ratio = (double)csvSize / tsSize;

            double hL = highRatioOfCSV_ToTS_Size;
            double lL = lowRatioOfCSV_ToTS_Size;

            if (ratio <= hL && ratio >= lL)
            {
                return true;
            }
            else
            {
                return false;
            }
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
