using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Tls.Crypto;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
namespace UpdateMetadata
{
    public static class NameLibrary
    {
        public static async Task CreateNameLibrary()
        {
            #region dataset_links
            Tables.Dataset_Links.tableName = await Tables.GetTableNames(1).ConfigureAwait(false);

            #endregion
            #region datasets
            Tables.Datasets.tableName = await Tables.GetTableNames(2).ConfigureAwait(false);

            #endregion

            #region RawMetadata
            Tables.RawMetadata.tableName = await Tables.GetTableNames(3).ConfigureAwait(false);
            Tables.RawMetadata.column1_UniqueVideoID = await Tables.GetColumnNamesLogic(3, 0).ConfigureAwait(false);
            Tables.RawMetadata.column2_FrameIndex = await Tables.GetColumnNamesLogic(3, 1).ConfigureAwait(false);
            Tables.RawMetadata.column3_UtcTime = await Tables.GetColumnNamesLogic(3, 2).ConfigureAwait(false);
            Tables.RawMetadata.column4_PlatformHeading = await Tables.GetColumnNamesLogic(3, 3).ConfigureAwait(false);
            Tables.RawMetadata.column5_PlatformPitch = await Tables.GetColumnNamesLogic(3, 4).ConfigureAwait(false);
            Tables.RawMetadata.column6_PlatformRoll = await Tables.GetColumnNamesLogic(3, 5).ConfigureAwait(false);
            Tables.RawMetadata.column7_SensorName = await Tables.GetColumnNamesLogic(3, 6).ConfigureAwait(false);
            Tables.RawMetadata.column8_SensorLatitude = await Tables.GetColumnNamesLogic(3, 7).ConfigureAwait(false);
            Tables.RawMetadata.column9_SensorLongitude = await Tables.GetColumnNamesLogic(3, 8).ConfigureAwait(false);
            Tables.RawMetadata.column10_SensorAltitude_m = await Tables.GetColumnNamesLogic(3, 9).ConfigureAwait(false);
            Tables.RawMetadata.column11_SensorRelativeRoll = await Tables.GetColumnNamesLogic(3, 10).ConfigureAwait(false);
            Tables.RawMetadata.column12_SensorRelativeAzimuth = await Tables.GetColumnNamesLogic(3, 11).ConfigureAwait(false);
            Tables.RawMetadata.column13_SensorRelativeElevation = await Tables.GetColumnNamesLogic(3, 12).ConfigureAwait(false);
            Tables.RawMetadata.column14_SensorHorizontalFovDegrees = await Tables.GetColumnNamesLogic(3, 13).ConfigureAwait(false);
            Tables.RawMetadata.column15_SensorVerticalFovDegrees = await Tables.GetColumnNamesLogic(3, 14).ConfigureAwait(false);
            Tables.RawMetadata.column16_FrameElevation = await Tables.GetColumnNamesLogic(3, 15).ConfigureAwait(false);
            Tables.RawMetadata.column17_FrameLat = await Tables.GetColumnNamesLogic(3, 16).ConfigureAwait(false);
            Tables.RawMetadata.column18_FrameLon = await Tables.GetColumnNamesLogic(3, 17).ConfigureAwait(false);
            Tables.RawMetadata.column19_TargetLat = await Tables.GetColumnNamesLogic(3, 18).ConfigureAwait(false);
            Tables.RawMetadata.column20_TargetLon = await Tables.GetColumnNamesLogic(3, 19).ConfigureAwait(false);
            Tables.RawMetadata.column21_TargetElevation = await Tables.GetColumnNamesLogic(3, 20).ConfigureAwait(false);
            Tables.RawMetadata.column22_SlantRange_m = await Tables.GetColumnNamesLogic(3, 21).ConfigureAwait(false);
            #endregion
            #region VideoID
            Tables.VideoID.tableName = await Tables.GetTableNames(4).ConfigureAwait(false);
            Tables.VideoID.column1_UniqueVideoID = await Tables.GetColumnNamesLogic(4, 0).ConfigureAwait(false);
            Tables.VideoID.column2_PathToVideo = await Tables.GetColumnNamesLogic(4, 1).ConfigureAwait(false);
            #endregion
        }
        public class General
        {
            public static string? pathToDrive { get; set; } = ConfigurationManager.AppSettings["PathToDrive"];
            public static string connectionString { get; private set; } = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            public static string databaseName { get; private set; } = GetDatabaseName(connectionString);
            public static string GetDatabaseName(string connectionString)
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return builder.Database;
            }
        }
        public static class Tables
        {
            public static class CompressedMetadata
            {
                // Table name (optional)
                public static string TableName { get; set; } = "compressed_metadata";

                // Data column names
                public static string UniqueVideoID { get; set; } = "UniqueVideoID";
                public static string DistanceTraveled { get; set; } = "DistanceTraveled";

                // Minimum and maximum for UniqueVideoID
                public static string UniqueVideoID_min { get; set; } = "UniqueVideoID_min";
                public static string UniqueVideoID_max { get; set; } = "UniqueVideoID_max";

                // Frame index minimum and maximum
                public static string FrameIndex_min { get; set; } = "FrameIndex_min";
                public static string FrameIndex_max { get; set; } = "FrameIndex_max";

                // UTC time minimum and maximum
                public static string UtcTime_min { get; set; } = "UtcTime_min";
                public static string UtcTime_max { get; set; } = "UtcTime_max";

                // Platform heading, pitch, and roll (min and max)
                public static string PlatformHeading_min { get; set; } = "PlatformHeading_min";
                public static string PlatformHeading_max { get; set; } = "PlatformHeading_max";
                public static string PlatformPitch_min { get; set; } = "PlatformPitch_min";
                public static string PlatformPitch_max { get; set; } = "PlatformPitch_max";
                public static string PlatformRoll_min { get; set; } = "PlatformRoll_min";
                public static string PlatformRoll_max { get; set; } = "PlatformRoll_max";

                // Sensor name unique values
                public static string SensorName_unique { get; set; } = "SensorName_unique";

                // Sensor latitude and longitude (min and max)
                public static string SensorLatitude_min { get; set; } = "SensorLatitude_min";
                public static string SensorLatitude_max { get; set; } = "SensorLatitude_max";
                public static string SensorLongitude_min { get; set; } = "SensorLongitude_min";
                public static string SensorLongitude_max { get; set; } = "SensorLongitude_max";

                // Sensor altitude in meters (min and max)
                public static string SensorAltitude_m_min { get; set; } = "SensorAltitude_m_min";
                public static string SensorAltitude_m_max { get; set; } = "SensorAltitude_m_max";

