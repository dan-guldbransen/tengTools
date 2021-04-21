using inRiver.DataSyncTask.Models.Litium;
using inRiver.Remoting.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Services
{
    public static class CategoryService
    {
        public static (string, List<string>) GetAssortmentIdAndExistingCategoryIds()
        {
            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                var response = client.GetAsync("/Litium/api/admin/products/assortments").Result;
                var assortment = JObject.Parse(JsonConvert.DeserializeObject<List<object>>(response.Content.ReadAsStringAsync().Result).First().ToString());
                var assortmentSystemId = assortment.Value<string>("systemId");

                var responseMessage = client.GetAsync($"/Litium/api/admin/products/assortments/{assortmentSystemId}/categories").Result;
                var existingCategoryIds = JsonConvert.DeserializeObject<List<string>>(responseMessage.Content.ReadAsStringAsync().Result);

                return (assortmentSystemId, existingCategoryIds);
            }
        }

        public static List<Models.Litium.Category> ProcessCategoryCVLs(List<CVLValue> productCategories, List<CVLValue> productGroups)
        {
            var retval = new List<Models.Litium.Category>();

            return retval;
        }
    }
}
