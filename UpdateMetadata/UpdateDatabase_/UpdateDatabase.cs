using MetaDatabaseLibrary.DBLogic;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System.IO;
using static MetaDatabaseLibrary.DBLogic.Scanner;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Xml;
using System.Globalization;
using static MetaDatabaseLibrary.DBLogic.CompressMetadata.MaxMin;
using System.Reflection.Metadata;
namespace MetaDatabaseLibrary.DBLogic
{
    public static class UpdateDatabase
    {
        private static List<TableInstances.VideoID> VideoID_FromDriveList = null;
        private static List<TableInstances.VideoID> VideoID_FromDatabaseList = null;
        private static List<string> OnlyForDebug_PotentialCSVFileNotFound = new List<string>();
        public static async Task SynchronizeVideosID_FromDrive_and_Database()
        {
            try
            {
                Debug.WriteLine("Starting Loading Lists");
                await LoadDrive_and_Database_into_Lists();
                Debug.WriteLine("Finished Loading Lists");

                Debug.WriteLine("Deleting From Database");
                await DeleteExcessVideosID_FromDatabase();
                Debug.WriteLine("Adding To Database");
                await AddNewVideosID_ToDatabase();

                if (VideoID_FromDatabaseList.Count() != VideoID_FromDriveList.Count())
                {
                    throw new Exception("Error synchronizing VideoID");
                }
                Debug.WriteLine("Updating Path To Video");
                await UpdatePathToVideo();
                Debug.WriteLine("Updating Raw Metadata. Will print <Done> when complete");

                //await UpdateRawMetadata();
                Debug.WriteLine("Done");
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error: " + e.ToString());
            }
        }
        private static async Task LoadDrive_and_Database_into_Lists()
        {
            await CompressMetadata.ManageCalc();
            var loadDriveTask = LoadDrive();
            var loadDatabaseTask = LoadDatabase();

            await Task.WhenAll(loadDriveTask, loadDatabaseTask);

            VideoID_FromDriveList = await loadDriveTask;
            VideoID_FromDatabaseList = await loadDatabaseTask;
            Debug.WriteLine("done");
        }
        private static async Task<List<TableInstances.VideoID>> LoadDrive()
        {
            List<TableInstances.VideoID> videoID_FromDrive = await Scanner.ScanDirectory.LoadVideos_from_Drive_Into_VideoID_List(NameLibrary.General.pathToDrive, new string[] { "*.ts", "*.mp4" });
            return videoID_FromDrive;
        }
        private static async Task<List<TableInstances.VideoID>> LoadDatabase()
        {
            List<TableInstances.VideoID> videoID = await ScanDatabase.LoadAllVideoIDs_Database(NameLibrary.General.connectionString);
            return videoID;
        }
        private static async Task DeleteExcessVideosID_FromDatabase()
        {
            List<TableInstances.VideoID> VideoID_ToRemove = new List<TableInstances.VideoID>();

            foreach (TableInstances.VideoID videoID_FromDatabase in VideoID_FromDatabaseList)
            {
                if (VideoID_FromDriveList.Any(videoID_FromDrive => videoID_FromDrive.UniqueVideoID == videoID_FromDatabase.UniqueVideoID))
                {
                }
                else
                {
                    await MySQLDataAccess.ExecuteSQL(SQL_QueriesStore.RawMetadata.deleteFrom, videoID_FromDatabase, NameLibrary.General.connectionString);
                    await MySQLDataAccess.ExecuteSQL(SQL_QueriesStore.VideoID.deleteFrom, videoID_FromDatabase, NameLibrary.General.connectionString);
                    VideoID_ToRemove.Add(videoID_FromDatabase);
                }
            }
            foreach (TableInstances.VideoID videoID_FromDatabase in VideoID_ToRemove)
            {
                VideoID_FromDatabaseList.Remove(videoID_FromDatabase);
            }
        }
        public static async Task AddNewVideosID_ToDatabase()
        {
            if (VideoID_FromDatabaseList.Count() < VideoID_FromDriveList.Count())
            {
                List<TableInstances.VideoID> videoID_ToAdd = new List<TableInstances.VideoID>();

                foreach (TableInstances.VideoID videoID_FromDrive in VideoID_FromDriveList)
                {
                    if (VideoID_FromDatabaseList.Any(videoID_FromDatabase => videoID_FromDatabase.UniqueVideoID != videoID_FromDrive.UniqueVideoID))
                    {
                        await MySQLDataAccess.ExecuteSQL(SQL_QueriesStore.VideoID.addTo, videoID_FromDrive, NameLibrary.General.connectionString);
                        videoID_ToAdd.Add(videoID_FromDrive);
                    }
                    else if (VideoID_FromDatabaseList.Count == 0)
                    {
                        await MySQLDataAccess.ExecuteSQL(SQL_QueriesStore.VideoID.addTo, videoID_FromDrive, NameLibrary.General.connectionString);
                        videoID_ToAdd.Add(videoID_FromDrive);
                    }
                }
                foreach (TableInstances.VideoID videoID in videoID_ToAdd)
                {
                    VideoID_FromDatabaseList.Add(videoID);
                }
            }
        }
        public static async Task UpdatePathToVideo()
        {
            foreach (TableInstances.VideoID videoID_FromDrive in VideoID_FromDriveList)
            {
                foreach (TableInstances.VideoID videoID_FromDatabase in VideoID_FromDatabaseList)
                {
                    if (videoID_FromDrive.UniqueVideoID == videoID_FromDatabase.UniqueVideoID)
                    {
                        if (videoID_FromDrive.PathToVideo != videoID_FromDatabase.PathToVideo)
                        {
                            await MySQLDataAccess.ExecuteSQL(SQL_QueriesStore.VideoID.insertPathToVideo, videoID_FromDrive, NameLibrary.General.connectionString);
                        }
                        break;
                    }
                }
            }
        }
        private static int count_File = 0;
        public static async Task UpdateRawMetadata()
        {
            foreach (TableInstances.VideoID videoID_FromDatabase in VideoID_FromDatabaseList)
            {
                string? dirName = Path.GetDirectoryName(videoID_FromDatabase.PathToVideo);

                foreach (string CSV_FilePath in Directory.EnumerateFiles(dirName, "*.csv", SearchOption.TopDirectoryOnly))
                {
                    count_File++;
                    List<string[]> allFieldsInCSV = await Scanner.ScanCSV_File.ReadCSV(CSV_FilePath);

                    if (CheckCSV.CheckAll(CSV_FilePath, videoID_FromDatabase.PathToVideo) &&
                        !await CheckIfRawInDB.CheckRawDB(videoID_FromDatabase, allFieldsInCSV))
                    {
                        await AddRawMetadata(allFieldsInCSV, videoID_FromDatabase);
                        break;
                    }
                    else
                    {
                        OnlyForDebug_PotentialCSVFileNotFound.Add(videoID_FromDatabase.PathToVideo);
                        Debug.WriteLine("CSV FILE NOT FOUND: " + videoID_FromDatabase.PathToVideo);
                    }
                }
            }
        }
        public static async Task AddRawMetadata(List<string[]> allFieldsInCSV, TableInstances.VideoID videoID_FromDatabase)
        {
            List<TableInstances.RawMetadata> rawMetadata_InstanceList = Scanner.ScanCSV_File.CreateRawMetadataInstances(allFieldsInCSV, videoID_FromDatabase);
            int count_Frame = 0;
            foreach (TableInstances.RawMetadata rawMetadata_Instance in rawMetadata_InstanceList)
            {
                count_Frame++;
                Debug.WriteLine("Working: " + count_Frame + "   " + count_File + " " + videoID_FromDatabase.PathToVideo);
                await MySQLDataAccess.ExecuteSQL<TableInstances.RawMetadata>(SQL_QueriesStore.RawMetadata.addTo, rawMetadata_Instance, NameLibrary.General.connectionString);
            }
        }
        public static class CheckCSV
        {
            private static double highRatioOfCSV_ToTS_Size = .5;
            private static double lowRatioOfCSV_ToTS_Size = .01;
            public static bool CheckAll(string csv, string ts)
            {
                return CheckIfCSV_Video_Threshold(csv, ts) && CheckFileNameMatch(csv, ts);
            }
            public static long GetFileSize(string filePath)
            {

                FileInfo fileInfo = new FileInfo(filePath);
                long fileSize = fileInfo.Length;
                return fileSize;
            }
            public static string ConvertFilePathToCSV(string filePath)
            {
                return Path.ChangeExtension(filePath, ".csv");
            }
            public static bool CheckIfCSV_Video_Threshold(string csv, string ts)
            {
                double csvSize = GetFileSize(csv);
                double tsSize = GetFileSize(ts);
                double ratio = (double)csvSize / tsSize;

                double hL = highRatioOfCSV_ToTS_Size;
                double lL = lowRatioOfCSV_ToTS_Size;

                if (ratio <= hL && ratio >= lL)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool CheckFileNameMatch(string csv, string ts)
            {
                string fileName_videoID_FromDatabase = Path.GetFileNameWithoutExtension(ts);
                string filename_CSV = Path.GetFileNameWithoutExtension(csv);

                return (filename_CSV.Equals(fileName_videoID_FromDatabase, StringComparison.OrdinalIgnoreCase));
            }
        }
        public static class CheckIfRawInDB
        {
            public static async Task<bool> CheckRawDB(TableInstances.VideoID vidID_Instance, List<string[]> allFieldsInCSV)
            {
                int rowCt = await GetRowCount(vidID_Instance);
                int csvCt = allFieldsInCSV.Count();

                if (rowCt == csvCt)
                    return true;
                else
                    return false;
            }
            public static async Task<int> GetRowCount(TableInstances.VideoID vidID_Instance)
            {
                List<int> rowCt = await
                     MySQLDataAccess.QuerySQL<int, TableInstances.VideoID>(
                         SQL_QueriesStore.RawMetadata.getRowCount,
                         vidID_Instance,
                         NameLibrary.General.connectionString);
                return rowCt[0];
            }



        }
        public static class DirectCsvToDB
        {
            // list of all vid id in db
            // Generate raw metadata class from csv 
            // add each csv into raw metadata for each instance 

