using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            results.HasRawMetadataInDb = await RawKlvInDbTest
                .TestIfRawMetadataInDB(videoId, csvMetadataFields);

            results.HasValidCsvVideoRatio = await CsvSizeToVidSizeRatio
                    .CheckIfCSV_Video_Threshold(csvMetadataFields, videoId.PathToVideo);

            results.HasValidUtcTimestamps = await UtcTimeTest
                .ValidateUtcTimestamps(csvFilePath);

            Debug.WriteLine($"Validation Results: HasRawMetadataInDb={results.HasRawMetadataInDb}, HasValidCsvVideoRatio={results.HasValidCsvVideoRatio}, HasValidUtcTimestamps={results.HasValidUtcTimestamps}");

            return results;
        }
    }
}