                // Sensor relative roll, azimuth, and elevation (min and max)
                public static string SensorRelativeRoll_min { get; set; } = "SensorRelativeRoll_min";
                public static string SensorRelativeRoll_max { get; set; } = "SensorRelativeRoll_max";
                public static string SensorRelativeAzimuth_min { get; set; } = "SensorRelativeAzimuth_min";
                public static string SensorRelativeAzimuth_max { get; set; } = "SensorRelativeAzimuth_max";
                public static string SensorRelativeElevation_min { get; set; } = "SensorRelativeElevation_min";
                public static string SensorRelativeElevation_max { get; set; } = "SensorRelativeElevation_max";

                // Sensor horizontal and vertical FOV in degrees (min and max)
                public static string SensorHorizontalFovDegrees_min { get; set; } = "SensorHorizontalFovDegrees_min";
                public static string SensorHorizontalFovDegrees_max { get; set; } = "SensorHorizontalFovDegrees_max";
                public static string SensorVerticalFovDegrees_min { get; set; } = "SensorVerticalFovDegrees_min";
                public static string SensorVerticalFovDegrees_max { get; set; } = "SensorVerticalFovDegrees_max";

                // Frame elevation (min and max)
                public static string FrameElevation_min { get; set; } = "FrameElevation_min";
                public static string FrameElevation_max { get; set; } = "FrameElevation_max";

                // Frame latitude and longitude (min and max)
                public static string FrameLat_min { get; set; } = "FrameLat_min";
                public static string FrameLat_max { get; set; } = "FrameLat_max";
                public static string FrameLon_min { get; set; } = "FrameLon_min";
                public static string FrameLon_max { get; set; } = "FrameLon_max";

                // Target latitude and longitude (min and max)
                public static string TargetLat_min { get; set; } = "TargetLat_min";
                public static string TargetLat_max { get; set; } = "TargetLat_max";
                public static string TargetLon_min { get; set; } = "TargetLon_min";
                public static string TargetLon_max { get; set; } = "TargetLon_max";

                // Target elevation (min and max)
                public static string TargetElevation_min { get; set; } = "TargetElevation_min";
                public static string TargetElevation_max { get; set; } = "TargetElevation_max";

                // Slant range in meters (min and max)
                public static string SlantRange_m_min { get; set; } = "SlantRange_m_min";
                public static string SlantRange_m_max { get; set; } = "SlantRange_m_max";
            }
            public static class RawMetadata
            {
                public static string? tableName { get; set; }
                public static string? column1_UniqueVideoID { get; set; }
                public static string? column2_FrameIndex { get; set; }
                public static string? column3_UtcTime { get; set; }
                public static string? column4_PlatformHeading { get; set; }
                public static string? column5_PlatformPitch { get; set; }
                public static string? column6_PlatformRoll { get; set; }
                public static string? column7_SensorName { get; set; }
                public static string? column8_SensorLatitude { get; set; }
                public static string? column9_SensorLongitude { get; set; }
                public static string? column10_SensorAltitude_m { get; set; }
                public static string? column11_SensorRelativeRoll { get; set; }
                public static string? column12_SensorRelativeAzimuth { get; set; }
                public static string? column13_SensorRelativeElevation { get; set; }
                public static string? column14_SensorHorizontalFovDegrees { get; set; }
                public static string? column15_SensorVerticalFovDegrees { get; set; }
                public static string? column16_FrameElevation { get; set; }
                public static string? column17_FrameLat { get; set; }
                public static string? column18_FrameLon { get; set; }
                public static string? column19_TargetLat { get; set; }
                public static string? column20_TargetLon { get; set; }
                public static string? column21_TargetElevation { get; set; }
                public static string? column22_SlantRange_m { get; set; }
            }
            public static class VideoID
            {
                public static string? tableName { get; set; }
                public static string? column1_UniqueVideoID { get; set; }
                public static string? column2_PathToVideo { get; set; }
            }
            public static class Dataset_Links
            {
                public static string? tableName { get; set; }
                public static string UniqueFrameID { get; set; } = "UniqueFrameID";
                public static string UtcTime { get; set; } = "UtcTime";
                public static string PlatformHeading { get; set; } = "PlatformHeading";
                public static string PlatformPitch { get; set; } = "PlatformPitch";
                public static string PlatformRoll { get; set; } = "PlatformRoll";
                public static string SensorName { get; set; } = "SensorName";
                public static string SensorLatitude { get; set; } = "SensorLatitude";
                public static string SensorLongitude { get; set; } = "SensorLongitude";
                public static string SensorAltitude_m { get; set; } = "SensorAltitude_m";
                public static string SensorRelativeRoll { get; set; } = "SensorRelativeRoll";
                public static string SensorRelativeAzimuth { get; set; } = "SensorRelativeAzimuth";
                public static string SensorRelativeElevation { get; set; } = "SensorRelativeElevation";
                public static string SensorHorizontalFovDegrees { get; set; } = "SensorHorizontalFovDegrees";
                public static string SensorVerticalFovDegrees { get; set; } = "SensorVerticalFovDegrees";
                public static string FrameElevation { get; set; } = "FrameElevation";
                public static string FrameLat { get; set; } = "FrameLat";
                public static string FrameLon { get; set; } = "FrameLon";
                public static string TargetLat { get; set; } = "TargetLat";
                public static string TargetLon { get; set; } = "TargetLon";
                public static string TargetElevation { get; set; } = "TargetElevation";
                public static string SlantRange_m { get; set; } = "SlantRange_m";
                public static string ParentVidPath { get; set; } = "ParentVidPath";
            }
            public static class Datasets
            {
                public static string? tableName { get; set; }
                public static string? column1_UniqueDatasetID { get; set; } = "UniqueDatasetID";
                public static string? column2_DatasetName { get; set; } = "datasetName";
                public static string? column3_pathToDataset { get; set; } = "pathToDataset";
            }
            public static async Task<string> GetTableNames(int tableIndex)
            {
                try
                {
                    List<string> listOfTableNames = await MySQLDataAccess.QuerySQL<string, dynamic>(SQL_QueriesStore.GetNames.tables, new { SchemaName = General.databaseName }, General.connectionString).ConfigureAwait(false);
                    switch (tableIndex)
                    {
                        case 0:
                            return listOfTableNames[0];
                        case 1:
                            return listOfTableNames[1];
                        case 2:
                            return listOfTableNames[2];
                        case 3:
                            return listOfTableNames[3];
                        case 4:
                            return listOfTableNames[4];
                        default: throw new Exception("Invalid column name/index");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return null;
                }
            }
            public static async Task<string> GetColumnNamesLogic(int tableIndex, int columnIndex)
            {
                try
                {
                    switch (tableIndex)
                    {
                        case 0:
                        //return await GetColumnNames_Compressed_Metadata(columnIndex);
                        case 1:
                            return null;
                        case 2:
                            return null;
                        case 3:
                            return await GetColumnNames_Raw_Metadata(columnIndex);
                        case 4:
                            return await GetColumnNames_Video_ID(columnIndex);
                        default: throw new Exception("Invalid column name/index");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return null;
                }
            }
            /*private static async Task<string> GetColumnNames_Compressed_Metadata(int columnIndex)
            {
              // List<string> listOfColumnNames = await MySQLDataAccess.QuerySQL<string, dynamic>(SQL_QueriesStore.GetNames.columns, new { TableName = CompressedMetadata.tableName }, General.connectionString).ConfigureAwait(false);

               *//* switch (columnIndex)
                {
                    case 0:
                        return listOfColumnNames[0];
                    case 1:
                        return listOfColumnNames[1];
                    default:
                        throw new Exception("Invalid table name or index");
                }*//*
            }*/
            private static async Task<string> GetColumnNames_Raw_Metadata(int index)
            {
                List<string> listOfColumnNames = await MySQLDataAccess.QuerySQL<string, dynamic>(SQL_QueriesStore.GetNames.columns, new { TableName = RawMetadata.tableName }, General.connectionString).ConfigureAwait(false);

                switch (index)
                {
                    case 0:
                        return listOfColumnNames[0];
                    case 1:
                        return listOfColumnNames[1];
                    case 2:
                        return listOfColumnNames[2];
                    case 3:
                        return listOfColumnNames[3];
                    case 4:
                        return listOfColumnNames[4];
                    case 5:
                        return listOfColumnNames[5];
                    case 6:
                        return listOfColumnNames[6];
                    case 7:
                        return listOfColumnNames[7];
                    case 8:
                        return listOfColumnNames[8];
                    case 9:
                        return listOfColumnNames[9];
                    case 10:
                        return listOfColumnNames[10];
                    case 11:
                        return listOfColumnNames[11];
                    case 12:
                        return listOfColumnNames[12];
                    case 13:
                        return listOfColumnNames[13];
                    case 14:
                        return listOfColumnNames[14];
                    case 15:
                        return listOfColumnNames[15];
                    case 16:
                        return listOfColumnNames[16];
                    case 17:
                        return listOfColumnNames[17];
                    case 18:
                        return listOfColumnNames[18];
                    case 19:
                        return listOfColumnNames[19];
                    case 20:
                        return listOfColumnNames[20];
                    case 21:
                        return listOfColumnNames[21];
                    default:
                        throw new Exception("Invalid column name table name or index");
                }
            }
            private static async Task<string> GetColumnNames_Video_ID(int index)
            {
                List<string> listOfColumnNames = await MySQLDataAccess.QuerySQL<string, dynamic>(SQL_QueriesStore.GetNames.columns, new { TableName = VideoID.tableName }, General.connectionString).ConfigureAwait(false);
                switch (index)
                {
                    case 0:
                        return listOfColumnNames[0];
                    case 1:
                        return listOfColumnNames[1];
                    default:
                        throw new Exception("Invalid column name table name or index");
                }
            }
        }
    }
    public static class TableInstances
    {
        public class CompressedMetadata
        {
            // Assuming UniqueVideoID is always present.
            public ulong UniqueVideoID { get; set; }

