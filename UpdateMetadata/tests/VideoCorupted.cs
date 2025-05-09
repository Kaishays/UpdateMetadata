using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata;
using UpdateMetadata.tests;

namespace ValidateKlvExtraction.Tests
{
    public static class VideoCorupted
    {
        public static bool CheckFile_Corrupted(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return true;
            }
            if (!File.Exists(filePath))
            {
                return true;
            }

            FileInfo fileInfo = new FileInfo(filePath);
            bool fileLengthToSmall = fileInfo.Length < RuntimeVariables.minFileSizeForTS;
            return fileLengthToSmall;
        }
    }
}
