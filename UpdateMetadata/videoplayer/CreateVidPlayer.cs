using System.Windows;
using KlvPlayer;
using StCoreWr;
using System.Diagnostics;

namespace KlvExtractor.VideoPlayer
{
    public static class CreateVidPlayer
    {
        public static void Initialize()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CKlvPlayer cKlvPlayer = new CKlvPlayer();

                    cKlvPlayer = ActivateLicense(cKlvPlayer);
                    cKlvPlayer = ConfigureVariables(cKlvPlayer);
                    cKlvPlayer = AddPlayerEvents(cKlvPlayer);
                    SetVidPlayer(cKlvPlayer);
                    HandleCorruptedVids
                    .CreateVerifyFramesArriveFromVideo();
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static CKlvPlayer ActivateLicense(CKlvPlayer cKlvPlayer)
        {
            string licensePath = @"StanagPlayerUnlocked-12-22.lic";
            string licenseKey = "A13EC1C1-3615E88F-2ABC85C7-E82242D5";

            if (!string.IsNullOrEmpty(licensePath) && !string.IsNullOrEmpty(licenseKey))
            {
                Trace.WriteLine("Activating the player");
                if (cKlvPlayer.Activate("KlvPlayer", licensePath, licenseKey) == false)
                {
                    MessageBox.Show("Error Activating ImpleoTv License");
                }
            }
            return cKlvPlayer;
        }
        private static CKlvPlayer AddPlayerEvents(CKlvPlayer cKlvPlayer)
        {
            cKlvPlayer.PlayerEvent += new NotifyPlayerEvent(StoreVidPlayer.Events.OnPlayerEvent);
            cKlvPlayer.SyncFrameEvent += new NotifySyncFrame(StoreVidPlayer.Events.OnSyncFrameEvent);
            cKlvPlayer.ErrorEvent += new NotifyError(StoreVidPlayer.Events.OnErrorEvent);

            return cKlvPlayer;
        }
        private static CKlvPlayer ConfigureVariables(CKlvPlayer cKlvPlayer)
        {
            cKlvPlayer.MaxCapturedVideoResolution = new VideoResolutionWr()
            {
                Width = 1920,
                Height = 1080
            };
            cKlvPlayer.RenderVideo = true;
            cKlvPlayer.RenderVideo = true;
            cKlvPlayer.AudioMayBeMissing = true;
            cKlvPlayer.VideoCaptureMode.uncompressedVideo = UncompressedVideoModeWr.UncompressedVideoMode_Rgb;
            cKlvPlayer.FrameAccuracyRequiresSequenceHeaders = false;
            cKlvPlayer.SetRate(1000);

            return cKlvPlayer;
        }
        private static void SetVidPlayer(CKlvPlayer cKlvPlayer)
        {
            StoreVidPlayer.m_KlvPlayer = cKlvPlayer;
        }
    }
}











/*extras: 
 *                     verifyFramesArriveFromVideo = new Stopwatch();
                                ProcessKLV.ResetKlVars();
 public static double FPS;
        public static Stopwatch verifyFramesArriveFromVideo;
        public static int playerState_IsStartingLoopCount;
        public static bool m_dragStarted = false;

        public static DispatcherTimer m_InfoUpdateTimer;

        public static int m_KlvPid = -1;
        public static bool m_fPaused = false;                             
        public static CStMonitorAgent m_MonitorAgent = null;              
        public static bool m_fPlaying = false;   
        public static long m_KlvPacketsCounter = 0;                      
        public static DateTime m_LastMonitorKlvUpdate = DateTime.Now;
       // public static int MaxMonitorKlvUpdateRate = 200;
        public static class Klv
        {
            public static List<CraftKLV> rawKlvList;
            public static JObject JsonKlv;
            public static MISB0601 MISB;
            public static AltiKLV KLV;
        }


 * 
 * 
 */