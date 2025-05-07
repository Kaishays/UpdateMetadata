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
using System.IO;
using System.Windows.Media.Media3D;
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
        videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 14\\\\2018\\\\Flight Test 30october18 Integrator\\\\Video\\\\Alticam_CH3__2018_10_30_13_43_02.ts";
        //videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 06\\\\2022_04_07_13_20_24_06EOIR_Local\\\\CH0_2022_04_07_13_47_37.ts";
        //videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam CLT LWIR\\\\4_4_19 Flight_Test\\\\Alticam_CH1__2019_04_04_13_05_02(1).ts";
        string sql = "SELECT UniqueVideoID FROM metadatabase.video_id WHERE PathToVideo LIKE '%" + videoId.PathToVideo + "%'";
        List<ulong> temp = await MySQLDataAccess.QuerySQL<ulong>(sql, NameLibrary.General.connectionString);
        videoId.UniqueVideoID = temp[0];

        if (!VideoCorupted.CheckFile_Corrupted(videoId.PathToVideo) &&
            TsVideoFileTest.IsValidVideoFile(videoId.PathToVideo))
        {
            (bool csvFound, string csvPath) = FindMatchingCsv_.FindMatchingCsv(videoId);

            if (true)
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
    private void CountTsFilesButton_Click(object sender, RoutedEventArgs e)
    {
        CountTsFilesButton.IsEnabled = false;
        StatusText.Text = "Counting TypeScript files...";
        
        try
        {
            string directoryPath = DirectoryPathTextBox.Text;
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                StatusText.Text = "Please enter a directory path";
                return;
            }

            if (!Directory.Exists(directoryPath))
            {                StatusText.Text = "Directory does not exist";
                return;
            }

            int tsFileCount = CountTsFiles(directoryPath);
            StatusText.Text = $"Found {tsFileCount} TypeScript files in the directory";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error counting files: {ex.Message}";
        }
        finally
        {
            CountTsFilesButton.IsEnabled = true;
        }
    }
    private int CountTsFiles(string directoryPath)
    {
        return Directory.GetFiles(directoryPath, "*.ts", SearchOption.AllDirectories).Length;
    }
}