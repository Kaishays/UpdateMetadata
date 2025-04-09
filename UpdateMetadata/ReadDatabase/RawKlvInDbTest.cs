using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UpdateMetadata.ReadDatabase
{
   public static class RawKlvInDbTest
    {
        public static async Task<bool> TestIfRawMetadatraInDB(TableInstances.VideoID vidID_Instance, List<string[]> allFieldsInCSV)
        {
            int rowCt = await GetRowCount(vidID_Instance);
            int csvCt = allFieldsInCSV.Count();

            if (rowCt == csvCt)
                return true;
            else
                return false;
        }
        public static async Task<int> GetRowCount(TableInstances.VideoID vidID_Instance)
        {
            List<int> rowCt = await
                 MySQLDataAccess.QuerySQL<int, TableInstances.VideoID>(
                     SQL_QueriesStore.RawMetadata.getRowCount,
                     vidID_Instance,
                     NameLibrary.General.connectionString);
            return rowCt[0];
        }

        // Helper class to properly map the COUNT(*) result
        private class RowCountResult
        {
            public int Count { get; set; }
        }
    }
}
