using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.Y_DriveReader
{
    public static class CsvToRawMetadata
    {
        public static List<TableInstances.RawMetadata> ConvertCsvToRawMetadata(List<string[]> allFramesInCSV, TableInstances.VideoID videoID_FromDatabase)
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
