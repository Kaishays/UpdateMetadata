using CefSharp;
using KlvPlayer;
using System.Windows;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlvExtractor.VideoPlayer;

namespace UpdateMetadata.Y_DriveReader
{
    public static class VidDurationGetter
    {
        public static CKlvPlayer m_KlvPlayer = null;
        public static (bool, TimeSpan) GetVideoDuration(string videoFilePath)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                if (CreateVidPlayerToCheckDuration(videoFilePath))
                {
                    bool vidDurationCorrupt = false;

                    TimeSpan duration = TimeSpan.FromSeconds(m_KlvPlayer.GetDuration());
                    if (duration.TotalSeconds == 0)
                    {
                        vidDurationCorrupt = true;
                        duration = GetVideoDurationFromFileMetadata(videoFilePath);
                    }
                    return (vidDurationCorrupt, duration);
                }
                return (false, TimeSpan.FromHours(5000));
            });
        }
        private static bool CreateVidPlayerToCheckDuration(string vidPath)
        {
            try
            {
                CreateVidPlayer.Initialize();
                bool init = m_KlvPlayer.Init(vidPath);
                return init;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        private static TimeSpan GetVideoDurationFromFileMetadata(string filePath)
        {
            try
            {
                var shellFile = ShellFile.FromFilePath(filePath);
                var durationValue = shellFile.Properties.System.Media.Duration.Value;
                if (durationValue != null)
                {
                    long durationTicks = (long)durationValue;
                    return TimeSpan.FromTicks(durationTicks);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return TimeSpan.FromHours(5000);
        }
    }
}
