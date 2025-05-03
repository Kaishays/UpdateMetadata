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
        public static bool testMetadata = false;
        
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
                try
                {
                    await TryUpdateRawMetadata(videoId);
                }
                catch {
                    LogReextractKlv.LogMissingCsvFile(1, videoId.PathToVideo);
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
            var csvMetadataFields = await
                       CsvToRawMetadata.ReadCSV(csvPath);

            TestResultsMetadata testResults =
                await TestManagerMetadata.ValidateMetadata(
                csvPath, videoId, csvMetadataFields);

            if (testResults.ShouldWriteMetadataToDB)
                await UpdateDatabaseRawMetadata(videoId);

            if (testResults.ShouldReextract)
                LogReextractKlv.LogMissingCsvFile(testResults, videoId.PathToVideo);
        }
        private static async Task UpdateDatabaseRawMetadata(TableInstances.VideoID videoId) {
            await DeleteFromDb.RemoveOldRawMetadata(videoId);
            await CsvInsertIntoDb.CsvToDB(videoId);
        }
    }
}

