using System.Windows;

namespace KlvExtractor.VideoPlayer
{
   public static class ResetPlayer
    {
        public static void Reset()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StoreVidPlayer.m_KlvPlayer.Stop();
                StoreVidPlayer.totalKlvFrames = 0;
            });
        }
    }
}


// extrra 

//             VidConVars.verifyFramesArriveFromVideo.Reset();
//             StoreVidPlayer.playerState_IsStartingLoopCount = 0;
