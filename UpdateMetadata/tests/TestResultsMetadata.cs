using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.tests
{
    public class TestResultsMetadata
    {
        public bool HasRawMetadataInDb { get; set; }
        public bool HasValidCsvVideoRatio { get; set; }
        public bool HasValidUtcTimestamps { get; set; }
        public bool CsvDataTooLong { get; set; }
        public bool HasTwoConsecutiveHashesMatch { get; set; }
        public bool ShouldWriteMetadataToDB => !HasRawMetadataInDb &&
                                    HasValidCsvVideoRatio &&
                                    HasValidUtcTimestamps &&
                                    HasTwoConsecutiveHashesMatch;
        public bool ShouldReextract => !HasValidCsvVideoRatio ||
                                        !HasValidUtcTimestamps ||
                                        !HasTwoConsecutiveHashesMatch;
    }
}
