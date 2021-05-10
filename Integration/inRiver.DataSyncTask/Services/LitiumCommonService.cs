using inRiver.DataSyncTask.Models.Litium;
using inRiver.DataSyncTask.Models.LitiumEntities;
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
                var response = client.GetAsync("/Litium/api/admin/globalization/languages").Result;
                var languages = JsonConvert.DeserializeObject<List<LanguageEntity>>(response.Content.ReadAsStringAsync().Result);

                if (languages != null && languages.Any())
                {
                    return languages.Select(l => l.Id).ToList();
                }

                return new List<string>();
            }
        }

        public static string GetCategoryTemplateSystemId()
        {
            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                var response = client.GetAsync("/Litium/api/admin/products/fieldTemplates").Result;
                var templates = JsonConvert.DeserializeObject<List<BaseModel>>(response.Content.ReadAsStringAsync().Result);

                if (templates != null && templates.Any())
                {
                    var categoryTemplate = templates.FirstOrDefault(x => x.Id == "Category");
                    if (categoryTemplate != null)
                        return categoryTemplate.SystemId;
                }

                throw new Exception("Cant find category template system id");
            }
        }

        public static string SaveData(Data data)
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

                return JsonConvert.DeserializeObject<ImportReportId>(result.Content.ReadAsStringAsync().Result).ImportReportIdValue;
            }
        }

        public static ImportReport GetImportReport(string importReportId)
        {
            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                var result = client.GetAsync($"litium/api/connect/erp/imports/{importReportId}").Result;

                return JsonConvert.DeserializeObject<ImportReport>(result.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
