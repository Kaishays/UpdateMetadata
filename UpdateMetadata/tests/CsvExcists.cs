using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ValidateKlvExtraction.Tests
{
    public static class CsvExcists
    {
        public static bool IsCsvValid(string tsFilePath)
        {
            string csvFilePath = ConvertToCsvPath(tsFilePath);
            
            if (!File.Exists(csvFilePath))
            {
                return false;
            }

            FileInfo csvFileInfo = new FileInfo(csvFilePath);
            return csvFileInfo.Length > 0;
        }
        public static string ConvertToCsvPath(string tsFilePath)
        {
            return Path.ChangeExtension(tsFilePath, ".csv");
        }
    }
}
