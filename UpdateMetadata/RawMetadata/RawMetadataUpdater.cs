using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UpdateMetadata.Y_DriveReader;

using Microsoft.WindowsAPICodePack.Shell;


namespace UpdateMetadata.RawMetadata
{
    public static class RawMetadataUpdater
    {
        private static int processedFileCount = 0;

        public static async Task UpdateRawMetadata()
        {
            await UpdateRawMetadata(
                SyncY_DriveToDatabase.databaseList_VideoID, 
                SyncY_DriveToDatabase.driveList_VideoID);
        }
        public static async Task UpdateRawMetadata(
            List<TableInstances.VideoID> databaseVideoIds, 
            List<TableInstances.VideoID> driveVideoIds)
        {

            foreach (var videoId in databaseVideoIds)
            {
                await FindMatchingCsv(videoId);
            }
        }
        private static async Task FindMatchingCsv(TableInstances.VideoID videoId)
        {
            string csvFilePath = Path.ChangeExtension(videoId.PathToVideo, ".csv");
            
            if (File.Exists(csvFilePath))
            {
                processedFileCount++;
                await ProcessCsvFile(csvFilePath, videoId);
            }
        }
        private static async Task ProcessCsvFile(string csvFilePath, TableInstances.VideoID videoId)
        {
            var metadataFields = await CsvToRawMetadata.ReadCSV(csvFilePath);
            
            if (await ShouldUpdateMetadata(csvFilePath, videoId, metadataFields))
            {
                await RemoveOldRawMetadata(videoId);
                await AddRawMetadataToDatabase(metadataFields, videoId);
            }
            else
            {
                LogMissingCsvFile(videoId.PathToVideo);
            }
        }

        private static async Task<bool> ShouldUpdateMetadata(
            string csvFilePath, 
            TableInstances.VideoID videoId, 
            List<string[]> metadataFields)
        {
            if (await RawKlvInDbTest.TestIfRawMetadatraInDB(videoId, metadataFields))
            {
                return false;
            }

            if (!Y_DriveKlvExtractionCompletionTest.CheckIfCSV_Video_Threshold(csvFilePath, videoId.PathToVideo))
            {
                return false;
            }

            if (!Y_DriveKlvExtractionCompletionTest.UtcTimeInEveryCsvRow(metadataFields))
            {
                return false;
            }

            return true;
        }

        private static async Task AddRawMetadataToDatabase(
            List<string[]> metadataFields, 
            TableInstances.VideoID videoId)
        {
            List<TableInstances.RawMetadata> rawMetadataRows 
                = CsvToRawMetadata.ConvertCsvToRawMetadata(metadataFields, videoId);
            int frameCount = 0;

            foreach (TableInstances.RawMetadata row in rawMetadataRows)
            {
                frameCount++;
                LogProcessingProgress(
                    frameCount, 
                    processedFileCount, 
                    videoId.PathToVideo);

                await MySQLDataAccess.ExecuteSQL(
                    SQL_QueriesStore.RawMetadata.addTo,
                    row,
                    NameLibrary.General.connectionString);
            }
        }
        private static async Task RemoveOldRawMetadata(TableInstances.VideoID videoId)
        {
            await MySQLDataAccess.ExecuteSQL(
                    SQL_QueriesStore.RawMetadata.deleteFrom,
                    videoId,
                    NameLibrary.General.connectionString);
        }

        private static void LogMissingCsvFile(string videoPath)
        {
            SyncY_DriveToDatabase.OnlyForDebug_PotentialCSVFileNotFound.Add(videoPath);
            Debug.WriteLine($"CSV FILE NOT FOUND: {videoPath}");
        }
        private static void LogProcessingProgress(int frameCount, int fileCount, string videoPath)
        {
            Debug.WriteLine($"Processing frame {frameCount} of file {fileCount}: {videoPath}");
        }
    }
}
