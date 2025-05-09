using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Windows;
using System.Windows.Threading;

namespace UpdateMetadata.RawMetadata
{
    public static class LogRawMetadataProcess
    {
        public static void LogProgress(int index, int totalCount)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.UpdateProgressCounter(index, totalCount);
                }
            });
        }
    }
}
