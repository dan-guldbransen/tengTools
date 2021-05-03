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
        public static string ClientId = ConfigurationManager.AppSettings.Get("IntegrationAccount");
        public static string ClientSecret = ConfigurationManager.AppSettings.Get("consid12345");

        public class API
        {
            public const string GetToken = "/litium/oauth/token";

            public const string GetAssortments = "/Litium/api/admin/products/assortments";
            public static string GetCategoriesByAssortment(string assortmentSystemId) => $"/Litium/api/admin/products/assortments/{assortmentSystemId}/categories";
            public static string GetCategoryById(string id) => $"/Litium/api/admin/products/categories/{id}";

            public static string PutCategory(string systemId) => $"/Litium/api/admin/products/categories/{systemId}";

            public const string PostCategory = "/Litium/api/admin/products/categories";

        }
    }
}
