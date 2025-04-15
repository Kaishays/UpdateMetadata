using StCoreWr;
using System.Diagnostics;

namespace KlvExtractor.VideoPlayer
{
   public static class HandleCorruptedVids
    {
        private static int playerState_IsStartingLoopCount = 0;
        private static Stopwatch verifyFramesArriveFromVideo;
        public static bool CheckPlayerStateError(Player_State playerState)
        {
            if (playerState == Player_State.Starting)
            {
                playerState_IsStartingLoopCount++;
               /* if (playerState_IsStartingLoopCount >
                    RuntimeVariables.limitOfPlayerStateIsStartingLoop)
                {
                    return true;
                }*/
            }
            else if (playerState == Player_State.Running)
            {
               /* if (verifyFramesArriveFromVideo.ElapsedMilliseconds
                    > RuntimeVariables.limitOfMillisecondsNoFrameArrived)
                {
                    return true;
                }*/
            }
            return playerState == Player_State.Stopped ||
                playerState == Player_State.Error;
        }
        public static void FramesArrive()
        {
            verifyFramesArriveFromVideo.Reset();
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
