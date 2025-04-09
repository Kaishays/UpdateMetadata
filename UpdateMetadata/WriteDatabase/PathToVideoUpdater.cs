using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.WriteDatabase
{
    public static class PathToVideoUpdater
    {
        public static async Task UpdatePathToVideo()
        {
            await UpdatePathToVideo(SyncY_DriveToDatabase.databaseList_VideoID, SyncY_DriveToDatabase.driveList_VideoID);
        }

        public static async Task UpdatePathToVideo(
            List<TableInstances.VideoID> databaseVideoIds, 
            List<TableInstances.VideoID> driveVideoIds)
        {
            var databaseVideoIdMap = databaseVideoIds.ToDictionary(vid => vid.UniqueVideoID);

            foreach (var driveVideoId in driveVideoIds)
            {
                if (databaseVideoIdMap.TryGetValue(driveVideoId.UniqueVideoID, out var databaseVideoId) && 
                    driveVideoId.PathToVideo != databaseVideoId.PathToVideo)
                {
                    await UpdateVideoPath(driveVideoId);
                }
            }
        }

        private static async Task UpdateVideoPath(TableInstances.VideoID videoId)
        {
            await MySQLDataAccess.ExecuteSQL(
                SQL_QueriesStore.VideoID.insertPathToVideo, 
                videoId, 
                NameLibrary.General.connectionString);
        }
    }
}
