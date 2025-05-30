using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UpdateMetadata
{
    public static class AddYdriveToReextractCsv
    {

        public static void AddAllTsFilesToReextractCsv()
        {
            string[] tsFiles = Directory.GetFiles(NameLibrary.General.pathToDrive, "*.ts", SearchOption.AllDirectories);
            
            foreach (string tsFile in tsFiles)
            {
                CsvWriter.ManageCSV_Append("", tsFile, RuntimeVariables.failKlvValidationCsvPath);
            }
        }
    }
}
