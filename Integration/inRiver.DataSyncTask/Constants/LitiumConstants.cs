using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Constants
{
    public class LitiumConstants
    {
        public static string BaseUrl = ConfigurationManager.AppSettings.Get("LitiumBaseUrl");

        public const string GrantType = "client_credentials";
        public static string ClientId = ConfigurationManager.AppSettings.Get("client_id");
        public static string ClientSecret = ConfigurationManager.AppSettings.Get("client_secret");

    }
}
