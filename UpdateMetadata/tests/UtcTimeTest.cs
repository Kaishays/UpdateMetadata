using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ValidateKlvExtraction.Tests
{
    public static class UtcTimeTest
    {
        private const string UtcTimeFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFZ";
        private static DateTime previousTimestamp = DateTime.MinValue;
        private static int percentFramesAllowedOutsideOfUtcTimeThreshold = 10;
        private static TimeSpan differenceBetweenUtcRowsThreshold = TimeSpan.FromMilliseconds(70);
        public static async Task<bool> ValidateTimestamps(string tsFilePath)
        {
            string csvFilePath = CsvExcists.ConvertToCsvPath(tsFilePath);
            List<string[]> metadataRows = await ReadCsvFile(csvFilePath);
            
            if (!ContainsValidUtcTimeInAllRows(metadataRows))
                return false;
                
            return AreTimestampsWithinThreshold(metadataRows);
        }
        private static bool ContainsValidUtcTimeInAllRows(List<string[]> metadataRows)
        {
            if (metadataRows == null || metadataRows.Count == 0)
                return false;

            foreach (string[] row in metadataRows)
            {
                if (row == null || row.Length == 0 || string.IsNullOrEmpty(row[0]))
                    return false;

                if (!IsValidUtcTimeFormat(row[0]))
                    return false;
            }
            return true;
        }
        private static bool AreTimestampsWithinThreshold(List<string[]> metadataRows)
        {
            ResetPreviousTimestamp();
            int framesOutsideOfTimestampThreshold = 0;
            int totalFrames = metadataRows.Count;
            int allowedBadFrames = totalFrames * 
                percentFramesAllowedOutsideOfUtcTimeThreshold / 100;
            foreach (string[] row in metadataRows)
            {
                DateTime timestamp = ParseUtcTimestamp(row[0]);

                if (!IsCurrentTimestampWithinThreshold(timestamp))
                    framesOutsideOfTimestampThreshold++;
            }
            return framesOutsideOfTimestampThreshold > allowedBadFrames;
        }
        private static void ResetPreviousTimestamp()
        {
            previousTimestamp = DateTime.MinValue;
        }
        private static DateTime ParseUtcTimestamp(string timestampString)
        {
            return DateTime.ParseExact(
                timestampString.Trim(),
                UtcTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);
        }
        private static bool IsCurrentTimestampWithinThreshold(DateTime current)
        {
            if (previousTimestamp == DateTime.MinValue)
            {
                previousTimestamp = current;
                return true;
            }

            TimeSpan difference = current - previousTimestamp;
            previousTimestamp = current;
            
            return difference < differenceBetweenUtcRowsThreshold;
        }
        private static bool IsValidUtcTimeFormat(string timeString)
        {
            try
            {
                ParseUtcTimestamp(timeString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<List<string[]>> ReadCsvFile(string csvFilePath)
        {
            List<string[]> rows = new List<string[]>();
            
            using (StreamReader reader = new StreamReader(csvFilePath))
            {
                string line = await reader.ReadLineAsync();
                while (line != null)
                {
                    rows.Add(line.Split(','));
                    line = await reader.ReadLineAsync();
                }
            }
            
            RemoveHeaderRow(rows);
            return rows;
        }
        private static void RemoveHeaderRow(List<string[]> rows)
        {
            if (rows.Count > 0)
                rows.RemoveAt(0);
        }
    }
}