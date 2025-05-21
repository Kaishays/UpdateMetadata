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
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Application = System.Windows.Application;
using UpdateMetadata.CompressKlvMetadata;
namespace UpdateMetadata;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static MainWindow current;
   
    public MainWindow()
    {
        InitializeComponent();
        current = this;
        _ = InitializeAsync();
        testModeToggle.IsChecked = RawMetadataUpdater.testMetadata;
         UpdateProgressCounter(0, 0);
    }
    public static async Task InitializeAsync()
    {
        await NameLibrary.CreateNameLibrary();
    }
    /// <summary>
    /// Updates the progress counter display with current progress information
    /// </summary>
    /// <param name="currentIndex">Current processing index</param>
    /// <param name="totalCount">Total number of items to process</param>
    public void UpdateProgressCounter(int currentIndex, int totalCount)
    {
        // Update the current index display
        currentIndexDisplay.Text = currentIndex.ToString("N0");
        
        // Update the total count display
        totalCountDisplay.Text = totalCount.ToString("N0");
        
        // Calculate and update completion percentage
        double percentComplete = totalCount > 0 ? (double)currentIndex / totalCount * 100 : 0;
        completionPercentDisplay.Text = $"{percentComplete:F1}%";
        
        // Also update status text if actively processing
        if (currentIndex > 0 && totalCount > 0)
        {
            statusDisplay.Text = $"Processing item {currentIndex:N0} of {totalCount:N0}";
        }
    }
    private async void SyncButton_Click(object sender, RoutedEventArgs e)
    {
        syncYDriveButton.IsEnabled = false;
        statusDisplay.Text = "Syncing...";
        try
        {
            await SyncY_DriveToDatabase.SyncDriveToDB();
        }
        catch (System.Exception ex)
        {
            statusDisplay.Text = $"Error: {ex.Message}";
        }
        finally
        {
            syncYDriveButton.IsEnabled = true;
        }
    }
    private async void 
    ForDebugOnly_SingleFileCheckButton_Click(object sender, RoutedEventArgs e)
    {
        //  Y:\Flight Tests\AC10\Rockwell Collins Flight 17Mar15\video_17March15\Alticam_2015_3_17_14_6_30.ts
        // in Db = '29476'
        statusDisplay.Text = "Single file check initiated...";
        TableInstances.VideoID videoId = new TableInstances.VideoID();
        videoId.UniqueVideoID = 5348024557550169;

        await ManageCompress.TryCompressRawMetadata(videoId);








        videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 06 CLT EOMW\\\\2024\\\\2024_03_18_12_13_16 ScanEagle LD\\\\CH1_2024_03_18_13_45_38.ts";
        videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 06EOIR2\\\\2024_11_06_16_21_06 EOIR2 1.2.1 Roll Tilt checkout\\\\Clip_CH1_2024_11_06_16_39_35.ts";
       // videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 06\\\\2022_04_07_13_20_24_06EOIR_Local\\\\CH0_2022_04_07_13_47_37.ts";
        //videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam CLT LWIR\\\\4_4_19 Flight_Test\\\\Alticam_CH1__2019_04_04_13_05_02(1).ts";
        string sql = "SELECT UniqueVideoID FROM metadatabase.video_id WHERE PathToVideo LIKE '%" + videoId.PathToVideo + "%'";
        List<ulong> temp = await MySQLDataAccess.QuerySQL<ulong>(sql, NameLibrary.General.connectionString);
        videoId.UniqueVideoID = temp[0];

        if (!VideoCorupted.CheckFile_Corrupted(videoId.PathToVideo) &&
            TsVideoFileTest.IsValidVideoFile(videoId.PathToVideo))
        {
            (bool csvFound, string csvPath) = FindMatchingCsv_.FindMatchingCsv(videoId);

            if (csvFound)
            {

                (bool isDataToLong, List<string[]> csvMetadataFields) = await CsvToRawMetadata.ReadCSV(csvPath);

                TestResultsMetadata testResults =
                    await TestManagerMetadata.ValidateMetadata(
                    csvPath, videoId, csvMetadataFields);

            }
        }
        statusDisplay.Text = "Ready";
    }
    private async void GetVideoCountButton_Click(object sender, RoutedEventArgs e)
    {
        metadataCountButton.IsEnabled = false;
        statusDisplay.Text = "Counting videos...";
        
        try
        {
            string sql = "SELECT COUNT(UniqueVideoID) FROM metadatabase.raw_metadata";
            List<long> result = await MySQLDataAccess.QuerySQL<long>(sql, NameLibrary.General.connectionString);
            statusDisplay.Text = $"Total rows in raw_metadata: {result[0]:N0}";
        }
        catch (System.Exception ex)
        {
            statusDisplay.Text = $"Error getting video count: {ex.Message}";
        }
        finally
        {
            metadataCountButton.IsEnabled = true;
        }
    }
    private void TestMetadataToggle_Click(object sender, RoutedEventArgs e)
    {
        RawMetadataUpdater.testMetadata = testModeToggle.IsChecked ?? true;
        statusDisplay.Text = $"Test metadata {(RawMetadataUpdater.testMetadata ? "enabled" : "disabled")}";
    }
    private void CountTsFilesButton_Click(object sender, RoutedEventArgs e)
    {
        fileCountButton.IsEnabled = false;
        statusDisplay.Text = "Counting TypeScript files...";
        
        try
        {
            string directoryPath = directoryInput.Text;
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                statusDisplay.Text = "Please enter a directory path";
                return;
            }

            if (!Directory.Exists(directoryPath))
            {                statusDisplay.Text = "Directory does not exist";
                return;
            }

            int tsFileCount = CountTsFiles(directoryPath);
            statusDisplay.Text = $"Found {tsFileCount} TypeScript files in the directory";
        }
        catch (Exception ex)
        {
            statusDisplay.Text = $"Error counting files: {ex.Message}";
        }
        finally
        {
            fileCountButton.IsEnabled = true;
        }
    }
    private int CountTsFiles(string directoryPath)
    {
        return Directory.GetFiles(directoryPath, "*.ts", SearchOption.AllDirectories).Length;
    }
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        // Use regular expression to allow only digits
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}