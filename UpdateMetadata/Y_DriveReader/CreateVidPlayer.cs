using System.Windows;
using KlvPlayer;
using StCoreWr;
using System.Diagnostics;
using UpdateMetadata.Y_DriveReader;

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
                    SetVidPlayer(cKlvPlayer);
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
        private static void SetVidPlayer(CKlvPlayer cKlvPlayer)
        {
            VidDurationGetter.m_KlvPlayer = cKlvPlayer;
        }
    }
}