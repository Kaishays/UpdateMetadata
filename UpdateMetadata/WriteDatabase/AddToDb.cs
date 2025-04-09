using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.WriteDatabase
{
    public static class AddToDb
    {
        public static async Task AddNewVideosID_ToDatabase()
        {

            if (!DatabaseNeedsUpdate(
                SyncY_DriveToDatabase.databaseList_VideoID, SyncY_DriveToDatabase.driveList_VideoID))
            {
                return;
            }

            var newVideoIds = IdentifyNewVideoIds(SyncY_DriveToDatabase.databaseList_VideoID, SyncY_DriveToDatabase.driveList_VideoID);
            await AddVideoIdsToDatabase(newVideoIds);
            UpdateInMemoryDatabaseList(SyncY_DriveToDatabase.databaseList_VideoID, newVideoIds);
        }

        private static bool DatabaseNeedsUpdate(
           List<TableInstances.VideoID> databaseVideoIds, List<TableInstances.VideoID> driveVideoIds)
        {
            return databaseVideoIds.Count < driveVideoIds.Count;
        }

        private static List<TableInstances.VideoID> IdentifyNewVideoIds(
            List<TableInstances.VideoID> databaseVideoIds, List<TableInstances.VideoID> driveVideoIds)
        {
            if (databaseVideoIds.Count == 0)
            {
                return new List<TableInstances.VideoID>(driveVideoIds);
            }

            var existingIds = databaseVideoIds.Select(v => v.UniqueVideoID).ToHashSet();
            return driveVideoIds.Where(v => !existingIds.Contains(v.UniqueVideoID)).ToList();
        }

        private static async Task AddVideoIdsToDatabase(List<TableInstances.VideoID> videoIds)
        {
            foreach (var videoId in videoIds)
            {
                await MySQLDataAccess.ExecuteSQL(
                    SQL_QueriesStore.VideoID.addTo, 
                    videoId, 
                    NameLibrary.General.connectionString);
            }
        }

        private static void UpdateInMemoryDatabaseList(
            List<TableInstances.VideoID> databaseVideoIds, List<TableInstances.VideoID> newVideoIds)
        {
            databaseVideoIds.AddRange(newVideoIds);
        }
    }
}
