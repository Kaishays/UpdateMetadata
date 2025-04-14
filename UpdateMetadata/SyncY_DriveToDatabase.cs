using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateMetadata.ReadDatabase;
using UpdateMetadata.Y_DriveReader;
using UpdateMetadata.CompressKlvMetadata;
using UpdateMetadata.WriteDatabase;
using System.Configuration;

namespace UpdateMetadata
{
    public static class SyncY_DriveToDatabase
    {
        public static List<TableInstances.VideoID> driveList_VideoID = null;
        public static List<TableInstances.VideoID> databaseList_VideoID = null;
        public static List<string> OnlyForDebug_PotentialCSVFileNotFound = new List<string>();
        
        public static async Task SyncDriveToDB()
        {
            try
            {
                Debug.WriteLine("Starting Loading Lists");
                await LoadDrive_and_Database_into_Lists();
                Debug.WriteLine("Finished Loading Lists");

                Debug.WriteLine("Deleting From Database");
                await DeleteFromDb.DeleteExcessVideosID_FromDatabase();

                Debug.WriteLine("Adding To Database");
                await AddToDb.AddNewVideosID_ToDatabase();

                if (!CompareVidIds())
                    return;

                Debug.WriteLine("Updating Path To Video");
                await PathToVideoUpdater.UpdatePathToVideo();

                Debug.WriteLine("Updating Raw Metadata. Will print <Done> when complete");
                await RawMetadataUpdater.UpdateRawMetadata();

                Debug.WriteLine("Done");
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error: " + e.ToString());
            }
        }
        private static bool CompareVidIds()
        {
            if (databaseList_VideoID.Count != driveList_VideoID.Count)
            {
                return false;
                throw new Exception("Error synchronizing VideoID: Lists have different counts");
            }

            var databaseIds = new HashSet<ulong>(databaseList_VideoID.Select(vid => vid.UniqueVideoID));
            var driveIds = new HashSet<ulong>(driveList_VideoID.Select(vid => vid.UniqueVideoID));
            
            if (!databaseIds.SetEquals(driveIds))
            {
                return false;
                throw new Exception("Error synchronizing VideoID: Lists contain different video IDs");
            }
            return true;
        }

        private static async Task LoadDrive_and_Database_into_Lists()
        {
            var loadDriveTask = Y_DriveLoader.GetVidPaths_and_Hash(NameLibrary.General.pathToDrive, new string[] { "*.ts", "*.mp4" });
            var loadDatabaseTask = 
                VidIDGetter.GetVid_Ids_FromDb();

            await Task.WhenAll(loadDriveTask, loadDatabaseTask);

            driveList_VideoID = await loadDriveTask;
            databaseList_VideoID = await loadDatabaseTask;
            Debug.WriteLine("done");
        }
    }
}