            // A double to hold the traveled distance.
            //public double DistanceTraveled { get; set; }

            // UTC time minimum and maximum (nullable DateTime).
            public DateTime? UtcTime_min { get; set; }
            public DateTime? UtcTime_max { get; set; }

            // Platform heading, pitch, and roll (nullable doubles).
            public double? PlatformHeading_min { get; set; }
            public double? PlatformHeading_max { get; set; }

            public double? PlatformPitch_min { get; set; }
            public double? PlatformPitch_max { get; set; }

            public double? PlatformRoll_min { get; set; }
            public double? PlatformRoll_max { get; set; }

            // Sensor name unique values – stored as a string.
            public string SensorName_unique { get; set; }

            // Sensor latitude and longitude (nullable doubles).
            public double? SensorLatitude_min { get; set; }
            public double? SensorLatitude_max { get; set; }

            public double? SensorLongitude_min { get; set; }
            public double? SensorLongitude_max { get; set; }

            // Sensor altitude in meters (nullable doubles).
            public double? SensorAltitude_m_min { get; set; }
            public double? SensorAltitude_m_max { get; set; }

            // Sensor relative roll, azimuth, and elevation (nullable doubles).
            public double? SensorRelativeRoll_min { get; set; }
            public double? SensorRelativeRoll_max { get; set; }

            public double? SensorRelativeAzimuth_min { get; set; }
            public double? SensorRelativeAzimuth_max { get; set; }

            public double? SensorRelativeElevation_min { get; set; }
            public double? SensorRelativeElevation_max { get; set; }

            // Sensor horizontal and vertical field of view in degrees (nullable floats).
            public float? SensorHorizontalFovDegrees_min { get; set; }
            public float? SensorHorizontalFovDegrees_max { get; set; }

            public float? SensorVerticalFovDegrees_min { get; set; }
            public float? SensorVerticalFovDegrees_max { get; set; }

            // Frame elevation (nullable floats).
            public float? FrameElevation_min { get; set; }
            public float? FrameElevation_max { get; set; }

            // Frame latitude and longitude (nullable floats).
            public float? FrameLat_min { get; set; }
            public float? FrameLat_max { get; set; }

            public float? FrameLon_min { get; set; }
            public float? FrameLon_max { get; set; }

            // Target latitude and longitude (nullable floats).
            public float? TargetLat_min { get; set; }
            public float? TargetLat_max { get; set; }

            public float? TargetLon_min { get; set; }
            public float? TargetLon_max { get; set; }

            // Target elevation (nullable floats).
            public float? TargetElevation_min { get; set; }
            public float? TargetElevation_max { get; set; }

