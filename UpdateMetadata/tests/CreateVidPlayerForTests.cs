using System.Windows;
using KlvPlayer;
using StCoreWr;
using System.Diagnostics;
using UpdateMetadata.Y_DriveReader;

namespace UpdateMetadata.tests
{
    public static class CreateVidPlayerForTests
    {
        private const string LicensePath = @"StanagPlayerUnlocked-12-22.lic";
        private const string LicenseKey = "A13EC1C1-3615E88F-2ABC85C7-E82242D5";
        private const string LicenseProductName = "KlvPlayer";

        public static CKlvPlayer CreateAndActivatePlayer()
        {
            try
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    CKlvPlayer player = new CKlvPlayer();

                    if (!TryActivateLicense(player))
                    {
                        MessageBox.Show("Error Activating ImpleoTv License. Player will not function correctly.", "License Activation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return player; 
                    }
                    return player;
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Exception during player creation/activation: {ex.Message}");
                MessageBox.Show($"Failed to initialize video player: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static bool TryActivateLicense(CKlvPlayer player)
        {
            if (string.IsNullOrEmpty(LicensePath) 
                || string.IsNullOrEmpty(LicenseKey))
            {
                Trace.WriteLine("License path or key is missing. Activation skipped.");
                return false;
            }

            Trace.WriteLine($"Activating the player with product '{LicenseProductName}' and license file '{LicensePath}'");
            bool success = player.Activate(LicenseProductName, LicensePath, LicenseKey);

            if (!success)
            {
                Trace.WriteLine("Activation failed.");
            }
            else
            {
                Trace.WriteLine("Activation successful.");
            }
            return success;
        }
    }
}