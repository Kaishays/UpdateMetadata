using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata.ReadDatabase;
using UpdateMetadata.Y_DriveReader;

namespace UpdateMetadata.WriteDatabase
{
    public static class DeleteRowsWithNullUtcTime
    {
        public static async void CheckWhereUtcTimeNull()
        {
            string select = "SELECT UniqueVideoID\r\nFROM Metadatabase.raw_metadata\r\nWHERE UtcTime IS NULL\r\nGROUP BY UniqueVideoID;\r\n";

            List<ulong> fileID_List = await MySQLDataAccess.QuerySQL<ulong, dynamic>(select, null, NameLibrary.General.connectionString);
            int count = 0;

            foreach (ulong id in fileID_List)
            {
                string delete = $"delete FROM metadatabase.raw_metadata where UniqueVideoID  = {id}";

                await MySQLDataAccess.ExecuteSQL(delete, NameLibrary.General.connectionString);
                count++;
                Debug.WriteLine(count);
            }
        }
    }
}
