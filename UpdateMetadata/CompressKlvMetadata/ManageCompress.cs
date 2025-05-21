using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata.RawMetadata;
using UpdateMetadata.tests;
using UpdateMetadata.WriteDatabase;
using UpdateMetadata.Y_DriveReader;
using ValidateKlvExtraction.Tests;

namespace UpdateMetadata.CompressKlvMetadata
{
    public static class ManageCompress
    {
        public static bool compressMetadata = true;
        public static SemaphoreSlim compressMetadataSemaphore = new SemaphoreSlim(1, 1);

        public static async Task CompressRawMetadata()
        {
            await CompressRawMetadata(
                SyncY_DriveToDatabase.databaseList_VideoID,
                SyncY_DriveToDatabase.driveList_VideoID);
        }
        public static async Task CompressRawMetadata(
            List<TableInstances.VideoID> databaseVideoIds,
            List<TableInstances.VideoID> driveVideoIds)
        {
            int index = 0;
            foreach (var videoId in databaseVideoIds)
            {
                index++;
                await compressMetadataSemaphore.WaitAsync();
               // LogRawMetadataProcess.LogProgress(index, databaseVideoIds.Count);
                try
                {
                    await TryCompressRawMetadata(videoId);
                }
                catch
                {
                   // LogReextractKlv.LogMissingCsvFile(1, videoId.PathToVideo);
                }
                finally
                {
                    compressMetadataSemaphore.Release();
                }
            }
        }
        public static async Task TryCompressRawMetadata(TableInstances.VideoID videoId)
        {
           await MaxMinKlvCalc.ProcessSingleVideo(videoId);


            // flight distance
            // lat lon  sainity  check 
            // graph averaging 
            // 

        }

    }
}
