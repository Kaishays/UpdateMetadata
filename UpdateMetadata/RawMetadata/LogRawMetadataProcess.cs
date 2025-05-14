using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace UpdateMetadata.RawMetadata
{
    public static class LogRawMetadataProcess
    {
        public static void LogProgress(int index, int totalCount)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.UpdateProgressCounter(index, totalCount);
                }
            });
        }
    }
}
