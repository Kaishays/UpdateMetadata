using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.tests
{
    public static class TestTwoConsecutiveHashes
    {
        public static async Task<bool> DoTwoConsecutiveHashesMatch(ulong uniqueVideoId)
        {
            List<TableInstances.z_extractionlogs> allHashRecordsForVideo = 
                await RetrieveAllHashRecordsForVideo(uniqueVideoId);

            return AnalyzeTwoHighestIndexHashes(allHashRecordsForVideo);
        }

        private static async Task<List<TableInstances.z_extractionlogs>> RetrieveAllHashRecordsForVideo(ulong uniqueVideoId)
        {
            try
            {
                return await MySQLDataAccess.QuerySQL<TableInstances.z_extractionlogs, dynamic>(
                    SQL_QueriesStore.ZExtractionLogs.selectAll,
                    new { UniqueVideoID = uniqueVideoId },
                    NameLibrary.General.connectionString
                );
            }
            catch
            {
                return new List<TableInstances.z_extractionlogs>();
            }
        }

        private static bool AnalyzeTwoHighestIndexHashes(List<TableInstances.z_extractionlogs> hashRecords)
        {
            if (DoesVideoHaveLessThanTwoHashRecords(hashRecords))
                return false;

            List<TableInstances.z_extractionlogs> sortedByIndexDescending = 
                SortHashRecordsByIndexDescending(hashRecords);

            return DoTwoHighestIndexesHaveMatchingHashes(sortedByIndexDescending);
        }

        private static bool DoesVideoHaveLessThanTwoHashRecords(List<TableInstances.z_extractionlogs> hashRecords)
        {
            return hashRecords.Count < 2;
        }

        private static List<TableInstances.z_extractionlogs> SortHashRecordsByIndexDescending(
            List<TableInstances.z_extractionlogs> hashRecords)
        {
            return hashRecords.OrderByDescending(record => record.RunIndex).ToList();
        }

        private static bool DoTwoHighestIndexesHaveMatchingHashes(
            List<TableInstances.z_extractionlogs> sortedHashRecords)
        {
            string highestIndexHash = sortedHashRecords[0].CsvHash;
            string secondHighestIndexHash = sortedHashRecords[1].CsvHash;

            return string.Equals(highestIndexHash, secondHighestIndexHash, StringComparison.Ordinal);
        }
    }
}
