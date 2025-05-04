using KlvPlayer;
using StCoreWr;
using StMonitorAgent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UpdateMetadata.videoplayer
{
    public static class StoreVidPlayer
    {
        public static CKlvPlayer m_KlvPlayer = null;
        public static bool m_fPaused = false;
        public static int totalKlvFrames = 0;
        public static class Events
        {
            /// <summary>
            /// OnPlayerEvent receives notifications on the player state change 
            /// </summary>
            /// <param name="ev">Player event</param>
            /// <param name="info">Event info</param>
            /// <param name="param">Event param</param>
            public static void OnPlayerEvent(Player_State ev, string info, long param)
            {

                switch (ev)
                {
                    case Player_State.Running:

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            if (m_fPaused)
                            {
                                m_fPaused = false;
                            }
                        }));
                        break;

                    case Player_State.Paused:
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            m_fPaused = true;
                        }));
                        break;

                    case Player_State.Completed:             
                        break;

                    case Player_State.Demo_Expired:   //Demo Session expired                  
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            MessageBox.Show("Player Demo Expired", "Demo Expired");
                        }));
                        break;

                    default:
                        break;
                }
            }

            /// <summary>
            ///  OnSyncFrameEvent  - This method is called when a new Video, Klv or private data are found in the stream. In case of SYNC Klv / Data these packets are syncronized
            /// </summary>
            /// <param name="streamList">Synchronized list of video frames, klv and private data</param>  
            public static void OnSyncFrameEvent(List<StreamFrameInfoWr> streamList)
            {
                HandleCorruptedVids.FramesArrive();
                streamList.ForEach(delegate (StreamFrameInfoWr streamFrame)
                {
                    switch (streamFrame.streamType)
                    {
                        case StreamType.VIDEO:
                            {
                                // Here we got an uncompressed frame        
                                Debug.WriteLine("Video");
                                VideoFrameInfoWr vf = streamFrame as VideoFrameInfoWr;
                                if (vf == null || vf.data == null)
                                    return;
                            }
                            break;

                        case StreamType.KLV:
                            {
                                KlvFrameInfoWr kf = streamFrame as KlvFrameInfoWr;
                                totalKlvFrames++;
                                Debug.WriteLine("KLV");
                            }
                            break;

                        case StreamType.PRIVATE_DATA:
                            {
                                Debug.WriteLine("Private");
                                DataFrameInfoWr df = streamFrame as DataFrameInfoWr;
                            }
                            break;
                    }
                });
            }
            public static void OnErrorEvent(Error_Type e, string err)
            {
                string errMessage = string.Format("{0} {1}", e, err);
                VideoPlayerErrorHandler.HandleError(errMessage);
            }
        }
    }
}
