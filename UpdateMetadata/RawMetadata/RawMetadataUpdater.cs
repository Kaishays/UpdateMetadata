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
            var csvMetadataFields = await CsvToRawMetadata.ReadCSV(csvFilePath);
            
            if (await 
                ShouldUpdateMetadata(csvFilePath, videoId, csvMetadataFields))
            {
                await RemoveOldRawMetadata(videoId);
                await AddRawMetadataToDatabase(csvMetadataFields, videoId);
            }
            else
            {
                LogMissingCsvFile(videoId.PathToVideo);
            }
        }
        private static async Task<bool> ShouldUpdateMetadata(
            string csvFilePath, 
            TableInstances.VideoID videoId, 
            List<string[]> csvMetadataFields)
        {
            if (VideoCorupted.CheckFile_Corrupted(videoId.PathToVideo))
            {
                return false;
            }

            if (!CsvExcists.IsCsvValid(csvFilePath))
            {
                return false;
            }

            if (!DoesCsvMatchVideoId(csvFilePath, videoId.PathToVideo))
            {
                return false;
            }

            if (await RawKlvInDbTest.TestIfRawMetadatraInDB(videoId, csvMetadataFields))
            {
                return false;
            }

            if (!await UtcTimeTest.ValidateTimestamps(csvFilePath))
            {
                return false;
            }

            if (!CsvSizeToVidSizeRatio.CheckIfCSV_Video_Threshold(csvFilePath, videoId.PathToVideo))
            {
                return false;
            }

            return true;
        }
        private static bool DoesCsvMatchVideoId(string csvFilePath, string videoPath)
        {
            string csvFileName = Path.GetFileNameWithoutExtension(csvFilePath);
            string videoFileName = Path.GetFileNameWithoutExtension(videoPath);
            
            return csvFileName.Equals(videoFileName, StringComparison.OrdinalIgnoreCase);
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

