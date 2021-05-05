using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.inRiver;
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
        public static (string, List<BaseModel>) GetAssortmentIdAndExistingCategoryIds()
        {
            using (var client = LitiumClient.GetAuthorizedClient())
            {
                var response = client.GetAsync("/Litium/api/admin/products/assortments").Result;
                var assortments = JsonConvert.DeserializeObject<List<BaseModel>>(response.Content.ReadAsStringAsync().Result);

                if(assortments == null || !assortments.Any())
                    throw new Exception("No assortment was found");

                var assortmentSystemId = assortments.First().SystemId;

                var responseMessage = client.GetAsync($"/Litium/api/admin/products/assortments/{assortmentSystemId}/categories").Result;
                var existingCategoryIds = JsonConvert.DeserializeObject<List<string>>(responseMessage.Content.ReadAsStringAsync().Result);

                var existingCategories = new List<BaseModel>();
                foreach (var id in existingCategoryIds)
                {
                    var categoryResponse = client.GetAsync($"/Litium/api/admin/products/categories/{id}").Result;
                    existingCategories.Add(JsonConvert.DeserializeObject<BaseModel>(categoryResponse.Content.ReadAsStringAsync().Result));
                }

                return (assortmentSystemId, existingCategories);
            }
        }

        public static void ProcessCategoryCVLs(CategoryStructure categoryStructure, List<CVLValue> productCategories, List<CVLValue> productGroups, List<string> cultures, string categoryTemplateId)
        {
            using (var client = LitiumClient.GetAuthorizedClient())
            {
                // Get assortmensystemid and existing categories
                (var assortmentSystemId, var existingCategories) = GetAssortmentIdAndExistingCategoryIds();

                foreach (var headCat in categoryStructure.HeadCategories)
                {
                    // get existing if update / else create
                    var existing = existingCategories.FirstOrDefault(x => x.Id == headCat.Id.ToString());

                    // create head category, systemid if existing
                    var litiumCategory = new Models.Litium.Category(
                       assortmentsystemId: assortmentSystemId,
                       fieldTemplateSystemId: categoryTemplateId,
                       id: headCat.Id.ToString(),
                       parentCategorySystemId: null,
                       systemId: existing?.SystemId);

                    // Extract the fields we want to sync // TODO : LOCALISED NAME
                    litiumCategory.Fields = ExtractHeadCategoryName(headCat.Name, cultures);

                    // Save to litium
                    var headCategoryEntity = CreateOrUpdateCategory(litiumCategory, client, existing);

                    // Product Categories - Subcategory level 1
                    if (headCat.Categories != null && headCat.Categories.Any())
                    {
                        foreach (var productCategory in headCat.Categories)
                        {
                            // get existing if update / else create
                            var existingProductCategory = existingCategories.FirstOrDefault(x => x.Id == productCategory.ProductCategoryNumber.ToString());

                            var litiumProductCategory = new Models.Litium.Category(
                               assortmentsystemId: assortmentSystemId,
                               fieldTemplateSystemId: categoryTemplateId,
                               id: productCategory.ProductCategoryNumber.ToString(),
                               parentCategorySystemId: headCategoryEntity?.SystemId,
                               systemId: existingProductCategory?.SystemId);

                            // TODO: GET LOCALISED NAME
                            litiumProductCategory.Fields = ExtractHeadCategoryName(productCategory.Name, cultures);

                            // Save to litium
                            var produtCategoryEntity = CreateOrUpdateCategory(litiumProductCategory, client, existingProductCategory);

                            if (productCategory.Categories != null && productCategory.Categories.Any())
                            {
                                foreach (var productGroup in productCategory.Categories)
                                {
                                    var existingProductGroup = existingCategories.FirstOrDefault(x => x.Id == productGroup.ProductGroupNumber.ToString());

                                    var litiumProductGroup = new Models.Litium.Category(
                                       assortmentsystemId: assortmentSystemId,
                                       fieldTemplateSystemId: categoryTemplateId,
                                       id: productGroup.ProductGroupNumber.ToString(),
                                       parentCategorySystemId: produtCategoryEntity?.SystemId,
                                       systemId: existingProductGroup?.SystemId);

                                    // TODO: GET LOCALISED NAME
                                    litiumProductGroup.Fields = ExtractHeadCategoryName(productGroup.Name, cultures);

                                    // Save to litium
                                    var produtGroupEntity = CreateOrUpdateCategory(litiumProductGroup, client, existingProductGroup);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static BaseModel CreateOrUpdateCategory(Models.Litium.Category litiumCategory,
            HttpClient client,
            BaseModel existing = null)
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

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<BaseModel>(response.Content.ReadAsStringAsync().Result);

            return new BaseModel();
        }

        private static CategoryField ExtractHeadCategoryName(string name, List<string> cultures)
        {
            var fields = new CategoryField();

            foreach (var culture in cultures)
            {
                fields.Name.Add(culture, name);
            }

            return fields;
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
