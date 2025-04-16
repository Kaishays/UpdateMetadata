using KlvPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpdateMetadata.tests;



namespace ValidateKlvExtraction.Tests
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
                        duration = TimeSpan.FromHours(5000);
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
                if (m_KlvPlayer == null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CreateVidPlayerForTests.Initialize();
                    });

                    if (m_KlvPlayer == null)
                    {
                        MessageBox.Show("Failed to initialize video player.");
                        return false;
                    }
                }
                bool init = m_KlvPlayer.Init(vidPath);
                return init;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
    }
}
