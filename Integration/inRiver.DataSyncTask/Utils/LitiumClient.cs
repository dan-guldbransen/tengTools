using inRiver.DataSyncTask.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using inRiver.DataSyncTask.Constants;

namespace inRiver.DataSyncTask.Utils
{
    public static class LitiumClient
    {
        public static string _token = string.Empty;
        
        public static HttpClient GetAuthorizedClient()
        {
            if(string.IsNullOrEmpty(_token))
                _token = GetJwtToken();

            var client = GetBaseClient();
            
            // Request headers for auth
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static string GetJwtToken()
        {
            var retval = string.Empty;
            using (var client = GetBaseClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, LitiumConstants.API.GetToken);

                var keyValues = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", LitiumConstants.GrantType),
                    new KeyValuePair<string, string>("client_id", LitiumConstants.ClientId),
                    new KeyValuePair<string, string>("client_secret", LitiumConstants.ClientSecret)
                };

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
                BaseAddress = new Uri(LitiumConstants.BaseUrl) 
            };
        }
    }
}
