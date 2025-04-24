using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata.tests;
using ValidateKlvExtraction.Tests;
using System.IO;

namespace UpdateMetadata
{
    public static class LogReextractKlv
    {
        private static int csvFileNotFoundCount = 0;
        private static int unhandledErrorCount = 0;
        private static int existingRawMetadataCount = 0;
        private static int invalidCsvVideoRatioCount = 0;
        private static int invalidUtcTimestampsCount = 0;
        private const string failKlvValidationCsvPath = @"S:\Projects\AltiCam Vision\Software (MSP & GUI)\KLV_Metadata_Extraction\FailCheck.csv";
        private const string countErrorsCsvPath = @"S:\Projects\AltiCam Vision\Software (MSP & GUI)\KLV_Metadata_Extraction\CountFailCheck.csv";

        public static void LogMissingCsvFile(int errorCode, string videoPath)
        {
            string errorString = "";
            switch (errorCode)
            {
                case (0):
                    errorString = "Csv File Not Found";
                    csvFileNotFoundCount++;
                    break;
                case (1):
                    errorString = "Unhandled Error in TryUpdateMetadata";
                    unhandledErrorCount++;
                    break;
                default:
                    break;
            }
     
            CsvWriter.ManageCSV_Append("", videoPath, failKlvValidationCsvPath);
            Debug.WriteLine($"CSV VALIDATION ERROR: {errorString} - {videoPath}");
            LogErrorCounts();
        }
        public static void LogMissingCsvFile(TestResultsMetadata testResultsMetadata, string videoPath)
        {
            DetermineErrorCount(testResultsMetadata);
            CsvWriter.ManageCSV_Append("", videoPath, failKlvValidationCsvPath);
            Debug.WriteLine($"CSV VALIDATION ERROR: {videoPath}");
            LogErrorCounts();
        }
        public static void LogErrorCounts()
        {
            StringBuilder countSummary = new StringBuilder("Error Count Summary: ");
            bool hasErrors = false;

            if (csvFileNotFoundCount > 0)
            {
                countSummary.Append($"CSV File Not Found: {csvFileNotFoundCount}");
                hasErrors = true;
            }

            if (unhandledErrorCount > 0)
            {
                if (hasErrors) countSummary.Append(" | ");
                countSummary.Append($"Unhandled Errors: {unhandledErrorCount}");
                hasErrors = true;
            }

            if (existingRawMetadataCount > 0)
            {
                if (hasErrors) countSummary.Append(" | ");
                countSummary.Append($"Metadata Already in DB: {existingRawMetadataCount}");
                hasErrors = true;
            }

            if (invalidCsvVideoRatioCount > 0)
            {
                if (hasErrors) countSummary.Append(" | ");
                countSummary.Append($"Invalid CSV/Video Ratio: {invalidCsvVideoRatioCount}");
                hasErrors = true;
            }

            if (invalidUtcTimestampsCount > 0)
            {
                if (hasErrors) countSummary.Append(" | ");
                countSummary.Append($"Invalid UTC Timestamps: {invalidUtcTimestampsCount}");
                hasErrors = true;
            }

            if (!hasErrors)
            {
                countSummary.Append("No errors detected");
            }

            CsvWriter.ManageCSV_Append("", countSummary.ToString(), countErrorsCsvPath);
        }
        private static void DetermineErrorCount(TestResultsMetadata testResultsMetadata)
        {
            if (testResultsMetadata.HasRawMetadataInDb)
            {
                existingRawMetadataCount++;
            }

            if (!testResultsMetadata.HasValidCsvVideoRatio)
            {
                invalidCsvVideoRatioCount++;
            }

            if (!testResultsMetadata.HasValidUtcTimestamps)
            {
                invalidUtcTimestampsCount++;
            }            
        }
    }
}
