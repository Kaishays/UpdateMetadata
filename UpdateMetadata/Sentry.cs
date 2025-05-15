using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata
{
    public static class Sentry
    {
        public static void CaptureMessage(string message, SentryLevel level = SentryLevel.Info)
        {
            SentrySdk.CaptureMessage(message, level);
        }

        public static void CaptureException(Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }

        public static void ConfigureScope(Action<Scope> configureScope)
        {
            SentrySdk.ConfigureScope(configureScope);
        }

        public static void AddBreadcrumb(string message, string category = null, string type = null, IDictionary<string, string> data = null, SentryLevel level = SentryLevel.Info)
        {
            SentrySdk.AddBreadcrumb(message, category, type, data, (BreadcrumbLevel)level);
        }

        public static void SetTag(string key, string value)
        {
            SentrySdk.ConfigureScope(scope => scope.SetTag(key, value));
        }

        public static void SetUser(string id, string username = null, string email = null, string ipAddress = null)
        {
            SentrySdk.ConfigureScope(scope => 
            {
                scope.User = new SentryUser
                {
                    Id = id,
                    Username = username,
                    Email = email,
                    IpAddress = ipAddress
                };
            });
        }
    }
}
