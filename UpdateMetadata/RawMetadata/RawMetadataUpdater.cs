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
                    LogReextractKlv.LogMissingCsvFile(videoId.PathToVideo);
                }
            }
        }
        private static async Task TryUpdateRawMetadata(TableInstances.VideoID videoId)
        {
            if(!VideoCorupted.CheckFile_Corrupted(videoId.PathToVideo) &&
                TsVideoFileTest.IsValidVideoFile(videoId.PathToVideo))
            {
                (bool csvFound, string csvPath) = FindMatchingCsv_.FindMatchingCsv(videoId);

                if (csvFound)
                {
                    var csvMetadataFields = await
                        CsvToRawMetadata.ReadCSV(csvPath);

                    TestResultsMetadata testResults =
                        await TestManagerMetadata.ValidateMetadata(
                        csvPath, videoId, csvMetadataFields);

                    if (testResults.ShouldUpdate)
                        await UpdateDatabaseRawMetadata(videoId);

                    if (testResults.ShouldReextract)
                        LogReextractKlv.LogMissingCsvFile(videoId.PathToVideo);
                }
                else
                {
                    LogReextractKlv.LogMissingCsvFile(videoId.PathToVideo);
                }
            }
        }
        private static async Task UpdateDatabaseRawMetadata(TableInstances.VideoID videoId) {
            await DeleteFromDb.RemoveOldRawMetadata(videoId);
            await CsvInsertIntoDb.CsvToDB(videoId);
        }
    }
}

