using StCoreWr;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UpdateMetadata.videoplayer
{
    public static class HandleCorruptedVids
    {
        public static int playerState_IsStartingLoopCount = 0;
        public static Stopwatch verifyFramesArriveFromVideo;
        public static bool CheckPlayerStateError(Player_State playerState)
        {
            if (playerState == Player_State.Starting)
            {
                playerState_IsStartingLoopCount++;
                if (playerState_IsStartingLoopCount >
                    RuntimeVariables.limitOfPlayerStateIsStartingLoop)
                {
                    return true;
                }
            }
            else if (playerState == Player_State.Running)
            {
                if (verifyFramesArriveFromVideo.ElapsedMilliseconds
                    > RuntimeVariables.limitOfMillisecondsNoFrameArrived)
                {
                    return true;
                }
            }
            return playerState == Player_State.Stopped ||
                    playerState == Player_State.Error;
        }
        public static void FramesArrive()
        {
            verifyFramesArriveFromVideo.Reset();
            verifyFramesArriveFromVideo.Start();
        }
        public static void StartVerifyFramesArriveFromVideo()
        {
            verifyFramesArriveFromVideo.Start();
        }
        public static void CreateVerifyFramesArriveFromVideo()
        {
            verifyFramesArriveFromVideo = new Stopwatch();
        }
    }
}
