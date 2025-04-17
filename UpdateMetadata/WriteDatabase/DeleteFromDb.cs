using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.WriteDatabase
{
    public static class DeleteFromDb
    {
        public static async Task DeleteExcessVideosID_FromDatabase()
        {
            var videosToRemove = IdentifyVideosToRemove(SyncY_DriveToDatabase.databaseList_VideoID, SyncY_DriveToDatabase.driveList_VideoID);
            
            if (videosToRemove.Count == 0)
            {
                return;
            }
            
            await RemoveVideosFromDatabase(videosToRemove);
            RemoveVideosFromInMemoryList(SyncY_DriveToDatabase.databaseList_VideoID, videosToRemove);
        }
        
        private static List<TableInstances.VideoID> IdentifyVideosToRemove(
            List<TableInstances.VideoID> databaseVideoIds, List<TableInstances.VideoID> driveVideoIds)
        {
            var driveIdsSet = driveVideoIds.Select(v => v.UniqueVideoID).ToHashSet();
            return databaseVideoIds
                .Where(dbVideo => !driveIdsSet.Contains(dbVideo.UniqueVideoID))
                .ToList();
        }
        
        private static async Task RemoveVideosFromDatabase(List<TableInstances.VideoID> videosToRemove)
        {
            foreach (var videoId in videosToRemove)
            {
                await DeleteVideoMetadata(videoId);
                await DeleteVideoId(videoId);
            }
        }
        
        private static async Task DeleteVideoMetadata(TableInstances.VideoID videoId)
        {
            await MySQLDataAccess.ExecuteSQL(
                SQL_QueriesStore.RawMetadata.deleteFrom, 
                videoId, 
                NameLibrary.General.connectionString);
        }
        
        private static async Task DeleteVideoId(TableInstances.VideoID videoId)
        {
            await MySQLDataAccess.ExecuteSQL(
                SQL_QueriesStore.VideoID.deleteFrom, 
                videoId, 
                NameLibrary.General.connectionString);
        }
        
        private static void RemoveVideosFromInMemoryList(
            List<TableInstances.VideoID> databaseVideoIds, List<TableInstances.VideoID> videosToRemove)
        {
            foreach (var videoId in videosToRemove)
            {
                databaseVideoIds.Remove(videoId);
            }
        }
        public static async Task RemoveOldRawMetadata(TableInstances.VideoID videoId)
        {
            await MySQLDataAccess.ExecuteSQL(
                    SQL_QueriesStore.RawMetadata.deleteFrom,
                    videoId,
                    NameLibrary.General.connectionString);
        }
    }
}