            public static async Task ManageCsvToDB(List<TableInstances.VideoID> VideoID_FromDatabaseList)
            {
                int count = 0;
                foreach (TableInstances.VideoID videoID_FromDatabase in VideoID_FromDatabaseList)
                {
                    count++;
                    Debug.WriteLine(count);
                    await UpdateDatabase.DirectCsvToDB.CsvToDB(videoID_FromDatabase);
                }
            }
            public static async Task CsvToDB(TableInstances.VideoID id)
            {
                string vidPath = id.PathToVideo;
                string csvPath = ConvertTsToCsvPath(vidPath);
                ulong VidID = id.UniqueVideoID;

                if (File.Exists(csvPath))
                {
                    if (CheckCSV.CheckAll(csvPath, vidPath))
                    {
                        string sql = BuildSQL_Local(vidPath, VidID);
                        if (sql != "no Csv Found")
                            try
                            {
                                await MySQLDataAccess.ExecuteSQL(sql, NameLibrary.General.connectionString);
                            }
                            catch
                            {

                            }
                    }
                }
            }
            public static string ConvertTsToCsvPath(string tsFilePath)
            {
                if (string.IsNullOrWhiteSpace(tsFilePath))
                    throw new ArgumentException("File path cannot be null or empty.", nameof(tsFilePath));

                return ConvertToSQlFormat(Path.ChangeExtension(tsFilePath, ".csv"));
            }
            public static string ConvertToSQlFormat(string path)
            {
                return path.Replace("\\", "/");
            }
            public static string BuildSQL_Local(string filepath, ulong VidID)
            {
                // Ensure the file path uses forward slashes for MySQL compatibility
                filepath = ConvertTsToCsvPath(filepath);
                if (!File.Exists(filepath))
                    return "no Csv Found";
                // Construct the SQL query dynamically
                string sqlQuery = $@"
                LOAD DATA LOCAL INFILE '{filepath}'
                INTO TABLE raw_metadata
                FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '""'
                LINES TERMINATED BY '\n'
                IGNORE 1 LINES
                (
                   @rawUtcTime,
                   PlatformHeading,
                   PlatformPitch,
                   PlatformRoll,
                   SensorName,
                   SensorLatitude,
                   SensorLongitude,
                   SensorAltitude_m,
                   SensorRelativeRoll,
                   SensorRelativeAzimuth,
                   SensorRelativeElevation,
                   SensorHorizontalFovDegrees,
                   SensorVerticalFovDegrees,
                   FrameElevation,
                   FrameLat,
                   FrameLon,
                   TargetLat,
                   TargetLon,
                   TargetElevation,
                   SlantRange_m
                )
                SET UtcTime = STR_TO_DATE(@rawUtcTime, '%Y-%m-%dT%H:%i:%s.%fZ'),
                    FrameIndex = DEFAULT,
                    UniqueVideoID = {VidID};";

                return sqlQuery;
            }

        }

