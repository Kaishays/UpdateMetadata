using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.CompressKlvMetadata
{
    public static class MaxMinKlvCalc
    {
        public class CompressedMetadataTemp
        {
            // Unique identifier for the video.
            public ulong UniqueVideoID { get; set; }

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

        public static async Task<bool> ProcessSingleVideo(TableInstances.VideoID videoId)
        {
            var metadata = await CalcMaxMinForSingleVideo(videoId);
            if (metadata == null)
                return false;

            await InsertMetadata(metadata);
            return true;
        }

        private static async Task<CompressedMetadataTemp> CalcMaxMinForSingleVideo(TableInstances.VideoID videoId)
        {
            var results = await MySQLDataAccess.QuerySQL<CompressedMetadataTemp, TableInstances.VideoID>(
                SQL_QueriesStore.CompressedMetadata.calcForOneVidID,
                videoId,
                NameLibrary.General.connectionString);
            
            return results.FirstOrDefault();
        }

        private static async Task InsertMetadata(CompressedMetadataTemp metadata)
        {
            await MySQLDataAccess.ExecuteSQL<CompressedMetadataTemp>(
                SQL_QueriesStore.CompressedMetadata.InsertCompressedMetadataQuery,
                metadata,
                NameLibrary.General.connectionString);
        }

    }
}
