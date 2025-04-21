using System.IO;

namespace UpdateMetadata.tests
{
    public static class FindMatchingCsv_
    {
        public static (bool, string) FindMatchingCsv(TableInstances.VideoID videoId)
        {
            string csvFilePath = Path.ChangeExtension(videoId.PathToVideo, ".csv");

            if (File.Exists(csvFilePath))
            {
                return (true, csvFilePath);

            }
            return (false, "");
        }
    }
}
