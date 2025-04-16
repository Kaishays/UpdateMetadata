using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpdateMetadata.Y_DriveReader;

namespace UpdateMetadata.WriteDatabase
{
    public static class CsvInsertIntoDb
    {
        public static async Task ManageCsvToDB(List<TableInstances.VideoID> VideoID_FromDatabaseList)
        {
            int count = 0;
            foreach (TableInstances.VideoID videoID_FromDatabase in VideoID_FromDatabaseList)
            {
                count++;
                Debug.WriteLine(count);
                await CsvToDB(videoID_FromDatabase);
            }
        }
        public static async Task CsvToDB(TableInstances.VideoID id)
        {
            string vidPath = id.PathToVideo;
            string csvPath = ConvertTsToCsvPath(vidPath);
            ulong VidID = id.UniqueVideoID;

            if (File.Exists(csvPath))
            {
                if (true)
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
}
