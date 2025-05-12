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
        public static bool testMetadata = true;
        public static SemaphoreSlim extractionSemaphore = new SemaphoreSlim(1, 1);

        public static async Task UpdateRawMetadata()
        {
            if (!int.TryParse(MainWindow.current.skipItemsInput.Text, out int skipItems))
                skipItems = 0;

            List<TableInstances.VideoID> databaseList_VideoID_skipped = SyncY_DriveToDatabase.databaseList_VideoID.Skip(skipItems).ToList();
            List<TableInstances.VideoID> driveList_VideoID_skipped = SyncY_DriveToDatabase.driveList_VideoID.Skip(skipItems).ToList();
            Debug.WriteLine(databaseList_VideoID_skipped.Count + " " + driveList_VideoID_skipped.Count);
            await UpdateRawMetadata(
                databaseList_VideoID_skipped,
                driveList_VideoID_skipped);
        }
        public static async Task UpdateRawMetadata(
            List<TableInstances.VideoID> databaseVideoIds, 
            List<TableInstances.VideoID> driveVideoIds)
        {
            int index = 0;
            foreach (var videoId in databaseVideoIds)
            {
                index++;
                await extractionSemaphore.WaitAsync();
                LogRawMetadataProcess.LogProgress(index, databaseVideoIds.Count);
                try
                {
                    await TryUpdateRawMetadata(videoId);
                }
                catch 
                {
                    LogReextractKlv.LogMissingCsvFile(1, videoId.PathToVideo);
                }
                finally
                {
                    extractionSemaphore.Release();
                }
            }
        }
        public static async Task TryUpdateRawMetadata(TableInstances.VideoID videoId)
        {
            if(!VideoCorupted.CheckFile_Corrupted(videoId.PathToVideo) &&
                TsVideoFileTest.IsValidVideoFile(videoId.PathToVideo))
            {
                (bool csvFound, string csvPath) = FindMatchingCsv_.FindMatchingCsv(videoId);

                if (csvFound && testMetadata)
                {
                    await TestRawMetadata(csvPath, videoId);
                }
                else if (!csvFound)
                {
                    LogReextractKlv.LogMissingCsvFile(0, videoId.PathToVideo);
                }
            }
        }
        private static async Task TestRawMetadata(string csvPath, TableInstances.VideoID videoId)
        {
            Debug.WriteLine("Testing CSV for: " + videoId.PathToVideo);
            var csvMetadataFields = await
                       CsvToRawMetadata.ReadCSV(csvPath);

            TestResultsMetadata testResults =
                await TestManagerMetadata.ValidateMetadata(
                csvPath, videoId, csvMetadataFields);

            if (testResults.ShouldWriteMetadataToDB)
                await UpdateDatabaseRawMetadata(videoId);

            if (testResults.ShouldReextract)
                LogReextractKlv.LogMissingCsvFile(testResults, videoId.PathToVideo);

            LogReextractKlv.LogErrorCounts();
        }
        private static async Task UpdateDatabaseRawMetadata(TableInstances.VideoID videoId) {
            await DeleteFromDb.RemoveOldRawMetadata(videoId);
            await CsvInsertIntoDb.CsvToDB(videoId);
        }
    }
}

