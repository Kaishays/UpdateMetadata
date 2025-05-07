using System;
using System.IO;

namespace UpdateMetadata.tests
{
    public static class TsVideoFileTest
    {
        public static bool IsValidVideoFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            if (!File.Exists(filePath))
            {
                return false;
            }

            string extension = Path.GetExtension(filePath);
            bool isValid = string.Equals(extension, ".ts", StringComparison.OrdinalIgnoreCase);
            
            return isValid;
        }
    }
} 