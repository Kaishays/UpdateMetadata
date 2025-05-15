using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using Sentry;

namespace UpdateMetadata;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        SentrySdk.Init(o =>
        {
            o.Dsn = "https://15bcb16757f2187a24f636e1dcf2b2af@o388708.ingest.us.sentry.io/4509329016553472";
            o.Debug = true;
        });
    }

    void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        SentrySdk.CaptureException(e.Exception);
        e.Handled = true;
    }
}

