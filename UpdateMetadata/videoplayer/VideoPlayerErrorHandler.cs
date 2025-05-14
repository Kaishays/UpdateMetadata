using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlvPlayer;
using StCoreWr;
using StMonitorAgent;
using System.Windows;
using System.Windows.Threading;
using UpdateMetadata.tests.VidPlayerTests;
using UpdateMetadata.RawMetadata;
using Application = System.Windows.Application;

namespace UpdateMetadata.videoplayer
{
    public static class VideoPlayerErrorHandler
    {
        public static async Task HandleError(string errMessage)
        {
            await RawMetadataUpdater.extractionSemaphore.WaitAsync();
            try
            {
                ResetVars.EnsureResetVidPlayerVars();
                CreateVidPlayer.Initialize();
            }
            finally
            {
                RawMetadataUpdater.extractionSemaphore.Release();
            }
        }
    }
}
