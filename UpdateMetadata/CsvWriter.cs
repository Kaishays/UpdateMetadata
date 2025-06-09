﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UpdateMetadata
{
    public static class CsvWriter
    {       
        public static bool ManageCSV_Append(string message, string targetFile, string filePathToWriteTo)
        {
            bool successfulParse = false;
            while (!successfulParse)
            {
                successfulParse = true;// AppendToCSV(message, targetFile, filePathToWriteTo);
            }
            return successfulParse;
        }
        public static bool AppendToCSV(string message, string targetFilePath, string filePathToWriteTo)
        {
            try
            {
                using (FileStream stream = new FileStream(filePathToWriteTo, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Seek(0, SeekOrigin.End);

                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        if (message == "")
                        {
                            writer.WriteLine(targetFilePath);
                        }
                        else
                        {
                            writer.WriteLine(message + " " + targetFilePath);
                        }
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
