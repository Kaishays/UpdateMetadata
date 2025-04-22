﻿using System;
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
        private static DateTime ErrorDateTimeConst = DateTime.MaxValue;
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

            return CountInvalidTimestamps(metadataRows) && 
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
        private static bool CountInvalidTimestamps(List<string[]> metadataRows)
        {
            int invalidCount = 0;
            int maxAllowedInvalidCount 
                = CalculateMaxAllowedInvalidCount(metadataRows.Count);

            foreach (string[] row in metadataRows)
            {
                if (row.Length == 0 || string.IsNullOrEmpty(row[0]) 
                    || !IsValidUtcTimeFormat(row[0]))
                {
                    invalidCount++;
                }
                if (invalidCount > maxAllowedInvalidCount)
                {
                    return false;
                }
            }
            return true;
        }

        private static int CalculateMaxAllowedInvalidCount(int totalCount)
        {
            return (int)(totalCount * (MaxPercentOutOfThreshold / 100.0));
        }
        private static bool IsValidUtcTimeFormat(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                return false;
            }

            timeString = timeString.Trim();
            if (timeString == "1970-01-01")
            {
                return true;
            }

            try
            {
                DateTime.ParseExact(
                timeString,
                UtcTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);
                return true;
            }
            catch
            {
                return false;
            }
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
                previousTimestamp = ErrorDateTimeConst;
                outOfSequenceCount++;
            }

            return outOfSequenceCount;
        }

        private static bool IsWithinTimeThreshold(DateTime currentTimestamp)
        {
            if (previousTimestamp == DateTime.MinValue)
            {
                previousTimestamp = currentTimestamp;
                return true;
            } else if (previousTimestamp == DateTime.MinValue)
            {
                previousTimestamp = currentTimestamp;
                return false;
            }

            TimeSpan difference = currentTimestamp - previousTimestamp;
            TimeSpan absoluteDifference = difference.Duration();
            
            previousTimestamp = currentTimestamp;
            return absoluteDifference <= MaxTimeDifference;
        }

        private static void ResetPreviousTimestamp()
        {
            previousTimestamp = DateTime.MinValue;
        }

        private static DateTime ParseUtcTimestamp(string timestampString)
        {
            if (string.IsNullOrEmpty(timestampString))
            {
                return ErrorDateTimeConst;
            }
            
            timestampString = timestampString.Trim();
            
            if (timestampString == "1970-01-01")
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
            try
            {
                return DateTime.ParseExact(
                    timestampString,
                    UtcTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal);
            }
            catch (FormatException)
            {
                return DateTime.MaxValue;
            }
        }

    }
}