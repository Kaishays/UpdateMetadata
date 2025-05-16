using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MetadataBaseUI.UpdateMetadataSentry
{
    public class BackgroundInternetCheck
    {
        private static Thread threadCheckForInternet = null;
        public static bool IsThereInternet = false;
        public static event EventHandler<EventArgs> OnInternetStatusChanged;

        public static void Start()
        {
            if (IsRunning() == false)
            {
                Stop();

                threadCheckForInternet = new Thread(checkForInternetProc);
                threadCheckForInternet.Name = "BackgroundInternetCheckThread";
                threadCheckForInternet.IsBackground = true;
                threadCheckForInternet.Start();
            }
        }

        public static void Stop()
        {
            threadCheckForInternet?.Abort();
            threadCheckForInternet = null;
        }

        public static bool IsRunning()
        {
            return (threadCheckForInternet != null && threadCheckForInternet.IsAlive);
        }


        static WebClient webClient = null;
        private static void checkForInternetProc()
        {
            if (webClient == null)
            {
                webClient = new WebClient();
                webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            }

            while (true)
            {
                bool newIsInternetVal = checkForInternetConnection();
                if (newIsInternetVal != IsThereInternet)
                {
                    IsThereInternet = newIsInternetVal;
                    OnInternetStatusChanged?.Invoke(null, new EventArgs());
                }

                Thread.Sleep(5000);
            }
        }

        private static bool checkForInternetConnection()
        {
            try
            {
                webClient.OpenRead("http://google.com/generate_204");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
