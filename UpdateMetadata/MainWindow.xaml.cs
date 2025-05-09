﻿using System.Text;
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
        testModeToggle.IsChecked = RawMetadataUpdater.testMetadata;
        
        // Initialize progress counter to zeros
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
            statusDisplay.Text = "Sync completed successfully";
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
        videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 06 CLT EOMW\\\\2024\\\\2024_03_18_12_13_16 ScanEagle LD\\\\CH1_2024_03_18_13_45_38.ts";
        //videoId.PathToVideo = @"Y:\\\\Flight Tests\\\\Alticam 06\\\\2022_04_07_13_20_24_06EOIR_Local\\\\CH0_2022_04_07_13_47_37.ts";
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
                var csvMetadataFields = await
                    CsvToRawMetadata.ReadCSV(csvPath);

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
    private async void TestProgressButton_Click(object sender, RoutedEventArgs e)
    {
        // Simulate progress with 100 steps
        const int totalSteps = 100;
        
        testProgressButton.IsEnabled = false;
        statusDisplay.Text = "Testing progress counter...";
        
        try
        {
            for (int i = 1; i <= totalSteps; i++)
            {
                // Update the progress counter
                LogRawMetadataProcess.LogProgress(i, totalSteps);
                
                // Simulate processing delay
                await Task.Delay(50);
            }
            
            statusDisplay.Text = "Progress counter test completed";
        }
        catch (Exception ex)
        {
            statusDisplay.Text = $"Error during progress test: {ex.Message}";
        }
        finally
        {
            testProgressButton.IsEnabled = true;
        }
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
}