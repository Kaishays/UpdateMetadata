using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using UpdateMetadata.ReadDatabase;
using System.Configuration;
using UpdateMetadata.tests;
using UpdateMetadata.Y_DriveReader;
using ValidateKlvExtraction.Tests;
using UpdateMetadata.RawMetadata;
namespace UpdateMetadata;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        _ = InitializeAsync();
        TestMetadataToggle.IsChecked = RawMetadataUpdater.testMetadata;
    }
    public static async Task InitializeAsync()
    {
        await NameLibrary.CreateNameLibrary();
    }

    private async void SyncButton_Click(object sender, RoutedEventArgs e)
    {
        SyncButton.IsEnabled = false;
        StatusText.Text = "Syncing...";
        
        try
        {
            await SyncY_DriveToDatabase.SyncDriveToDB();
            StatusText.Text = "Sync completed successfully";
        }
        catch (System.Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            SyncButton.IsEnabled = true;
        }
    }
    private async void 
    ForDebugOnly_SingleFileCheckButton_Click(object sender, RoutedEventArgs e)
    {

        //  Y:\Flight Tests\AC10\Rockwell Collins Flight 17Mar15\video_17March15\Alticam_2015_3_17_14_6_30.ts
        // in Db = '29476'

        StatusText.Text = "Single file check initiated...";
        TableInstances.VideoID videoId = new TableInstances.VideoID();
        videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam09\\\\MWIR36\\\\MWIR36-FLIR Flights\\\\2021_09_20 (Drift Boat Columbia)\\\\Search Gimbal (front)\\\\CH1_2021_09_20_22_32_27.ts";

        string sql = "SELECT UniqueVideoID FROM metadatabase.video_id WHERE PathToVideo LIKE '%" + videoId.PathToVideo + "%'";
        List<ulong> temp = await MySQLDataAccess.QuerySQL<ulong>(sql, NameLibrary.General.connectionString);
        videoId.UniqueVideoID = temp[0];

        RawMetadataUpdater.TryUpdateRawMetadata(videoId);

        if (!VideoCorupted.CheckFile_Corrupted(videoId.PathToVideo) &&
            TsVideoFileTest.IsValidVideoFile(videoId.PathToVideo))
        {
            (bool csvFound, string csvPath) = FindMatchingCsv_.FindMatchingCsv(videoId);

            if (csvFound)
            {
                var csvMetadataFields = await
                    CsvToRawMetadata.ReadCSV(csvPath);

                TestResultsMetadata testResults =
                    await TestManagerMetadata.ValidateMetadata(
                    csvPath, videoId, csvMetadataFields);

            }
        }
        StatusText.Text = "Ready";
    }

    private async void GetVideoCountButton_Click(object sender, RoutedEventArgs e)
    {
        GetVideoCountButton.IsEnabled = false;
        StatusText.Text = "Counting videos...";
        
        try
        {
            string sql = "SELECT COUNT(UniqueVideoID) FROM metadatabase.raw_metadata";
            List<long> result = await MySQLDataAccess.QuerySQL<long>(sql, NameLibrary.General.connectionString);
            StatusText.Text = $"Total rows in raw_metadata: {result[0]:N0}";
        }
        catch (System.Exception ex)
        {
            StatusText.Text = $"Error getting video count: {ex.Message}";
        }
        finally
        {
            GetVideoCountButton.IsEnabled = true;
        }
    }

    private void TestMetadataToggle_Click(object sender, RoutedEventArgs e)
    {
        RawMetadataUpdater.testMetadata = TestMetadataToggle.IsChecked ?? true;
        StatusText.Text = $"Test metadata {(RawMetadataUpdater.testMetadata ? "enabled" : "disabled")}";
    }
}