using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using UpdateMetadata.tests;
using ValidateKlvExtraction.Tests;
using Microsoft.WindowsAPICodePack.Shell;
using UpdateMetadata.Y_DriveReader;
using UpdateMetadata.WriteDatabase;

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
            var csvMetadataFields = await 
                CsvToRawMetadata.ReadCSV(csvFilePath);

            bool updateDb = await ShouldUpdateDatabaseKlv(
               videoId, csvMetadataFields);

            if (updateDb)
            {
                await UpdateDatabaseRawMetadata(videoId);
            }

            if (await ShouldReextractKlvData(
                csvFilePath, videoId))
            {
                if(!updateDb)
                    await UpdateDatabaseRawMetadata(videoId);

                LogMissingCsvFile(videoId.PathToVideo);
            }
        }
        private static async Task UpdateDatabaseRawMetadata(TableInstances.VideoID videoId) {
            await DeleteFromDb.RemoveOldRawMetadata(videoId);
            await CsvInsertIntoDb.CsvToDB(videoId);
        }
        private static async Task<bool> ShouldUpdateDatabaseKlv(
           TableInstances.VideoID videoId,
           List<string[]> csvMetadataFields)
        {
            if (VideoCorupted
                .CheckFile_Corrupted(videoId.PathToVideo))
            {
                return false;
            }

            if (await RawKlvInDbTest
                .TestIfRawMetadatraInDB(videoId, csvMetadataFields))
            {
                return false;
            }

            return true;
        }
        private static async Task<bool> ShouldReextractKlvData(
            string csvFilePath, 
            TableInstances.VideoID videoId)
        {
            if (!CsvExcists.IsCsvValid(csvFilePath))
            {
                return false;
            }

            if (!await CsvSizeToVidSizeRatio
                .CheckIfCSV_Video_Threshold(csvFilePath, videoId.PathToVideo))
            {
                return false;
            }

            if (!await UtcTimeTest
                .ValidateUtcTimestamps(csvFilePath))
            {
                return false;
            }

            return true;
        }
        private static void LogMissingCsvFile(string videoPath)
        {
            CsvWriter.ManageCSV_Append("", videoPath);
            Debug.WriteLine($"CSV FILE ERROR: {videoPath}");
        }
        private static void LogProcessingProgress(int frameCount, int fileCount, string videoPath)
        {
            Debug.WriteLine($"Processing frame {frameCount} of file {fileCount}: {videoPath}");
        }
    }
}

