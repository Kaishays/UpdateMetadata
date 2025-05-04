using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KlvPlayer;
using StCoreWr;
using StMonitorAgent;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace UpdateMetadata.videoplayer
{
    public static class PlayVideo
    {
        public static bool StartFilePlayback(string filePath)
        {
            if (File.Exists(filePath))
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    bool init = StoreVidPlayer.m_KlvPlayer.Init(filePath);
                    bool start = StoreVidPlayer.m_KlvPlayer.Start();
                    HandleCorruptedVids
                    .StartVerifyFramesArriveFromVideo();
                    return init && start;
                });
            }
            else
                return false;
        }
    }
}