            // Slant range in meters (nullable floats).
            public float? SlantRange_m_min { get; set; }
            public float? SlantRange_m_max { get; set; }
        }
        public class RawMetadata
        {
            public ulong UniqueVideoID { get; set; }
            public int FrameIndex { get; set; }
            public DateTime UtcTime { get; set; }
            public double PlatformHeading { get; set; }
            public double PlatformPitch { get; set; }
            public double PlatformRoll { get; set; }
            public string SensorName { get; set; }
            public double SensorLatitude { get; set; }
            public double SensorLongitude { get; set; }
            public double SensorAltitude_m { get; set; }
            public double SensorRelativeRoll { get; set; }
            public double SensorRelativeAzimuth { get; set; }
            public double SensorRelativeElevation { get; set; }
            public double SensorHorizontalFovDegrees { get; set; }
            public double SensorVerticalFovDegrees { get; set; }
            public double FrameElevation { get; set; }
            public double FrameLat { get; set; }
            public double FrameLon { get; set; }
            public double TargetLat { get; set; }
            public double TargetLon { get; set; }
            public double TargetElevation { get; set; }
            public double SlantRange_m { get; set; }
        }
        public class VideoID
        {
            public ulong UniqueVideoID { get; set; }
            public string PathToVideo { get; set; }
        }
        public class datasets
        {
            public ulong UniqueVideoID { get; set; }
            public string DataSetName { get; set; }
            public string PathToDataset { get; set; }
        }
        public class dataset_links
        {
            public ulong UniqueFrameID { get; set; }
            public DateTime UtcTime { get; set; }
            public double PlatformHeading { get; set; }
            public double PlatformPitch { get; set; }
            public double PlatformRoll { get; set; }
            public string SensorName { get; set; }
            public double SensorLatitude { get; set; }
            public double SensorLongitude { get; set; }
            public double SensorAltitude_m { get; set; }
            public double SensorRelativeRoll { get; set; }
            public double SensorRelativeAzimuth { get; set; }
            public double SensorRelativeElevation { get; set; }
            public double SensorHorizontalFovDegrees { get; set; }
            public double SensorVerticalFovDegrees { get; set; }
            public double FrameElevation { get; set; }
            public double FrameLat { get; set; }
            public double FrameLon { get; set; }
            public double TargetLat { get; set; }
            public double TargetLon { get; set; }
            public double TargetElevation { get; set; }
            public double SlantRange_m { get; set; }
            public string ParentVidPath { get; set; }
        }
    }
    public static class SQL_QueriesStore
    {
        public static class VideoID
        {
            public static string insertPathToVideo = $"UPDATE {NameLibrary.Tables.VideoID.tableName} SET {NameLibrary.Tables.VideoID.column2_PathToVideo} = @{NameLibrary.Tables.VideoID.column2_PathToVideo} WHERE {NameLibrary.Tables.VideoID.column1_UniqueVideoID} = @{NameLibrary.Tables.VideoID.column1_UniqueVideoID}";
            public static string selectFrom = $"SELECT {NameLibrary.Tables.VideoID.column1_UniqueVideoID}, {NameLibrary.Tables.VideoID.column2_PathToVideo} FROM {NameLibrary.Tables.VideoID.tableName}";
            public static string addTo = $"INSERT INTO {NameLibrary.Tables.VideoID.tableName} ({NameLibrary.Tables.VideoID.column1_UniqueVideoID}, {NameLibrary.Tables.VideoID.column2_PathToVideo}) VALUES (@{NameLibrary.Tables.VideoID.column1_UniqueVideoID}, @{NameLibrary.Tables.VideoID.column2_PathToVideo})";
            public static string deleteFrom = $"DELETE FROM {NameLibrary.Tables.VideoID.tableName} WHERE {NameLibrary.Tables.VideoID.column1_UniqueVideoID} = @{NameLibrary.Tables.VideoID.column1_UniqueVideoID}";
        }
        public static class CompressedMetadata
        {
            public static string InsertCompressedMetadataQuery = @"
                INSERT INTO metadatabase.compressed_metadata
                (
                    UniqueVideoID,
                    FrameIndex_min,
                    FrameIndex_max,
                    UtcTime_min,
                    UtcTime_max,
                    PlatformHeading_min,
                    PlatformHeading_max,
                    PlatformPitch_min,
                    PlatformPitch_max,
                    PlatformRoll_min,
                    PlatformRoll_max,
                    SensorName_unique,
                    SensorLatitude_min,
                    SensorLatitude_max,
                    SensorLongitude_min,
                    SensorLongitude_max,
                    SensorAltitude_m_min,
                    SensorAltitude_m_max,
                    SensorRelativeRoll_min,
                    SensorRelativeRoll_max,
                    SensorRelativeAzimuth_min,
                    SensorRelativeAzimuth_max,
                    SensorRelativeElevation_min,
                    SensorRelativeElevation_max,
                    SensorHorizontalFovDegrees_min,
                    SensorHorizontalFovDegrees_max,
                    SensorVerticalFovDegrees_min,
                    SensorVerticalFovDegrees_max,
                    FrameElevation_min,
                    FrameElevation_max,
                    FrameLat_min,
                    FrameLat_max,
                    FrameLon_min,
                    FrameLon_max,
                    TargetLat_min,
                    TargetLat_max,
                    TargetLon_min,
                    TargetLon_max,
                    TargetElevation_min,
                    TargetElevation_max,
                    SlantRange_m_min,
                    SlantRange_m_max
                )
                VALUES
                (
                    @UniqueVideoID,
                    @FrameIndex_min,
                    @FrameIndex_max,
                    @UtcTime_min,
                    @UtcTime_max,
                    @PlatformHeading_min,
                    @PlatformHeading_max,
                    @PlatformPitch_min,
                    @PlatformPitch_max,
                    @PlatformRoll_min,
                    @PlatformRoll_max,
                    @SensorName_unique,
                    @SensorLatitude_min,
                    @SensorLatitude_max,
                    @SensorLongitude_min,
                    @SensorLongitude_max,
                    @SensorAltitude_m_min,
                    @SensorAltitude_m_max,
                    @SensorRelativeRoll_min,
                    @SensorRelativeRoll_max,
                    @SensorRelativeAzimuth_min,
                    @SensorRelativeAzimuth_max,
                    @SensorRelativeElevation_min,
                    @SensorRelativeElevation_max,
                    @SensorHorizontalFovDegrees_min,
                    @SensorHorizontalFovDegrees_max,
                    @SensorVerticalFovDegrees_min,
                    @SensorVerticalFovDegrees_max,
                    @FrameElevation_min,
                    @FrameElevation_max,
                    @FrameLat_min,
                    @FrameLat_max,
                    @FrameLon_min,
                    @FrameLon_max,
                    @TargetLat_min,
                    @TargetLat_max,
                    @TargetLon_min,
                    @TargetLon_max,
                    @TargetElevation_min,
                    @TargetElevation_max,
                    @SlantRange_m_min,
                    @SlantRange_m_max
                    );
                    ";
            public static string calc = @"
                SELECT 
                UniqueVideoID,
                MIN(FrameIndex) AS FrameIndex_min,
                MAX(FrameIndex) AS FrameIndex_max,
                MIN(UtcTime) AS UtcTime_min,
                MAX(UtcTime) AS UtcTime_max,
                MIN(PlatformHeading) AS PlatformHeading_min,
                MAX(PlatformHeading) AS PlatformHeading_max,
                MIN(PlatformPitch) AS PlatformPitch_min,
                MAX(PlatformPitch) AS PlatformPitch_max,
                MIN(PlatformRoll) AS PlatformRoll_min,
                MAX(PlatformRoll) AS PlatformRoll_max,
                -- Optimize GROUP_CONCAT by limiting length
                GROUP_CONCAT(DISTINCT TRIM(SensorName) ORDER BY SensorName SEPARATOR ', ') AS SensorName_unique,
                MIN(SensorLatitude) AS SensorLatitude_min,
                MAX(SensorLatitude) AS SensorLatitude_max,
                MIN(SensorLongitude) AS SensorLongitude_min,
                MAX(SensorLongitude) AS SensorLongitude_max,
                MIN(SensorAltitude_m) AS SensorAltitude_m_min,
                MAX(SensorAltitude_m) AS SensorAltitude_m_max,
                MIN(SensorRelativeRoll) AS SensorRelativeRoll_min,
                MAX(SensorRelativeRoll) AS SensorRelativeRoll_max,
                MIN(SensorRelativeAzimuth) AS SensorRelativeAzimuth_min,
                MAX(SensorRelativeAzimuth) AS SensorRelativeAzimuth_max,
                MIN(SensorRelativeElevation) AS SensorRelativeElevation_min,
                MAX(SensorRelativeElevation) AS SensorRelativeElevation_max,
                MIN(SensorHorizontalFovDegrees) AS SensorHorizontalFovDegrees_min,
                MAX(SensorHorizontalFovDegrees) AS SensorHorizontalFovDegrees_max,
                MIN(SensorVerticalFovDegrees) AS SensorVerticalFovDegrees_min,
                MAX(SensorVerticalFovDegrees) AS SensorVerticalFovDegrees_max,
                MIN(FrameElevation) AS FrameElevation_min,
                MAX(FrameElevation) AS FrameElevation_max,
                MIN(FrameLat) AS FrameLat_min,
                MAX(FrameLat) AS FrameLat_max,
                MIN(FrameLon) AS FrameLon_min,
                MAX(FrameLon) AS FrameLon_max,
                MIN(TargetLat) AS TargetLat_min,
                MAX(TargetLat) AS TargetLat_max,
                MIN(TargetLon) AS TargetLon_min,
                MAX(TargetLon) AS TargetLon_max,
                MIN(TargetElevation) AS TargetElevation_min,
                MAX(TargetElevation) AS TargetElevation_max,
                MIN(SlantRange_m) AS SlantRange_m_min,
                MAX(SlantRange_m) AS SlantRange_m_max
                FROM metadatabase.raw_metadata
                GROUP BY UniqueVideoID
                ORDER BY UniqueVideoID DESC  -- Ensures latest video IDs are processed first
                ";
        }
        public static class GetNames
        {
            public static string tables = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @SchemaName";
            public static string columns = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME =  @TableName ORDER BY ORDINAL_POSITION;";
        }
        public static class RawMetadata
        {
            public static string addTo = $"INSERT INTO {NameLibrary.Tables.RawMetadata.tableName} " +
             $"({NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}, " +
             $"{NameLibrary.Tables.RawMetadata.column2_FrameIndex}, " +
             $"{NameLibrary.Tables.RawMetadata.column3_UtcTime}, " +
             $"{NameLibrary.Tables.RawMetadata.column4_PlatformHeading}, " +
             $"{NameLibrary.Tables.RawMetadata.column5_PlatformPitch}, " +
             $"{NameLibrary.Tables.RawMetadata.column6_PlatformRoll}, " +
             $"{NameLibrary.Tables.RawMetadata.column7_SensorName}, " +
             $"{NameLibrary.Tables.RawMetadata.column8_SensorLatitude}, " +
             $"{NameLibrary.Tables.RawMetadata.column9_SensorLongitude}, " +
             $"{NameLibrary.Tables.RawMetadata.column10_SensorAltitude_m}, " +
             $"{NameLibrary.Tables.RawMetadata.column11_SensorRelativeRoll}, " +
             $"{NameLibrary.Tables.RawMetadata.column12_SensorRelativeAzimuth}, " +
             $"{NameLibrary.Tables.RawMetadata.column13_SensorRelativeElevation}, " +
             $"{NameLibrary.Tables.RawMetadata.column14_SensorHorizontalFovDegrees}, " +
             $"{NameLibrary.Tables.RawMetadata.column15_SensorVerticalFovDegrees}, " +
             $"{NameLibrary.Tables.RawMetadata.column16_FrameElevation}, " +
             $"{NameLibrary.Tables.RawMetadata.column17_FrameLat}, " +
             $"{NameLibrary.Tables.RawMetadata.column18_FrameLon}, " +
             $"{NameLibrary.Tables.RawMetadata.column19_TargetLat}, " +
             $"{NameLibrary.Tables.RawMetadata.column20_TargetLon}, " +
             $"{NameLibrary.Tables.RawMetadata.column21_TargetElevation}, " +
             $"{NameLibrary.Tables.RawMetadata.column22_SlantRange_m}) " +
             $"VALUES (@{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}, " +
             $"@{NameLibrary.Tables.RawMetadata.column2_FrameIndex}, " +
             $"@{NameLibrary.Tables.RawMetadata.column3_UtcTime}, " +
             $"@{NameLibrary.Tables.RawMetadata.column4_PlatformHeading}, " +
             $"@{NameLibrary.Tables.RawMetadata.column5_PlatformPitch}, " +
             $"@{NameLibrary.Tables.RawMetadata.column6_PlatformRoll}, " +
             $"@{NameLibrary.Tables.RawMetadata.column7_SensorName}, " +
             $"@{NameLibrary.Tables.RawMetadata.column8_SensorLatitude}, " +
             $"@{NameLibrary.Tables.RawMetadata.column9_SensorLongitude}, " +
             $"@{NameLibrary.Tables.RawMetadata.column10_SensorAltitude_m}, " +
             $"@{NameLibrary.Tables.RawMetadata.column11_SensorRelativeRoll}, " +
             $"@{NameLibrary.Tables.RawMetadata.column12_SensorRelativeAzimuth}, " +
             $"@{NameLibrary.Tables.RawMetadata.column13_SensorRelativeElevation}, " +
             $"@{NameLibrary.Tables.RawMetadata.column14_SensorHorizontalFovDegrees}, " +
             $"@{NameLibrary.Tables.RawMetadata.column15_SensorVerticalFovDegrees}, " +
             $"@{NameLibrary.Tables.RawMetadata.column16_FrameElevation}, " +
             $"@{NameLibrary.Tables.RawMetadata.column17_FrameLat}, " +
             $"@{NameLibrary.Tables.RawMetadata.column18_FrameLon}, " +
             $"@{NameLibrary.Tables.RawMetadata.column19_TargetLat}, " +
             $"@{NameLibrary.Tables.RawMetadata.column20_TargetLon}, " +
             $"@{NameLibrary.Tables.RawMetadata.column21_TargetElevation}, " +
             $"@{NameLibrary.Tables.RawMetadata.column22_SlantRange_m})";
            public static string deleteFrom = $"DELETE FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getRowCount = $"SELECT COUNT(*) AS RowCount FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.VideoID.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string insertCSV = $@"
                LOAD DATA LOCAL INFILE filepath
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
                    UniqueVideoID = 'VidID';
                ";

        }
        public static class Datasets
        {
            public static string addTo = $"INSERT INTO {NameLibrary.Tables.Datasets.tableName} ({NameLibrary.Tables.Datasets.column1_UniqueDatasetID}, {NameLibrary.Tables.Datasets.column2_DatasetName}, {NameLibrary.Tables.Datasets.column3_pathToDataset}) VALUES (@{NameLibrary.Tables.Datasets.column1_UniqueDatasetID}, {NameLibrary.Tables.Datasets.column2_DatasetName}, {NameLibrary.Tables.Datasets.column3_pathToDataset})";
        }
        public static class Dataset_Links
        {
            public static string addTo = $"INSERT INTO {NameLibrary.Tables.Dataset_Links.tableName} " +
        $"(" +
            $"{NameLibrary.Tables.Dataset_Links.UniqueFrameID}, " +
            $"{NameLibrary.Tables.Dataset_Links.UtcTime}, " +
            $"{NameLibrary.Tables.Dataset_Links.PlatformHeading}, " +
            $"{NameLibrary.Tables.Dataset_Links.PlatformPitch}, " +
            $"{NameLibrary.Tables.Dataset_Links.PlatformRoll}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorName}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorLatitude}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorLongitude}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorAltitude_m}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorRelativeRoll}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorRelativeAzimuth}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorRelativeElevation}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorHorizontalFovDegrees}, " +
            $"{NameLibrary.Tables.Dataset_Links.SensorVerticalFovDegrees}, " +
            $"{NameLibrary.Tables.Dataset_Links.FrameElevation}, " +
            $"{NameLibrary.Tables.Dataset_Links.FrameLat}, " +
            $"{NameLibrary.Tables.Dataset_Links.FrameLon}, " +
            $"{NameLibrary.Tables.Dataset_Links.TargetLat}, " +
            $"{NameLibrary.Tables.Dataset_Links.TargetLon}, " +
            $"{NameLibrary.Tables.Dataset_Links.TargetElevation}, " +
            $"{NameLibrary.Tables.Dataset_Links.SlantRange_m}" +
            $"{NameLibrary.Tables.Dataset_Links.ParentVidPath}" +

