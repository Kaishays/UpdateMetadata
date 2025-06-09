using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateMetadata.tests.KlvFramerateTest;
using ValidateKlvExtraction.Tests;

namespace UpdateMetadata.tests
{
    public static class TestManagerMetadata
    {
        public static async Task<TestResultsMetadata> ValidateMetadata(
           string csvFilePath,
           TableInstances.VideoID videoId,
           List<string[]> csvMetadataFields)
        {
            var results = new TestResultsMetadata();

            results.HasRawMetadataInDb = false;

            results.HasValidCsvVideoRatio = true;

            results.HasValidUtcTimestamps = true;

            results.HasTwoConsecutiveHashesMatch = true;

            return results;
        }

        public static string FormatUtcTimestamp(DateTime timestamp)
        {
            return timestamp.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'");
        }
    }
}
