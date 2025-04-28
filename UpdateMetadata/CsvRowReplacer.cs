using System;
using System.IO;
using System.Linq;

namespace UpdateMetadata
{
    public static class CsvRowReplacer
    {
        public static void ReplaceRow(string csvPath, int targetRowIndex, string[] newRowData)
        {
            var allLines = File.ReadAllLines(csvPath).ToList();
            
            if (targetRowIndex < 0 || targetRowIndex >= allLines.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(targetRowIndex), "Row index is outside the bounds of the CSV file");
            }

            allLines[targetRowIndex] = string.Join(",", newRowData);
            File.WriteAllLines(csvPath, allLines);
        }
    }
} 