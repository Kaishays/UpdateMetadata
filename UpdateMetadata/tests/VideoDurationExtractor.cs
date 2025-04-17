using KlvPlayer;
using StCoreWr;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using UpdateMetadata.tests;

namespace ValidateKlvExtraction.Tests
{
    public static class VideoDurationExtractor
    {
        private static CKlvPlayer videoPlayer;
        private const int DefaultWaitTimeMilliseconds = 2000;
        private const int FallbackDurationHours = 5000;

        public static async Task<(bool IsCorrupt, TimeSpan Duration)> ExtractDuration(string videoFilePath)
        {
            return await Application.Current.Dispatcher.Invoke(async () =>
            {
                if (!InitializePlayerWithVideo(videoFilePath))
                {
                    return (false, TimeSpan.FromHours(FallbackDurationHours));
                }

                if (!await WaitForPlayerReady())
                {
                    StopPlayer();
                    return (false, TimeSpan.FromHours(FallbackDurationHours));
                }

                TimeSpan duration = GetPlayerDuration();
                bool isCorrupt = duration.TotalSeconds == 0;
                
                if (isCorrupt)
                {
                    duration = TimeSpan.FromHours(FallbackDurationHours);
                }

                return (isCorrupt, duration);
            });
        }

        private static bool InitializePlayerWithVideo(string videoFilePath)
        {
            try
            {
                if (videoPlayer == null)
                {
                    return CreateAndSetupPlayer(videoFilePath);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing video player: {ex.Message}", 
                    "Player Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static bool CreateAndSetupPlayer(string videoFilePath)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                videoPlayer = CreateVidPlayerForTests.CreateAndActivatePlayer();

                if (videoPlayer == null)
                {
                    MessageBox.Show("Failed to initialize video player.", 
                        "Player Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return InitVideoInPlayer(videoFilePath);
            });
        }

        private static bool InitVideoInPlayer(string videoFilePath)
        {
            bool initialized = videoPlayer.Init(videoFilePath);
            bool started = videoPlayer.Start();
            
            return initialized && started;
        }

        private static async Task<bool> WaitForPlayerReady()
        {
            if (videoPlayer == null)
            {
                return false;
            }

            return await WaitForRunningState();
        }

        private static async Task<bool> WaitForRunningState()
        {
            while (videoPlayer.PlayerState != Player_State.Running)
            {
                await Task.Delay(DefaultWaitTimeMilliseconds);
                Debug.Write(videoPlayer.PlayerState);

                if (IsErrorState(videoPlayer.PlayerState))
                {
                    StopPlayer();
                    return false;
                }
            }
            return true;
        }

        private static TimeSpan GetPlayerDuration()
        {
            return TimeSpan.FromSeconds(videoPlayer.GetDuration());
        }

        private static void StopPlayer()
        {
            videoPlayer?.Stop();
        }

        private static bool IsErrorState(Player_State state)
        {
            return state == Player_State.Paused ||
                   state == Player_State.Completed ||
                   state == Player_State.Error ||
                   state == Player_State.DurationChanged ||
                   state == Player_State.SegmentChanged ||
                   state == Player_State.SegmentListChanged ||
                   state == Player_State.SegmentStarted ||
                   state == Player_State.SegmentCompleted ||
                   state == Player_State.PlayerChanged ||
                   state == Player_State.Demo_Expired ||
                   state == Player_State.VideoDeleted ||
                   state == Player_State.SyntheticFrameOn ||
                   state == Player_State.SyntheticFrameOff ||
                   state == Player_State.SdpCreated;
        }
    }
}
