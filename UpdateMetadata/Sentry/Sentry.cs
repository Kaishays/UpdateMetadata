using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;
using System.IO;

namespace MetadataBaseUI.UpdateMetadataSentry
{
    public static class Sentry
    {
        private static readonly string BASE_EXCEPTIONS_FOLDER_PATH = @"C:\Hood Tech\Exceptions\";
        private static string m_AppName;
        private static string m_TurretName;
        private static DateTime StartTime = DateTime.UtcNow;

        public static string AppExceptionFolderPath { get; private set; }
        public static bool UserIsClosingApp = false;

        private static List<string> breadcrumbMsgs = new List<string>();
        private static Mutex mutex = new Mutex();
        public static void StoreBreadcrumb(string message)
        {
            mutex.WaitOne();

            breadcrumbMsgs.Add(message);
            if (breadcrumbMsgs.Count > 5)
            {
                breadcrumbMsgs.RemoveAt(0);
            }

            mutex.ReleaseMutex();
        }
        public static void Init(string appName, string projectKeyString, bool reportMachineAndUserName = false)
        {
            m_AppName = appName;
            AppExceptionFolderPath = Path.Combine(BASE_EXCEPTIONS_FOLDER_PATH, m_AppName);

            SentrySdk.Init(o =>
            {
                o.Dsn = projectKeyString;
                o.AutoSessionTracking = true;

               /* o.SetBeforeSend((sentryEvent, hint) =>
                {
                    if (isIgnoredEvent(sentryEvent) == true || Debugger.IsAttached)
                    {
                        return null; // Cancel the send to sentry's servers by returning null
                    }

                    foreach (var b in breadcrumbMsgs)
                    {
                        sentryEvent.AddBreadcrumb(b);
                    }

                    sentryEvent.AddBreadcrumb("Session Duration: " + (DateTime.UtcNow - StartTime).ToString(@"hh\:mm\:ss"));
                    sentryEvent.AddBreadcrumb("Gimbal: " + m_TurretName);
                    sentryEvent.AddBreadcrumb($"User Closed App: {UserIsClosingApp}");

                    if (reportMachineAndUserName == true)
                    {
                        sentryEvent.AddBreadcrumb(Environment.UserDomainName + "\\" + Environment.UserName + ", PC: " + Environment.MachineName);
                    }

                    breadcrumbMsgs.Clear();

                    if (!BackgroundInternetCheck.IsThereInternet)
                    {
                        saveToDisk(sentryEvent);
                        return null; // Cancel the send to UpdateMetadataSentry's servers by returning null
                    }
                    else
                    {
                        return sentryEvent;
                    }
                });*/
            }); 

            /*if (reportMachineAndUserName)
            {
                try
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        var dict = new Dictionary<string, string>();
                        dict.Add("MachineName", Environment.MachineName);
                        scope.User.Other = dict;
                        scope.User.Username = Environment.UserDomainName + "\\" + Environment.UserName;
                    });
                }
                catch
                {

                }
            }

            BackgroundInternetCheck.OnInternetStatusChanged += BackgroundInternetCheck_OnInternetStatusChanged;
            BackgroundInternetCheck.Start();

            if (BackgroundInternetCheck.IsThereInternet)
            {
                uploadCached();
            }*/
        }

        private static bool isIgnoredEvent(SentryEvent sentryEvent)
        {
            foreach (var e in sentryEvent.SentryExceptions)
            {
                if (e == null)
                {
                    continue;
                }

                // Filter out the offline map "AggregateException" failures thrown
                // by ArcGIS when PC is offline that do not crash the GUI.
                if (e.Type == "System.AggregateException" || e.Mechanism?.Type != null && e.Mechanism.Type.Contains("UnobservedTaskException"))
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetTurretName(string turretName)
        {
            m_TurretName = turretName;
        }

        private static void saveToDisk(SentryEvent sentryEvent)
        {
            string file = Path.Combine(AppExceptionFolderPath, generateRandomFilename(".txt"));
            string content = Environment.MachineName + ", " + Environment.UserDomainName + "\\" + Environment.UserName + "\n";
            content += "Occurred at " + DateTime.Now.ToString() + "\n\n";

            if (sentryEvent.Message != null)
            {
                content += sentryEvent.Message + "\n\n";
            }

            if (sentryEvent.SentryExceptions?.Any() == true)
            {
                foreach (var exc in sentryEvent.SentryExceptions)
                {
                    content += "Exception\n---------------\n";

                    if (exc.Type != null)
                    {
                        content += exc.Type.ToString() + ": \"";
                    }

                    if (exc.Value != null && exc.Value.Length > 0)
                    {
                        content += exc.Value + "\"\n";
                    }

                    if (exc.Stacktrace != null)
                    {
                        for (int i = exc.Stacktrace.Frames.Count - 1; i >= 0; i--)
                        {
                            var frame = exc.Stacktrace.Frames[i];

                            if (frame?.FileName?.Length > 0)
                            {
                                content += frame.FileName + ".";
                            }

                            if (frame?.Module?.Length > 0)
                            {
                                content += frame.Module + ".";
                            }

                            if (frame?.Function?.Length > 0)
                            {
                                content += frame.Function;
                            }

                            if (frame.LineNumber.HasValue)
                            {
                                content += " (line number " + frame.LineNumber + ")" + "\n";
                            }
                            else
                            {
                                content += "\n";
                            }
                        }
                    }
                }
            }
            else
            {
                content += sentryEvent.Exception.StackTrace;
            }

            WriteStringToFile(file, content);
        }

        private static void BackgroundInternetCheck_OnInternetStatusChanged(object sender, EventArgs e)
        {
            if (BackgroundInternetCheck.IsThereInternet == true)
            {
                uploadCached();
            }
        }

        private static string generateRandomFilename(string fileExtension)
        {
            return Path.ChangeExtension(Guid.NewGuid().ToString().Substring(0, 18).Replace("-", ""), fileExtension);
        }

        private static void uploadCached()
        {
            if (!Directory.Exists(AppExceptionFolderPath))
            {
                return;
            }

            var files = Directory.GetFiles(AppExceptionFolderPath, "*.txt", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    string exceptionText = File.ReadAllText(file);
                    SentrySdk.CaptureMessage(exceptionText);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }

                try
                {
                    string newExtension = Path.ChangeExtension(file, ".log");
                    File.Move(file, newExtension);
                }
                catch (Exception)
                {

                }
            }
        }
        public static void CaptureMessage(string msg)
        {
            SentrySdk.CaptureMessage(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception">The exception to be captured</param>
        /// <param name="handled"></param>
        public static void CaptureException(Exception exception, bool handled)
        {
            if (handled)
            {
                exception.AddSentryTag("EXCEPTION.HANDLED", "1");
            }
            else
            {
                exception.AddSentryTag("EXCEPTION.HANDLED", "0");
            }

            /*if (!Debugger.IsAttached)
            {
                SentrySdk.CaptureException(exception);
            }*/
            SentrySdk.CaptureException(exception);
        }

        private static void WriteStringToFile(string filename, string contents)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                File.WriteAllText(filename, contents);
            }
            catch (Exception)
            {

            }
        }
    }
}

