using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UpdateMetadata.RawMetadata;

namespace UpdateMetadata.RunLogHashing
{
    public static class HashLogUpdater
    {
        public static async Task UpdateCsvHashLogs()
        {
            List<TableInstances.VideoID> databaseVideoIds = SyncY_DriveToDatabase.databaseList_VideoID;
            Debug.WriteLine($"Updating hash logs for {databaseVideoIds.Count} video IDs");
            await ProcessAllVideoIdsForHashUpdates(databaseVideoIds);
        }

        private static async Task ProcessAllVideoIdsForHashUpdates(List<TableInstances.VideoID> databaseVideoIds)
        {
            int currentIndex = 0;
            foreach (var videoId in databaseVideoIds)
            {
                currentIndex++;
                LogRawMetadataProcess.LogProgress(currentIndex, databaseVideoIds.Count);
                await ProcessSingleVideoIdHashUpdate(videoId);
            }
        }

        private static async Task ProcessSingleVideoIdHashUpdate(TableInstances.VideoID videoId)
        {
            try
            {
                await HashLogEntryCreator.TryCreateHashLogEntryForVideo(videoId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating hash log for {videoId.PathToVideo}: {ex.Message}");
            }
        }
    }
} 