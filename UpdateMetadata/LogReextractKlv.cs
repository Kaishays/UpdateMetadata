using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata.tests;
using ValidateKlvExtraction.Tests;

namespace UpdateMetadata
{
    public static class LogReextractKlv
    {
        public static void LogMissingCsvFile(string videoPath)
        {
            CsvWriter.ManageCSV_Append("", videoPath);
            Debug.WriteLine($"CSV FILE ERROR: {videoPath}");
        }
    }
}
