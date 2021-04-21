using inRiver.DataSyncTask.Models.Litium;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Services
{
    public static class LitiumCommonService
    {
        public static List<string> GetCulturesInUse()
        {
            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                var cultures = new List<string>();
                var response = client.GetAsync("/Litium/api/admin/globalization/channels").Result;
                var channels = JsonConvert.DeserializeObject<List<JObject>>(response.Content.ReadAsStringAsync().Result);
                if(channels != null && channels.Any())
                {
                    var firstChannel = channels.First();
                    var nameFieldValues = firstChannel.SelectTokens("fields").Children().First().First().Children();
                    foreach(var field in nameFieldValues)
                    {
                        cultures.Add(((JProperty)field).Name);
                    }
                }
                return cultures;
            }
        }

        public static void SaveData(Data data)
        {
            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(
                  json,
                  Encoding.UTF8,
                  "application/json"
                  );

                var result = client.PostAsync("litium/api/connect/erp/imports", content).Result;

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
