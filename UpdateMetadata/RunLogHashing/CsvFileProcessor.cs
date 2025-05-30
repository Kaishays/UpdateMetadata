using System;
using System.IO;
using System.Threading.Tasks;

namespace UpdateMetadata.RunLogHashing
{
    public static class CsvFileProcessor
    {
        public static string ConvertVideoPathToCsvPath(string videoFilePath)
        {
            return Path.ChangeExtension(videoFilePath, ".csv");
        }

        public static bool DoesCsvFileExist(string csvFilePath)
        {
            return File.Exists(csvFilePath);
        }

        public static DateTime GetCsvLastWriteTime(string csvFilePath)
        {
            return File.GetLastWriteTime(csvFilePath);
        }

        public static async Task<string> GenerateHashForCsvFile(string csvFilePath)
        {
            ulong uniqueFileHashCode = await GenerateUniqueVidHash.GetUniqueFileHashCode(csvFilePath);
            return uniqueFileHashCode.ToString();
        }
    }
} 