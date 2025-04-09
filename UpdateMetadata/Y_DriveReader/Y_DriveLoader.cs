using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.Y_DriveReader
{
    public static class Y_DriveLoader
    {
        public static async Task<List<TableInstances.VideoID>> 
            GetVidPaths_and_Hash(string directoryPath, string[] filesTypesToSearch)
        {

            List<TableInstances.VideoID> videoID_InstanceList = new List<TableInstances.VideoID>();
            TableInstances.VideoID videoID_Instance;
            try
            {
                foreach (var fileType in filesTypesToSearch)
                {
                    foreach (string filePath in Directory.EnumerateFiles(directoryPath, fileType, SearchOption.AllDirectories))
                    {
                        videoID_Instance = new TableInstances.VideoID();
                        ulong uniqueVideoID = await GenerateUniqueVidHash.GetUniqueFileHashCode(filePath);

                        videoID_Instance.PathToVideo = filePath;
                        videoID_Instance.UniqueVideoID = uniqueVideoID;
                        videoID_InstanceList.Add(videoID_Instance);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            return videoID_InstanceList;
        }
    }
}
