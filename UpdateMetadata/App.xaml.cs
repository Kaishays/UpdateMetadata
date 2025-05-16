using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MetadataBaseUI.UpdateMetadataSentry;

namespace UpdateMetadata;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Initialize Sentry early in the app lifecycle - must be in constructor, not OnStartup
        MetadataBaseUI.UpdateMetadataSentry.Sentry.Init("updatemetdata", "https://15bcb16757f2187a24f636e1dcf2b2af@o388708.ingest.us.sentry.io/4509329016553472", false);
        
        // Set up global exception handlers
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        this.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        // Capture unobserved task exceptions (async/await)
        MetadataBaseUI.UpdateMetadataSentry.Sentry.CaptureException(e.Exception, false);
        
        // Mark as observed to prevent the application from crashing
        e.SetObserved();
    }

    private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Capture UI thread exceptions
        MetadataBaseUI.UpdateMetadataSentry.Sentry.CaptureException(e.Exception, false);
        
        // Mark as handled to prevent the application from crashing
        // Remove this line if you want the application to crash after reporting
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Capture non-UI thread exceptions
        if (e.ExceptionObject is Exception exception)
        {
            MetadataBaseUI.UpdateMetadataSentry.Sentry.CaptureException(exception, false);
        }
        
        // Note: This event cannot prevent the application from terminating
    }
}

