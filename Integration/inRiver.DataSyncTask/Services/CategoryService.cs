using inRiver.DataSyncTask.Models.Litium;
using inRiver.DataSyncTask.Models.LitiumEntities;
using inRiver.DataSyncTask.Utils;
using inRiver.Remoting.Objects;
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
    public static class CategoryService
    {
        public static (string, List<CategoryEntity>) GetAssortmentIdAndExistingCategoryIds()
        {
            using (var client = LitiumClient.GetAuthorizedClient())
            {
                var response = client.GetAsync("/Litium/api/admin/products/assortments").Result;
                var assortment = JObject.Parse(JsonConvert.DeserializeObject<List<object>>(response.Content.ReadAsStringAsync().Result).First().ToString());
                var assortmentSystemId = assortment.Value<string>("systemId");

                var responseMessage = client.GetAsync($"/Litium/api/admin/products/assortments/{assortmentSystemId}/categories").Result;
                var existingCategoryIds = JsonConvert.DeserializeObject<List<string>>(responseMessage.Content.ReadAsStringAsync().Result);

                var existingCategories = new List<CategoryEntity>();
                foreach(var id in existingCategoryIds)
                {
                    var categoryResponse = client.GetAsync($"/Litium/api/admin/products/categories/{id}").Result;
                    existingCategories.Add(JsonConvert.DeserializeObject<CategoryEntity>(categoryResponse.Content.ReadAsStringAsync().Result));
                }

                return (assortmentSystemId, existingCategories);
            }
        }

        public static List<Models.Litium.Category> ProcessCategoryCVLs(List<CVLValue> productCategories, List<CVLValue> productGroups, string assortmentId, List<string> cultures, List<CategoryEntity> existingCategories)
        {
            var retval = new List<Models.Litium.Category>();
            using (var client = LitiumClient.GetAuthorizedClient())
            {
                // Category level 1
                foreach (var cat in productCategories)
                {
                    // get existing if update / else create
                    var existing = existingCategories.FirstOrDefault(x => x.Id == cat.Id.ToString());

                    // Create model level 1 category with existing Id if category is update, else null if new category
                    var litiumCategory = new Models.Litium.Category(
                        assortmentsystemId: assortmentId, 
                        id: cat.Index.ToString(), 
                        parentCategorySystemId: null, 
                        systemId: existing?.SystemId);

                    // Extract the fields we want to sync
                    litiumCategory.Fields = ExtractCategoryNameFields(cat, cultures);

                    // Save to litium
                    CreateOrUpdateCategory(litiumCategory, client, existing);

                    // save for later? will we if needed
                    retval.Add(litiumCategory);
                }

                // Update existing list if newly created level 1 categoryies
                (_, existingCategories) = GetAssortmentIdAndExistingCategoryIds();

                // Category level 2
                foreach (var cat in productGroups)
                {
                    // check if parent exists, must exist if level 2 category is to be processed
                    var parent = productCategories.FirstOrDefault(x => x.Key == cat.ParentKey);
                    if(parent == null)
                        continue;

                    var litiumParentCategory = existingCategories.FirstOrDefault(x => x.Id == parent.Id.ToString());
                    if(litiumParentCategory == null)
                        continue;

                    // get existing if update / else create
                    var existing = existingCategories.FirstOrDefault(x => x.Id == cat.Id.ToString());

                    // create model level 2 category with parent id
                    var litiumCategory = new Models.Litium.Category(
                        assortmentsystemId: assortmentId,
                        id: cat.Index.ToString(),
                        parentCategorySystemId: litiumParentCategory.SystemId,
                        systemId: existing?.SystemId);

                    // Extract the fields we want to sync
                    litiumCategory.Fields = ExtractCategoryNameFields(cat, cultures);

                    // Save to litium
                    CreateOrUpdateCategory(litiumCategory, client, existing);

                    // save for later? will we if needed
                    retval.Add(litiumCategory);
                }

            }
            return retval;
        }

        private static void CreateOrUpdateCategory(Models.Litium.Category litiumCategory,
            HttpClient client,
            CategoryEntity existing = null)
        {
            string json = JsonConvert.SerializeObject(litiumCategory);
            var content = new StringContent(
              json,
              Encoding.UTF8,
              "application/json"
              );

            // Post for new categories
            // Put for update (patch for single values)
            HttpResponseMessage response;
            if (existing != null)
            {
                response = client.PutAsync($"/Litium/api/admin/products/categories/{existing.SystemId}", content).Result;
            }
            else
            {
                response = client.PostAsync("/Litium/api/admin/products/categories", content).Result;
            }

            //TODO: Handle response with logging ??
        }

        private static CategoryField ExtractCategoryNameFields(CVLValue cateNames, List<string> cultures)
        {
            var value = cateNames.Value as LocaleString;
            var fields = new CategoryField();

            foreach (var culture in cultures)
            {
                fields.Name.Add(culture, value[new System.Globalization.CultureInfo("en")]);
            }

            return fields;
        }
    }
}
