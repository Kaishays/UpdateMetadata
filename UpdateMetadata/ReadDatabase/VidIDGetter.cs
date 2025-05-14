using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using Application = System.Windows.Application;

namespace UpdateMetadata.ReadDatabase
{
    public static class VidIDGetter
    {
        public static async Task<List<TableInstances.VideoID>> GetVid_Ids_FromDb()
        {
            List<TableInstances.VideoID> fileID_and_Path_List = new List<TableInstances.VideoID>();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            try
            {
                fileID_and_Path_List = await MySQLDataAccess.QuerySQL<TableInstances.VideoID, dynamic>(SQL_QueriesStore.VideoID.selectFrom, null, connectionString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            return fileID_and_Path_List;
        }
    }
}