        $") " +
        $"VALUES " +
        $"(" +
            $"@{NameLibrary.Tables.Dataset_Links.UniqueFrameID}, " +
            $"@{NameLibrary.Tables.Dataset_Links.UtcTime}, " +
            $"@{NameLibrary.Tables.Dataset_Links.PlatformHeading}, " +
            $"@{NameLibrary.Tables.Dataset_Links.PlatformPitch}, " +
            $"@{NameLibrary.Tables.Dataset_Links.PlatformRoll}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorName}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorLatitude}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorLongitude}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorAltitude_m}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorRelativeRoll}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorRelativeAzimuth}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorRelativeElevation}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorHorizontalFovDegrees}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SensorVerticalFovDegrees}, " +
            $"@{NameLibrary.Tables.Dataset_Links.FrameElevation}, " +
            $"@{NameLibrary.Tables.Dataset_Links.FrameLat}, " +
            $"@{NameLibrary.Tables.Dataset_Links.FrameLon}, " +
            $"@{NameLibrary.Tables.Dataset_Links.TargetLat}, " +
            $"@{NameLibrary.Tables.Dataset_Links.TargetLon}, " +
            $"@{NameLibrary.Tables.Dataset_Links.TargetElevation}, " +
            $"@{NameLibrary.Tables.Dataset_Links.SlantRange_m}" +
            $"{NameLibrary.Tables.Dataset_Links.ParentVidPath}" +
        $")";
        }
        public static class Filters
        {
            public static string findPathToVideoFromID = $"SELECT DISTINCT {NameLibrary.Tables.VideoID.column2_PathToVideo} FROM {NameLibrary.Tables.VideoID.tableName} WHERE {NameLibrary.Tables.VideoID.column1_UniqueVideoID} = @{NameLibrary.Tables.VideoID.column1_UniqueVideoID}";

