using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UpdateMetadata.RunLogHashing
{
    public static class HashLogEntryCreator
    {
        public static async Task<bool> TryCreateHashLogEntryForVideo(TableInstances.VideoID videoId)
        {
            string csvFilePath = CsvFileProcessor.ConvertVideoPathToCsvPath(videoId.PathToVideo);
            
            if (!CsvFileProcessor.DoesCsvFileExist(csvFilePath))
                return false;
                
            DateTime csvLastWriteTime = CsvFileProcessor.GetCsvLastWriteTime(csvFilePath);
            List<DateTime> existingDatabaseWriteTimes = await HashLogDatabaseService.GetAllExistingWriteTimesFromDatabase(videoId);
            
            if (HashLogDatabaseService.IsWriteTimeAlreadyInDatabase(csvLastWriteTime, existingDatabaseWriteTimes))
                return false;
                
            return await CreateNewHashLogEntry(videoId, csvFilePath, csvLastWriteTime);
        }

        private static async Task<bool> CreateNewHashLogEntry(TableInstances.VideoID videoId, string csvFilePath, DateTime csvLastWriteTime)
        {
            string csvFileHash = await CsvFileProcessor.GenerateHashForCsvFile(csvFilePath);
            uint nextRunIndex = await RunIndexCalculator.CalculateNextAvailableRunIndex(videoId.UniqueVideoID);
            return await HashLogDatabaseService.SaveHashLogEntryToDatabase(videoId.UniqueVideoID, csvFileHash, nextRunIndex, csvLastWriteTime);
        }
    }
} 