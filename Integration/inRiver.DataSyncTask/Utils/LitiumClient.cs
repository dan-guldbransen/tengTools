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
                var request = new HttpRequestMessage(HttpMethod.Post, "/litium/oauth/token");

                var keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                keyValues.Add(new KeyValuePair<string, string>("client_id", "IntegrationAccount"));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", "consid12345"));

                //keyValues.Add(new KeyValuePair<string, string>("client_id", "integration-user"));
                //keyValues.Add(new KeyValuePair<string, string>("client_secret", "a7a17390aa5348588c92f0bd55d705e6617fa3419dab4340a0c8ff634082e50c"));

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
                BaseAddress = new Uri("http://localhost:56020") 
                //BaseAddress = new Uri("http://tengtools.test.workplace.nu/")
            };
        }
    }
}