            // Base query for the CompressedMetadata table.
            public static string SensorNameBaseQuery = $@"
                SELECT UniqueVideoID
                    FROM (
                        SELECT 
                            {NameLibrary.Tables.CompressedMetadata.UniqueVideoID},
                            GROUP_CONCAT(
                                DISTINCT TRIM(SensorName_unique)
                                ORDER BY UniqueVideoID
                                SEPARATOR ', '
                            ) AS SensorName_unique
                        FROM compressed_metadata";

            public static string NoSensorNameBaseQuery = $@"
                SELECT {NameLibrary.Tables.CompressedMetadata.UniqueVideoID} FROM compressed_metadata WHERE ";


            public static string endFilterSensorName_Aggregated = $@" WHERE SensorName_unique LIKE CONCAT('%', @SensorName, '%')";

            public static string intersect = "\n INTERSECT \n";
            public static string where = " WHERE ";

            public static string close = " GROUP BY UniqueVideoID" +
                ") AS aggregated_data";


            // Duration filter (not implemented)
            public static string filterDuration = "No implement";


            #region Platform
            // Filter for Platform Heading (using aggregated min and max values)
            public static string filterPlatformHeading =
                $" {NameLibrary.Tables.CompressedMetadata.PlatformHeading_min} > @MinPlatformHeading AND {NameLibrary.Tables.CompressedMetadata.PlatformHeading_max} < @MaxPlatformHeading";

            // Filter for Platform Pitch
            public static string filterPlatformPitch =
                $" {NameLibrary.Tables.CompressedMetadata.PlatformPitch_min} > @MinPlatformPitch AND {NameLibrary.Tables.CompressedMetadata.PlatformPitch_max} < @MaxPlatformPitch";

            // Filter for Platform Roll
            public static string filterPlatformRoll =
                $" {NameLibrary.Tables.CompressedMetadata.PlatformRoll_min} > @MinPlatformRoll AND {NameLibrary.Tables.CompressedMetadata.PlatformRoll_max} < @MaxPlatformRoll";
            #endregion

            public static string findVideoIDFromPathToVideo =
                $"SELECT {NameLibrary.Tables.VideoID.column1_UniqueVideoID} FROM {NameLibrary.Tables.VideoID.tableName} " +
                         $"WHERE {NameLibrary.Tables.VideoID.column2_PathToVideo} LIKE CONCAT('%', @DirectoryPath, '%')";

            public static string filterUtcTime =
              $" {NameLibrary.Tables.CompressedMetadata.UtcTime_min} <= @MinDate AND {NameLibrary.Tables.CompressedMetadata.UtcTime_max} >= @MaxDate";

            // Filter for Sensor Latitude
            public static string filterSensorLatitude =
                $" {NameLibrary.Tables.CompressedMetadata.SensorLatitude_min} <= @MinSensorLatitude AND {NameLibrary.Tables.CompressedMetadata.SensorLatitude_max} >= @MaxSensorLatitude";

            // Filter for Sensor Longitude
            public static string filterSensorLongitude =
                $" {NameLibrary.Tables.CompressedMetadata.SensorLongitude_min} <= @MinSensorLongitude AND {NameLibrary.Tables.CompressedMetadata.SensorLongitude_max} >= @MaxSensorLongitude";

            // Filter for Sensor Altitude
            public static string filterSensorAltitude =
                $" {NameLibrary.Tables.CompressedMetadata.SensorAltitude_m_min} <= @MinSensorAltitude AND {NameLibrary.Tables.CompressedMetadata.SensorAltitude_m_max} >= @MaxSensorAltitude";

            // Filter for Sensor Relative Roll
            public static string filterSensorRelativeRoll =
                $" {NameLibrary.Tables.CompressedMetadata.SensorRelativeRoll_min} <= @MinSensorRelativeRoll AND {NameLibrary.Tables.CompressedMetadata.SensorRelativeRoll_max} >= @MaxSensorRelativeRoll";

            // Filter for Sensor Relative Azimuth
            public static string filterSensorRelativeAzimuth =
                $" {NameLibrary.Tables.CompressedMetadata.SensorRelativeAzimuth_min} <= @MinSensorRelativeAzimuth AND {NameLibrary.Tables.CompressedMetadata.SensorRelativeAzimuth_max} >= @MaxSensorRelativeAzimuth";

            // Filter for Sensor Relative Elevation
            public static string filterSensorRelativeElevation =
                $" {NameLibrary.Tables.CompressedMetadata.SensorRelativeElevation_min} <= @MinSensorRelativeElevation AND {NameLibrary.Tables.CompressedMetadata.SensorRelativeElevation_max} >= @MaxSensorRelativeElevation";

            // Filter for Sensor Horizontal FOV Degrees
            public static string filterSensorHorizontalFovDegrees =
                $" {NameLibrary.Tables.CompressedMetadata.SensorHorizontalFovDegrees_min} <= @MinSensorHorizontalFovDegrees AND {NameLibrary.Tables.CompressedMetadata.SensorHorizontalFovDegrees_max} >= @MaxSensorHorizontalFovDegrees";

            #region skipVertFov

            // Filter for Sensor Vertical FOV Degrees
            public static string filterSensorVerticalFovDegrees =
                $" {NameLibrary.Tables.CompressedMetadata.SensorVerticalFovDegrees_min} > @MinSensorVerticalFovDegrees AND {NameLibrary.Tables.CompressedMetadata.SensorVerticalFovDegrees_max} < @MaxSensorVerticalFovDegrees";
            #endregion

            // Filter for Frame Elevation
            public static string filterFrameElevation =
                $" {NameLibrary.Tables.CompressedMetadata.FrameElevation_min} <= @MinFrameElevation AND {NameLibrary.Tables.CompressedMetadata.FrameElevation_max} >= @MaxFrameElevation";

            // Filter for Frame Latitude
            public static string filterFrameLat =
                $" {NameLibrary.Tables.CompressedMetadata.FrameLat_min} <= @MinFrameLat AND {NameLibrary.Tables.CompressedMetadata.FrameLat_max} >= @MaxFrameLat";

            // Filter for Frame Longitude
            public static string filterFrameLon =
                $" {NameLibrary.Tables.CompressedMetadata.FrameLon_min} <= @MinFrameLon AND {NameLibrary.Tables.CompressedMetadata.FrameLon_max} >= @MaxFrameLon";

            // Filter for Target Latitude
            public static string filterTargetLat =
                $" {NameLibrary.Tables.CompressedMetadata.TargetLat_min} <= @MinTargetLat AND {NameLibrary.Tables.CompressedMetadata.TargetLat_max} >= @MaxTargetLat";

            // Filter for Target Longitude
            public static string filterTargetLon =
                $" {NameLibrary.Tables.CompressedMetadata.TargetLon_min} <= @MinTargetLon AND {NameLibrary.Tables.CompressedMetadata.TargetLon_max} >= @MaxTargetLon";

            // Filter for Target Elevation
            public static string filterTargetElevation =
                $" {NameLibrary.Tables.CompressedMetadata.TargetElevation_min} <= @MinTargetElevation AND {NameLibrary.Tables.CompressedMetadata.TargetElevation_max} >= @MaxTargetElevation";

            // Filter for Slant Range
            public static string filterSlantRange =
                $" {NameLibrary.Tables.CompressedMetadata.SlantRange_m_min} <= @MinSlantRange AND {NameLibrary.Tables.CompressedMetadata.SlantRange_m_max} >= @MaxSlantRange";

            // Query to find videos with more than X frames having SlantRange_m in a specific range
            public static class InDepthSearchQueries
            {
                //public static string GetSlantRangeQuery(int framesLimit) { }

                public static string filterFrameCountBaseQuery = $@"
                SELECT 
                    UniqueVideoID,
                    COUNT(*) AS FrameCount
                    FROM 
                    metadatabase.raw_metadata
                    WHERE 
                    UniqueVideoID IN (@VideoIDList)";

                public static string endQuery = @"GROUP BY 
                    UniqueVideoID
                    HAVING
                    COUNT(*) > 1000
                    ORDER BY
                    FrameCount DESC";

                public static string filterSlantRange = "AND SlantRange_m BETWEEN @MinSlantRange AND @MaxSlantRange";

                public static string filterSlantRangeFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSlantRange +
                    " " + endQuery;

                public static string filterUtcTime = "AND UtcTime BETWEEN @MinUtcTime AND @MaxUtcTime";
                public static string filterUtcTimeFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterUtcTime +
                    " " + endQuery;

                public static string filterPlatformHeading = "AND PlatformHeading BETWEEN @MinPlatformHeading AND @MaxPlatformHeading";
                public static string filterPlatformHeadingFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterPlatformHeading +
                    " " + endQuery;

                public static string filterPlatformPitch = "AND PlatformPitch BETWEEN @MinPlatformPitch AND @MaxPlatformPitch";
                public static string filterPlatformPitchFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterPlatformPitch +
                    " " + endQuery;

                public static string filterPlatformRoll = "AND PlatformRoll BETWEEN @MinPlatformRoll AND @MaxPlatformRoll";
                public static string filterPlatformRollFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterPlatformRoll +
                    " " + endQuery;

                public static string filterSensorLatitude = "AND SensorLatitude BETWEEN @MinSensorLatitude AND @MaxSensorLatitude";
                public static string filterSensorLatitudeFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorLatitude +
                    " " + endQuery;

                public static string filterSensorLongitude = "AND SensorLongitude BETWEEN @MinSensorLongitude AND @MaxSensorLongitude";
                public static string filterSensorLongitudeFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorLongitude +
                    " " + endQuery;

                public static string filterSensorAltitude = "AND SensorAltitude_m BETWEEN @MinSensorAltitude AND @MaxSensorAltitude";
                public static string filterSensorAltitudeFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorAltitude +
                    " " + endQuery;

                public static string filterSensorRelativeRoll = "AND SensorRelativeRoll BETWEEN @MinSensorRelativeRoll AND @MaxSensorRelativeRoll";
                public static string filterSensorRelativeRollFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorRelativeRoll +
                    " " + endQuery;

                public static string filterSensorRelativeAzimuth = "AND SensorRelativeAzimuth BETWEEN @MinSensorRelativeAzimuth AND @MaxSensorRelativeAzimuth";
                public static string filterSensorRelativeAzimuthFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorRelativeAzimuth +
                    " " + endQuery;

                public static string filterSensorRelativeElevation = "AND SensorRelativeElevation BETWEEN @MinSensorRelativeElevation AND @MaxSensorRelativeElevation";
                public static string filterSensorRelativeElevationFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorRelativeElevation +
                    " " + endQuery;

                public static string filterSensorHorizontalFovDegrees = "AND SensorHorizontalFovDegrees BETWEEN @MinSensorHorizontalFovDegrees AND @MaxSensorHorizontalFovDegrees";
                public static string filterSensorHorizontalFovDegreesFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorHorizontalFovDegrees +
                    " " + endQuery;

                public static string filterSensorVerticalFovDegrees = "AND SensorVerticalFovDegrees BETWEEN @MinSensorVerticalFovDegrees AND @MaxSensorVerticalFovDegrees";
                public static string filterSensorVerticalFovDegreesFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorVerticalFovDegrees +
                    " " + endQuery;

                public static string filterFrameElevation = "AND FrameElevation BETWEEN @MinFrameElevation AND @MaxFrameElevation";
                public static string filterFrameElevationFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterFrameElevation +
                    " " + endQuery;

                public static string filterFrameLat = "AND FrameLat BETWEEN @MinFrameLat AND @MaxFrameLat";
                public static string filterFrameLatFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterFrameLat +
                    " " + endQuery;

                public static string filterFrameLon = "AND FrameLon BETWEEN @MinFrameLon AND @MaxFrameLon";
                public static string filterFrameLonFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterFrameLon +
                    " " + endQuery;

                public static string filterTargetLat = "AND TargetLat BETWEEN @MinTargetLat AND @MaxTargetLat";
                public static string filterTargetLatFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterTargetLat +
                    " " + endQuery;

                public static string filterTargetLon = "AND TargetLon BETWEEN @MinTargetLon AND @MaxTargetLon";
                public static string filterTargetLonFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterTargetLon +
                    " " + endQuery;

                public static string filterTargetElevation = "AND TargetElevation BETWEEN @MinTargetElevation AND @MaxTargetElevation";
                public static string filterTargetElevationFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterTargetElevation +
                    " " + endQuery;

                public static string filterSensorName = "AND SensorName = @SensorName";
                public static string filterSensorNameFrameCount =
                    filterFrameCountBaseQuery +
                    " " + filterSensorName +
                    " " + endQuery;
            }

        }
        public static class GraphQueries
        {
            public static string getUtcTime = $"SELECT {NameLibrary.Tables.RawMetadata.column3_UtcTime} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getPlatformHeading = $"SELECT {NameLibrary.Tables.RawMetadata.column4_PlatformHeading} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getPlatformPitch = $"SELECT {NameLibrary.Tables.RawMetadata.column5_PlatformPitch} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getPlatformRoll = $"SELECT {NameLibrary.Tables.RawMetadata.column6_PlatformRoll} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorName = $"SELECT {NameLibrary.Tables.RawMetadata.column7_SensorName} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorLatitude = $"SELECT {NameLibrary.Tables.RawMetadata.column8_SensorLatitude} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorLongitude = $"SELECT {NameLibrary.Tables.RawMetadata.column9_SensorLongitude} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorAltitude_m = $"SELECT {NameLibrary.Tables.RawMetadata.column10_SensorAltitude_m} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorRelativeRoll = $"SELECT {NameLibrary.Tables.RawMetadata.column11_SensorRelativeRoll} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorRelativeAzimuth = $"SELECT {NameLibrary.Tables.RawMetadata.column12_SensorRelativeAzimuth} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorRelativeElevation = $"SELECT {NameLibrary.Tables.RawMetadata.column13_SensorRelativeElevation} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorHorizontalFovDegrees = $"SELECT {NameLibrary.Tables.RawMetadata.column14_SensorHorizontalFovDegrees} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSensorVerticalFovDegrees = $"SELECT {NameLibrary.Tables.RawMetadata.column15_SensorVerticalFovDegrees} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getFrameElevation = $"SELECT {NameLibrary.Tables.RawMetadata.column16_FrameElevation} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getFrameLat = $"SELECT {NameLibrary.Tables.RawMetadata.column17_FrameLat} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getFrameLon = $"SELECT {NameLibrary.Tables.RawMetadata.column18_FrameLon} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getTargetLat = $"SELECT {NameLibrary.Tables.RawMetadata.column19_TargetLat} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getTargetLon = $"SELECT {NameLibrary.Tables.RawMetadata.column20_TargetLon} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getTargetElevation = $"SELECT {NameLibrary.Tables.RawMetadata.column21_TargetElevation} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
            public static string getSlantRange_m = $"SELECT {NameLibrary.Tables.RawMetadata.column22_SlantRange_m} FROM {NameLibrary.Tables.RawMetadata.tableName} WHERE {NameLibrary.Tables.RawMetadata.column1_UniqueVideoID} = @{NameLibrary.Tables.RawMetadata.column1_UniqueVideoID}";
        }
    }
}
