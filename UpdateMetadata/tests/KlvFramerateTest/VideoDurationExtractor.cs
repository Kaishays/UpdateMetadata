using CefSharp;
using KlvPlayer;
using StCoreWr;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using UpdateMetadata.videoplayer;

namespace UpdateMetadata.tests.VidPlayerTests
{
    public static class VideoDurationExtractor
    {
        private const int FallbackDurationHours = 5000;
        public static TimeSpan dur_fail = TimeSpan.FromHours(FallbackDurationHours);
        public static async Task<(bool IsCorrupt, TimeSpan Duration)> ExtractDuration(string videoFilePath)
        {
            try
            {
                CheckIfVidPlayerInit();
                ResetVars.EnsureResetVidPlayerVars();

                if (PlayVideo.StartFilePlayback(videoFilePath))
                {
                    if (await MonitorPlayerState.Manage())
                    {
                        double dur = StoreVidPlayer.m_KlvPlayer.GetDuration(); 
                        if (dur != 0)
                        {
                            TimeSpan dur_sec = TimeSpan.FromSeconds(dur);
                            return (false, dur_sec);
                        }   
                    }
                }
                return (true, dur_fail);
            }
            catch (Exception e)
            {
                return (true, dur_fail);
            }
        }
        private static void CheckIfVidPlayerInit()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (StoreVidPlayer.m_KlvPlayer == null)
                {
                    CreateVidPlayer.Initialize();
                }
            });
        }
    } 
}
