using Newtonsoft.Json.Linq;
using StCoreWr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata.videoplayer
{
    public static class ResetVars
    {
        public static void EnsureResetVidPlayerVars()
        {
            bool areReset = ResetVidPlayerVars();
            while (!areReset)
            {
                areReset = ResetVidPlayerVars();
            }
        }
        public static bool ResetVidPlayerVars()
        {
            bool allReset = true;

            if (HandleCorruptedVids.verifyFramesArriveFromVideo
                != null
                && HandleCorruptedVids.verifyFramesArriveFromVideo.IsRunning)
            {
                HandleCorruptedVids.verifyFramesArriveFromVideo.Stop();
                HandleCorruptedVids.verifyFramesArriveFromVideo.Reset();
            }
            else
            {
                HandleCorruptedVids.verifyFramesArriveFromVideo
                    = new Stopwatch();
            }

            if (HandleCorruptedVids.playerState_IsStartingLoopCount
                != 0)
            {
                HandleCorruptedVids.playerState_IsStartingLoopCount = 0;
            }

            if (StoreVidPlayer.m_KlvPlayer.PlayerState
                != Player_State.Stopped)
            {
                StoreVidPlayer.m_KlvPlayer.Stop();
                allReset = false;
            }

            if (StoreVidPlayer.totalKlvFrames
                != 0)
            {
                StoreVidPlayer.totalKlvFrames = 0;
            }

            return allReset;
        }
    }
}
