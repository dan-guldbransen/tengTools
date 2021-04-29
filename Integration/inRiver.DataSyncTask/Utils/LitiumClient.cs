using inRiver.DataSyncTask.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Utils
{
    public static class LitiumClient
    {
        public static HttpClient GetAuthorizedClient()
        {
            var jwtToken = GetJwtToken();
            var client = GetBaseClient();
            
            // Request headers for auth
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static string GetJwtToken()
        {
            var retval = string.Empty;
            using (var client = GetBaseClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/litium/oauth/token");

                var keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                keyValues.Add(new KeyValuePair<string, string>("client_id", "serviceID"));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", "servicepw"));

                request.Content = new FormUrlEncodedContent(keyValues);
                var response = client.SendAsync(request).Result;

                var data = JsonConvert.DeserializeObject<JWTResponse>(response.Content.ReadAsStringAsync().Result);
                retval = data.AccessToken;
            }
            return retval;
        }

        private static HttpClient GetBaseClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri("http://localhost:51134")
            };
        }
    }
}
