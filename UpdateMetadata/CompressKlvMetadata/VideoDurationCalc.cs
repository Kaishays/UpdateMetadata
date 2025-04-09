using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.CompressKlvMetadata
{
    public static class VideoDurationCalc // Not implemented //
    {
        /*public static async Task ManageDur()
        {
            List<TableInstances.VideoID> allIds = await GetVideoPaths();

        }
        public static async Task<List<TableInstances.VideoID>> GetVideoPaths()
        {
            List<TableInstances.VideoID> fileID_and_Path_List = new List<TableInstances.VideoID>();

            fileID_and_Path_List = await MySQLDataAccess.QuerySQL<TableInstances.VideoID, dynamic>(
                SQL_QueriesStore.VideoID.selectFrom,
                null, NameLibrary.General.connectionString);

            return fileID_and_Path_List;
        }
        public class TempDurationID
        {
            public TableInstances.VideoID videoID1;
            public double dur;
            public TempDurationID(double _dur, TableInstances.VideoID _videoID1)
            {
                videoID1 = _videoID1;
                dur = _dur;
            }
        }
        public static async Task<List<double>> GetDur(List<TableInstances.VideoID> allIds)
        {
            List<TempDurationID> durationResults = new List<TempDurationID>();

            foreach (var id in allIds)
            {
                string path = id.PathToVideo;

                // Replace with your actual asynchronous method to compute the video duration.
                double duration = await VideoDurationCalculator.GetDurationAsync(path);

                TempDurationID result = new TempDurationID(duration, id);
                durationResults.Add(result);
            }

            return duration
                }
        public static class CreateCompredM_Class
        {

        }*/


    }
}
