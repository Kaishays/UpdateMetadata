using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.CompressKlvMetadata
{
    public static class FlightDistanceCalc
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
}
