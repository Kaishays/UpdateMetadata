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
}