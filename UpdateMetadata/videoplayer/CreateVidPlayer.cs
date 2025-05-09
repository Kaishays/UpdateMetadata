using System;
using System.Windows;
using KlvPlayer;
using StCoreWr;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UpdateMetadata.videoplayer
{
    public static class CreateVidPlayer
    {
        public static void Initialize()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StoreVidPlayer.m_KlvPlayer = new CKlvPlayer();
                    CKlvPlayer cKlvPlayer = new CKlvPlayer();
                    cKlvPlayer = ActivateLicense(cKlvPlayer);
                    cKlvPlayer = ConfigureVariables(cKlvPlayer);
                    cKlvPlayer = AddPlayerEvents(cKlvPlayer);
                    SetVidPlayer(cKlvPlayer);
                    HandleCorruptedVids
                    .CreateVerifyFramesArriveFromVideo();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Initializing Player:" + ex.Message);
            }
        }
        private static CKlvPlayer ActivateLicense(CKlvPlayer cKlvPlayer)
        {
            string projectDirectory = GetProjectDirectory();
            string licenseFileName = "StanagPlayerUnlocked-12-22.lic";
            string licensePath = Path.Combine(projectDirectory, licenseFileName);
            string licenseKey = "A13EC1C1-3615E88F-2ABC85C7-E82242D5";

            if (!string.IsNullOrEmpty(licensePath) && !string.IsNullOrEmpty(licenseKey))
            {
                Trace.WriteLine($"Activating the player with license at: {licensePath}");
                if (cKlvPlayer.Activate("KlvPlayer", licensePath, licenseKey) == false)
                {
                    MessageBox.Show("Error Activating ImpleoTv License");
                }
            }
            return cKlvPlayer;
        }

        private static string GetProjectDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Directory.GetParent(baseDirectory).FullName;
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
            cKlvPlayer.AudioMayBeMissing = true;
            cKlvPlayer.VideoCaptureMode.uncompressedVideo = UncompressedVideoModeWr.UncompressedVideoMode_Rgb;
            cKlvPlayer.FrameAccuracyRequiresSequenceHeaders = false;
            cKlvPlayer.SetRate(10000);
            
            return cKlvPlayer;
        }
        private static void SetVidPlayer(CKlvPlayer cKlvPlayer)
        {
            StoreVidPlayer.m_KlvPlayer = cKlvPlayer;
        }
    }
}