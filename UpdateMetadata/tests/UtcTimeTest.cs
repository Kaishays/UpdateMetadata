using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ValidateKlvExtraction.Tests
{
    public static class UtcTimeTest
    {
        private const string UtcTimeFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFZ";
        private const double MaxPercentOutOfThreshold = 1.0;
        private const int MaxTimeDifferenceMilliseconds = 70;
        
        private static DateTime previousTimestamp = DateTime.MinValue;
        private static readonly TimeSpan MaxTimeDifference = TimeSpan.FromMilliseconds(MaxTimeDifferenceMilliseconds);

        public static async Task<bool> ValidateUtcTimestamps(string tsFilePath)
        {
            string csvFilePath = ConvertToCsvPath(tsFilePath);
            List<string[]> metadataRows = await ReadCsvFileWithoutHeader(csvFilePath);
            
            if (metadataRows.Count == 0)
            {
                return false;
            }
                
            return HasValidUtcTimeFormat(metadataRows) && 
                   HasConsistentTimestampSequence(metadataRows);
        }

        private static string ConvertToCsvPath(string tsFilePath)
        {
            return tsFilePath.Replace(".ts", ".csv");
        }

        private static async Task<List<string[]>> ReadCsvFileWithoutHeader(string csvFilePath)
        {
            List<string[]> rows = await ReadAllCsvRows(csvFilePath);
            
            if (rows.Count > 0)
            {
                rows.RemoveAt(0);
            }
            
            return rows;
        }
        
        private static async Task<List<string[]>> ReadAllCsvRows(string csvFilePath)
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
            
            return rows;
        }

        private static bool HasValidUtcTimeFormat(List<string[]> metadataRows)
        {
            int invalidTimestampCount = CountInvalidTimestamps(metadataRows);
            int maxAllowedInvalidCount = CalculateMaxAllowedInvalidCount(metadataRows.Count);
            
            return invalidTimestampCount <= maxAllowedInvalidCount;
        }

        private static int CountInvalidTimestamps(List<string[]> metadataRows)
        {
            int invalidCount = 0;
            
            foreach (string[] row in metadataRows)
            {
                if (row.Length == 0 || string.IsNullOrEmpty(row[0]) 
                    || !IsValidUtcTimeFormat(row[0]))
                {
                    invalidCount++;
                }
            }
            
            return invalidCount;
        }

        private static int CalculateMaxAllowedInvalidCount(int totalCount)
        {
            return (int)(totalCount * (MaxPercentOutOfThreshold / 100.0));
        }

        private static bool HasConsistentTimestampSequence(List<string[]> metadataRows)
        {
            ResetPreviousTimestamp();
            
            int outOfSequenceCount = CountOutOfSequenceTimestamps(metadataRows);
            int maxAllowedOutOfSequence = CalculateMaxAllowedInvalidCount(metadataRows.Count);
            
            return outOfSequenceCount <= maxAllowedOutOfSequence;
        }

        private static int CountOutOfSequenceTimestamps(List<string[]> metadataRows)
        {
            int outOfSequenceCount = 0;
            
            foreach (string[] row in metadataRows)
            {
                if (row.Length > 0 && !string.IsNullOrEmpty(row[0]) 
                    && IsValidUtcTimeFormat(row[0]))
                {
                    DateTime currentTimestamp = ParseUtcTimestamp(row[0]);
                    
                    if (!IsWithinTimeThreshold(currentTimestamp))
                    {
                        outOfSequenceCount++;
                    }
                }
            }
            
            return outOfSequenceCount;
        }

        private static bool IsWithinTimeThreshold(DateTime currentTimestamp)
        {
            if (previousTimestamp == DateTime.MinValue)
            {
                previousTimestamp = currentTimestamp;
                return true;
            }

            TimeSpan difference = currentTimestamp - previousTimestamp;
            previousTimestamp = currentTimestamp;
            
            return difference <= MaxTimeDifference;
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

        private static bool IsValidUtcTimeFormat(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                return false;
            }
            
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
    }
}