        public static async void CheckWhereUtcTimeNull()
        {
            string x = "SELECT UniqueVideoID\r\nFROM Metadatabase.raw_metadata\r\nWHERE UtcTime IS NULL\r\nGROUP BY UniqueVideoID;\r\n";
            string y = "SELECT * FROM metadatabase.video_id WHERE UniqueVideoID IN @fileID_List";


            List<ulong> fileID_List = await MySQLDataAccess.QuerySQL<ulong, dynamic>(x, null, NameLibrary.General.connectionString);
            int count = 0;
            foreach (ulong id in fileID_List)
            {
                string delete = $"delete FROM metadatabase.raw_metadata where UniqueVideoID  = {id}";
                // await MySQLDataAccess.ExecuteSQL(delete, NameLibrary.General.connectionString);
                count++;
                Debug.WriteLine(count);
            }

            // delete rawmeta data that contain any of fileID_List 
            // refil  the raw metadata usign fileID_and_Path_List 


            List<TableInstances.VideoID> fileID_and_Path_List = await MySQLDataAccess.QuerySQL<TableInstances.VideoID, List<ulong>>(y, fileID_List, NameLibrary.General.connectionString);

            foreach (TableInstances.VideoID videoID_FromDatabase in fileID_and_Path_List)
            {
                string? dirName = Path.GetDirectoryName(videoID_FromDatabase.PathToVideo);

                foreach (string CSV_FilePath in Directory.EnumerateFiles(dirName, "*.csv", SearchOption.TopDirectoryOnly))
                {
                    count_File++;
                    List<string[]> allFieldsInCSV = await Scanner.ScanCSV_File.ReadCSV(CSV_FilePath);

                    if (CheckCSV.CheckAll(CSV_FilePath, videoID_FromDatabase.PathToVideo) &&
                        !await CheckIfRawInDB.CheckRawDB(videoID_FromDatabase, allFieldsInCSV))
                    {
                        await AddRawMetadata(allFieldsInCSV, videoID_FromDatabase);
                        break;
                    }
                    else
                    {
                        OnlyForDebug_PotentialCSVFileNotFound.Add(videoID_FromDatabase.PathToVideo);
                        Debug.WriteLine("CSV FILE NOT FOUND: " + videoID_FromDatabase.PathToVideo);
                    }
                }
            }

            Debug.WriteLine(fileID_and_Path_List.Count);
        }
    }
    public class Scanner
    {
        public static class ScanDirectory
        {
            public static TableInstances.VideoID videoID_Instance;
            public static async Task<List<TableInstances.VideoID>> LoadVideos_from_Drive_Into_VideoID_List(string directoryPath, string[] filesTypesToSearch)
            {
                List<TableInstances.VideoID> videoID_InstanceList = new List<TableInstances.VideoID>();
                try
                {
                    foreach (var fileType in filesTypesToSearch)
                    {
                        foreach (string filePath in Directory.EnumerateFiles(directoryPath, fileType, SearchOption.AllDirectories))
                        {
                            videoID_Instance = new TableInstances.VideoID();
                            ulong uniqueVideoID = await GenerateUniqueVideoID.GetUniqueFileIndex(filePath);

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
        public static class ScanDatabase
        {
            public static async Task<List<TableInstances.VideoID>> LoadAllVideoIDs_Database(string connectionString)
            {
                List<TableInstances.VideoID> fileID_and_Path_List = new List<TableInstances.VideoID>();

                try
                {
                    fileID_and_Path_List = await MySQLDataAccess.QuerySQL<TableInstances.VideoID, dynamic>(SQL_QueriesStore.VideoID.selectFrom, null, connectionString);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                }
                return fileID_and_Path_List;
            }
        }
        public static class ScanCSV_File
        {
            public static async Task<List<string[]>> ReadCSV(string csvFilePath)
            {
                List<string[]> allFieldsInCSV = new List<string[]>();
                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    string? line = await reader.ReadLineAsync();
                    while (line != null)
                    {
                        string[] fields = line.Split(',');
                        allFieldsInCSV.Add(fields);

                        line = await reader.ReadLineAsync();
                    }
                }
                DeleteCSV_FileColumnHeaders(allFieldsInCSV);
                return allFieldsInCSV;
            }
            public static List<TableInstances.RawMetadata> CreateRawMetadataInstances(List<string[]> allFramesInCSV, TableInstances.VideoID videoID_FromDatabase)
            {
                List<TableInstances.RawMetadata> allFrames_RawMetadataList = new List<TableInstances.RawMetadata>();
                TableInstances.RawMetadata rawMetadata;
                int frameIndex = 0;
                foreach (string[] frameMetadata in allFramesInCSV)
                {
                    rawMetadata = new TableInstances.RawMetadata();
                    rawMetadata.UniqueVideoID = videoID_FromDatabase.UniqueVideoID;
                    rawMetadata.FrameIndex = frameIndex;
                    frameIndex++;
                    rawMetadata.UtcTime = ParseUtcTime(frameMetadata[0]);
                    rawMetadata.PlatformHeading = double.Parse(frameMetadata[1]);
                    rawMetadata.PlatformPitch = double.Parse(frameMetadata[2]);
                    rawMetadata.PlatformRoll = double.Parse(frameMetadata[3]);
                    rawMetadata.SensorName = frameMetadata[4];
                    rawMetadata.SensorLatitude = double.Parse(frameMetadata[5]);
                    rawMetadata.SensorLongitude = double.Parse(frameMetadata[6]);
                    rawMetadata.SensorAltitude_m = double.Parse(frameMetadata[7]);
                    rawMetadata.SensorRelativeRoll = double.Parse(frameMetadata[8]);
                    rawMetadata.SensorRelativeAzimuth = double.Parse(frameMetadata[9]);
                    rawMetadata.SensorRelativeElevation = double.Parse(frameMetadata[10]);
                    rawMetadata.SensorHorizontalFovDegrees = double.Parse(frameMetadata[11]);
                    rawMetadata.SensorVerticalFovDegrees = double.Parse(frameMetadata[12]);
                    rawMetadata.FrameElevation = double.Parse(frameMetadata[13]);
                    rawMetadata.FrameLat = double.Parse(frameMetadata[14]);
                    rawMetadata.FrameLon = double.Parse(frameMetadata[15]);
                    rawMetadata.TargetLat = double.Parse(frameMetadata[16]);
                    rawMetadata.TargetLon = double.Parse(frameMetadata[17]);
                    rawMetadata.TargetElevation = double.Parse(frameMetadata[18]);
                    rawMetadata.SlantRange_m = double.Parse(frameMetadata[19]);

                    allFrames_RawMetadataList.Add(rawMetadata);
                }
                return allFrames_RawMetadataList;
            }
            private static DateTime ParseUtcTime(string UtcTimeStr)
            {
                try
                {
                    DateTime UtcTime;
                    UtcTimeStr = UtcTimeStr.Trim();
                    UtcTime = DateTime.ParseExact(
                        UtcTimeStr,
                        "yyyy-MM-ddTHH:mm:ss.FFFFFFZ",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal);

                    return UtcTime;
                }
                catch
                {
                    return default(DateTime);
                }
            }
            private static void DeleteCSV_FileColumnHeaders(List<string[]> allFields)
            {
                allFields.RemoveAt(0);
            }
        }
    }
    public static class CompressMetadata
    {
        public static async Task ManageCalc()
        {
            await MaxMin.ManageMM();

            //await CalcFlightDis.ManageCalcFlightDis();
        }
        public static class CalcFlightDis
        {
            public static async Task ManageCalcFlightDis()
            {
                List<ulong> videoIds = await GetVideoIds();
                List<double> lat = new List<double>();
                List<double> lon = new List<double>();
                double flightKilo = 0;

                foreach (ulong videoId in videoIds)
                {
                    lat = await GetLat(videoId);
                    lon = await GetLong(videoId);

                    for (int i = 1; i < lat.Count - 1; i += 1)
                    {
                        double calc = Haversine(lat[i - 1], lon[i - 1], lat[i], lon[i]);
                        flightKilo += calc;

                        Debug.WriteLine("Calc Haversin");
                    }
                    /* TableInstances.CompressedMetadata compressedMetadata = new TableInstances.CompressedMetadata();
                     compressedMetadata.UniqueVideoID = videoId;
                     compressedMetadata.DistanceTraveled = flightKilo;
                     await MySQLDataAccess.ExecuteSQL<TableInstances.CompressedMetadata>(SQL_QueriesStore.CompressedMetadata.addTo, compressedMetadata, NameLibrary.General.connectionString);
                     flightKilo = 0;*/
                }
            }
            public static async Task<List<ulong>> GetVideoIds()
            {
                List<TableInstances.VideoID> fileID_and_Path_List = new List<TableInstances.VideoID>();
                List<ulong> videoIds = new List<ulong>();

                fileID_and_Path_List = await MySQLDataAccess.QuerySQL<TableInstances.VideoID, dynamic>(
                    SQL_QueriesStore.VideoID.selectFrom,
                    null, NameLibrary.General.connectionString);

                foreach (TableInstances.VideoID f in fileID_and_Path_List)
                {
                    videoIds.Add(f.UniqueVideoID);
                }
                return videoIds;
            }
            public static async Task<List<double>> GetLat(ulong videoID)
            {
                List<double> doubleGraphValues =
                    await MySQLDataAccess.QuerySQL<double,
                    dynamic>(SQL_QueriesStore.GraphQueries.getSensorLatitude,
                    new { UniqueVideoID = videoID },
                    NameLibrary.General.connectionString);
                return doubleGraphValues;
            }
            public static async Task<List<double>> GetLong(ulong videoID)
            {
                List<double> doubleGraphValues =
                    await MySQLDataAccess.QuerySQL<double,
                    dynamic>(SQL_QueriesStore.GraphQueries.getSensorLongitude,
                    new { UniqueVideoID = videoID },
                    NameLibrary.General.connectionString);
                return doubleGraphValues;
            }

            public static double Haversine(double lat1, double lon1,
                            double lat2, double lon2)
            {
                // distance between latitudes and longitudes
                double dLat = (Math.PI / 180) * (lat2 - lat1);
                double dLon = (Math.PI / 180) * (lon2 - lon1);

                // convert to radians
                lat1 = (Math.PI / 180) * (lat1);
                lat2 = (Math.PI / 180) * (lat2);

                // apply formulae
                double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                           Math.Pow(Math.Sin(dLon / 2), 2) *
                           Math.Cos(lat1) * Math.Cos(lat2);
                double rad = 6371; // earths radius in kil
                double c = 2 * Math.Asin(Math.Sqrt(a));
                return rad * c;
            }
        }
        public static class MaxMin
        {
            public class CompressedMetadataTemp
            {
                // Unique identifier for the video.
                public ulong UniqueVideoID { get; set; }

                // Frame index minimum and maximum.
                public int? FrameIndex_min { get; set; }
                public int? FrameIndex_max { get; set; }

                // UTC time minimum and maximum.
                public DateTime? UtcTime_min { get; set; }
                public DateTime? UtcTime_max { get; set; }

                // Platform heading, pitch, and roll (min and max).
                public double? PlatformHeading_min { get; set; }
                public double? PlatformHeading_max { get; set; }

                public double? PlatformPitch_min { get; set; }
                public double? PlatformPitch_max { get; set; }

                public double? PlatformRoll_min { get; set; }
                public double? PlatformRoll_max { get; set; }

                // A concatenated list of distinct sensor names.
                public string SensorName_unique { get; set; }

                // Sensor latitude and longitude (min and max).
                public double? SensorLatitude_min { get; set; }
                public double? SensorLatitude_max { get; set; }

                public double? SensorLongitude_min { get; set; }
                public double? SensorLongitude_max { get; set; }

                // Sensor altitude in meters (min and max).
                public double? SensorAltitude_m_min { get; set; }
                public double? SensorAltitude_m_max { get; set; }

                // Sensor relative roll, azimuth, and elevation (min and max).
                public double? SensorRelativeRoll_min { get; set; }
                public double? SensorRelativeRoll_max { get; set; }

                public double? SensorRelativeAzimuth_min { get; set; }
                public double? SensorRelativeAzimuth_max { get; set; }

                public double? SensorRelativeElevation_min { get; set; }
                public double? SensorRelativeElevation_max { get; set; }

                // Sensor horizontal and vertical field of view in degrees (min and max).
                public float? SensorHorizontalFovDegrees_min { get; set; }
                public float? SensorHorizontalFovDegrees_max { get; set; }

                public float? SensorVerticalFovDegrees_min { get; set; }
                public float? SensorVerticalFovDegrees_max { get; set; }

                // Frame elevation (min and max).
                public float? FrameElevation_min { get; set; }
                public float? FrameElevation_max { get; set; }

                // Frame latitude and longitude (min and max).
                public float? FrameLat_min { get; set; }
                public float? FrameLat_max { get; set; }

                public float? FrameLon_min { get; set; }
                public float? FrameLon_max { get; set; }

                // Target latitude and longitude (min and max).
                public float? TargetLat_min { get; set; }
                public float? TargetLat_max { get; set; }

                public float? TargetLon_min { get; set; }
                public float? TargetLon_max { get; set; }

                // Target elevation (min and max).
                public float? TargetElevation_min { get; set; }
                public float? TargetElevation_max { get; set; }

                // Slant range in meters (min and max).
                public float? SlantRange_m_min { get; set; }
                public float? SlantRange_m_max { get; set; }
            }
            public static async Task ManageMM()
            {
                List<CompressedMetadataTemp> compressedMetadataTempList
                 = await MaxMin.CalcMaxMin();
                foreach (CompressedMetadataTemp x in compressedMetadataTempList)
                {
                    await MySQLDataAccess.ExecuteSQL<CompressedMetadataTemp>(
                    SQL_QueriesStore.CompressedMetadata.InsertCompressedMetadataQuery,
                    x,
                    NameLibrary.General.connectionString);
                }
            }
            public static async Task<List<CompressedMetadataTemp>> CalcMaxMin()
            {
                return await MySQLDataAccess.QuerySQL<CompressedMetadataTemp>(
                    SQL_QueriesStore.CompressedMetadata.calc, NameLibrary.General.connectionString);
            }
        }
        public static class VidDur // Not implemented //
        {
            /*public static async Task ManageDur()
            {
                List<TableInstances.VideoID> allIds = await GetVideoPaths();

            }
            public static async Task<List<TableInstances.VideoID>> GetVideoPaths()
            {
                List<TableInstances.VideoID> fileID_and_Path_List = new List<TableInstances.VideoID>();

                fileID_and_Path_List = await MySQLDataAccess.QuerySQL<TableInstances.VideoID, dynamic>(
                    SQL_QueriesStore.VideoID.selectFrom,
                    null, NameLibrary.General.connectionString);

                return fileID_and_Path_List;
            }
            public class TempDurationID
            {
                public TableInstances.VideoID videoID1;
                public double dur;
                public TempDurationID(double _dur, TableInstances.VideoID _videoID1)
                {
                    videoID1 = _videoID1;
                    dur = _dur;
                }
            }
            public static async Task<List<double>> GetDur(List<TableInstances.VideoID> allIds)
            {
                List<TempDurationID> durationResults = new List<TempDurationID>();

                foreach (var id in allIds)
                {
                    string path = id.PathToVideo;

                    // Replace with your actual asynchronous method to compute the video duration.
                    double duration = await VideoDurationCalculator.GetDurationAsync(path);

                    TempDurationID result = new TempDurationID(duration, id);
                    durationResults.Add(result);
                }

                return duration
                    }
            public static class CreateCompredM_Class
            {

            }*/
        }
    }
    /*Summary
    * GetFileInformationByHandle is built into fileapi.h.
    * Each query takes 1 ms to 15 ms on Y:\ drive so 16.5 sec to 4.13 minutes to get unique ID for 16500 files. 
    * https://learn.microsoft.com/en-us/windows/win32/api/fileapi/ns-fileapi-by_handle_file_information 
    * If path tableName / file tableName is changed the ID will not change.
    * If the file is moved the ID will change.
    * If the file is copy and pasted to a new 
    * 
    * location the ID will change.
    Summary */
    public class GenerateUniqueVideoID
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        [StructLayout(LayoutKind.Sequential)]
        struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]

        struct BY_HANDLE_FILE_INFORMATION
        {
            public uint FileAttributes;
            public FILETIME CreationTime;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }
        public static async Task<ulong> GetUniqueFileIndex(string filePath)
        {
            using (SafeFileHandle fileHandle = File.OpenHandle(filePath))
            {
                if (GetFileInformationByHandle(fileHandle, out BY_HANDLE_FILE_INFORMATION fileInfo))
                {
                    ulong uniqueID = ((ulong)fileInfo.FileIndexHigh << 32) | fileInfo.FileIndexLow;
                    return await Task.FromResult(uniqueID);
                }
                throw new IOException("Unable to get file information.");
            }
        }
    }
}
