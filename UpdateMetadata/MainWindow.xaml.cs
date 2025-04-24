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
        videoId.UniqueVideoID = 281474976710812;
        videoId.PathToVideo = "Y:\\\\Flight Tests\\\\AC10\\\\Rockwell Collins Flight 17Mar15\\\\video_17March15\\\\Alticam_2015_3_17_14_6_30.ts";


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
}