using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpdateMetadata.RunLogHashing
{
    public static class HashLogDatabaseService
    {
        private const double WriteTimeToleranceInSeconds = 1.0;

        public static async Task<List<DateTime>> GetAllExistingWriteTimesFromDatabase(TableInstances.VideoID videoId)
        {
            try
            {
                return await MySQLDataAccess.QuerySQL<DateTime, dynamic>(
                    SQL_QueriesStore.ZExtractionLogs.getAllWriteTimes,
                    new { UniqueVideoID = videoId.UniqueVideoID },
                    NameLibrary.General.connectionString
                );
            }
            catch
            {
                return new List<DateTime>();
            }
        }

        public static async Task<bool> SaveHashLogEntryToDatabase(ulong uniqueVideoId, string csvFileHash, 
            uint runIndex, DateTime writeTime)
        {
            try
            {
                await MySQLDataAccess.ExecuteSQL(
                    SQL_QueriesStore.ZExtractionLogs.addTo,
                    new
                    {
                        UniqueVideoID = uniqueVideoId,
                        CsvHash = csvFileHash,
                        Run_Index = runIndex,
                        WriteTime = writeTime
                    },
                    NameLibrary.General.connectionString
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<List<uint>> GetAllExistingRunIndicesFromDatabase(ulong uniqueVideoId)
        {
            try
            {
                string getRunIndicesQuery = $"SELECT {NameLibrary.Tables.ZExtractionLogs.RunIndex} FROM {NameLibrary.Tables.ZExtractionLogs.tableName} WHERE {NameLibrary.Tables.ZExtractionLogs.UniqueVideoID} = @{NameLibrary.Tables.ZExtractionLogs.UniqueVideoID}";
                
                return await MySQLDataAccess.QuerySQL<uint, dynamic>(
                    getRunIndicesQuery,
                    new { UniqueVideoID = uniqueVideoId },
                    NameLibrary.General.connectionString
                );
            }
            catch
            {
                return new List<uint>();
            }
        }

        public static bool IsWriteTimeAlreadyInDatabase(DateTime csvLastWriteTime, List<DateTime> existingWriteTimes)
        {
            return existingWriteTimes.Any(databaseWriteTime => 
                AreWriteTimesApproximatelyEqual(csvLastWriteTime, databaseWriteTime));
        }

        private static bool AreWriteTimesApproximatelyEqual(DateTime firstTime, DateTime secondTime)
        {
            double timeDifferenceInSeconds = Math.Abs((firstTime - secondTime).TotalSeconds);
            return timeDifferenceInSeconds < WriteTimeToleranceInSeconds;
        }
    }
} 