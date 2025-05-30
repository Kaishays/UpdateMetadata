using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpdateMetadata.RunLogHashing
{
    public static class RunIndexCalculator
    {
        public static async Task<uint> CalculateNextAvailableRunIndex(ulong uniqueVideoId)
        {
            List<uint> existingRunIndices = await HashLogDatabaseService.GetAllExistingRunIndicesFromDatabase(uniqueVideoId);
            return FindNextAvailableRunIndex(existingRunIndices);
        }

        private static uint FindNextAvailableRunIndex(List<uint> existingRunIndices)
        {
            if (!existingRunIndices.Any())
                return 0;

            existingRunIndices.Sort();
            uint nextRunIndex = 0;
            
            foreach (uint existingIndex in existingRunIndices)
            {
                if (nextRunIndex < existingIndex)
                    break;
                nextRunIndex = existingIndex + 1;
            }
            
            return nextRunIndex;
        }
    }
} 