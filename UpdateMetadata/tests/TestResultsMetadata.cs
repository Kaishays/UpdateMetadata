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
        public bool ShouldWriteMetadataToDB => !HasRawMetadataInDb &&
                                    HasValidCsvVideoRatio &&
                                    HasValidUtcTimestamps;
        public bool ShouldReextract => !HasValidCsvVideoRatio ||
                                        !HasValidUtcTimestamps;
    }
}
