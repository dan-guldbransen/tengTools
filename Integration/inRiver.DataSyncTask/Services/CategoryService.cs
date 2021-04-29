using inRiver.DataSyncTask.Models.Litium;
using inRiver.Remoting.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

                var existingCategoryIdsOutput = new List<string>();
                foreach (var id in existingCategoryIds)
                {
                    existingCategoryIdsOutput.Add(id);
                }

                foreach (var idString in existingCategoryIds)
                {
                    var responseMessage2 = client.GetAsync($"/Litium/api/admin/products/categories/{idString}/categories").Result;
                    var existingCategoryIds2 = JsonConvert.DeserializeObject<List<string>>(responseMessage2.Content.ReadAsStringAsync().Result);

                    foreach (var id in existingCategoryIds2)
                    {
                        existingCategoryIdsOutput.Add(id);
                    }
                }
                return (assortmentSystemId, existingCategoryIdsOutput); 
            }
        }
       

        public static string GetFieldTemplateId()
        {
            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                var response = client.GetAsync("/Litium/api/admin/products/fieldTemplates").Result;
                var fieldTemplates = JsonConvert.DeserializeObject<List<FieldTemplateEntity>>(response.Content.ReadAsStringAsync().Result);
                var fieldTemplate = fieldTemplates.Where(x => x.Id == "Category");
                var fieldTemplatesSystemId = fieldTemplate.FirstOrDefault().systemId;

                return fieldTemplatesSystemId;
            }
        }

        public static List<Models.Litium.Category> GetExistingCategories(List<string> systemIds)
        {
            var output = new List<Models.Litium.Category>();

            using (var client = Utils.LitiumClient.GetAuthorizedClient())
            {
                foreach (var systemId in systemIds)
                {
                    var response = client.GetAsync($"/Litium/api/admin/products/categories/{systemId}").Result;
                    var responseContent = response.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<Models.Litium.Category>(responseContent.Result);
                    output.Add(category);
                }
                return output.Distinct().ToList();
            }
        }

        public static List<string> GetLitiumCategoryIds(List<Models.Litium.Category> categories)
        {
            var output = new List<string>();
            foreach (var category in categories)
            {
                output.Add(category.Id);
            }
            return output;
        }

        public static List<Models.Litium.Category> ProcessCategoryCVLsProductCategories(List<CVLValue> productCategories, int level, List<Models.Litium.Category> retval, List<CVLValue> parentCategories = null)
        {
            var assortmentAndCategoriesResult = GetAssortmentIdAndExistingCategoryIds();
            var existingCategoriesIds = assortmentAndCategoriesResult.Item2;
            var existingCategories = GetExistingCategories(existingCategoriesIds);
            var assortment = assortmentAndCategoriesResult.Item1;
            var productCategoriesToExport = CreateCategoriesFromProductCategories(productCategories, level, existingCategories, assortment, parentCategories);
            var existingLitiumCategoryIds = GetLitiumCategoryIds(existingCategories);

            foreach (var category in productCategoriesToExport)
            {
                var exists = existingLitiumCategoryIds.Contains(category.Id);
                if (exists == false)
                {
                    LitiumCommonService.SaveCategories(category);
                }
                else
                {
                    var idInRiverCategory = productCategories.Find(c => c.Id.ToString() == category.Id);
                    category.SystemId = existingCategories.Where(x => x.Id == category.Id).Select(x => x.SystemId).FirstOrDefault();
                   
                    LitiumCommonService.UpdateCategories(category);
                }
                retval.Add(category);
            }
            return retval;
        }


        public static List<Models.Litium.Category> CreateCategoriesFromProductCategories(List<CVLValue> productCategories, int level, List<Models.Litium.Category> existingCategories, string assortment, List<CVLValue> parentCategories = null)
        {
            var retval = new List<Models.Litium.Category>();
            foreach (var category in productCategories)
            {
                var productCategory = new Models.Litium.Category(assortment, category.Id.ToString());
                string parent = "";
                var parentKey = "00000000-0000-0000-0000-000000000000";

                if (level > 2)
                {
                    parent = GetParentCategory(category, parentCategories);
                    if (parent != "" )
                    {
                        parentKey = parent;
                        productCategory.ParentLitiumId = parentKey;
                    }
                }
                else if(level == 2)
                {
                    productCategory.ParentLitiumId = GetParentCategory(category, parentCategories);
                }
                productCategory.ParentCategorySystemId = parentKey;
                productCategory.FieldTemplateSystemId = GetFieldTemplateId();
                productCategory.Fields = CreateFields(category); 
                retval.Add(productCategory);
            }
            return retval;
        }

        public static CategoryField CreateFields( CVLValue category)
        {
            var fields = new Dictionary<string, string>();
            var lang = category.Value as LocaleString;
            var cultures = LitiumCommonService.GetCulturesInUse();

            foreach (var culture in cultures)
            {
                var name = lang[new System.Globalization.CultureInfo(culture)];
                if (name == null || name == "")
                {
                    name = lang[new System.Globalization.CultureInfo("en")];
                }

                if (culture == "en-UK")
                {
                    fields.Add("en-GB", name);
                }
                else
                {
                    fields.Add(culture, name);
                }
            }
            var field = new CategoryField();
            field.Name = fields;

            return field;
        }


            public static string GetParentCategory(CVLValue category, List<CVLValue> parentCategories = null)
        {
            string retval = "";
            var existingCategorysIds = GetAssortmentIdAndExistingCategoryIds();

            foreach (var parentCategory2 in parentCategories)
            {
                if(category.ParentKey == parentCategory2.Key)
                {
                    foreach (var categoryId in existingCategorysIds.Item2)
                    {
                        using (var client = Utils.LitiumClient.GetAuthorizedClient())
                        {
                            var response = client.GetAsync($"/Litium/api/admin/products/categories/{categoryId}").Result;
                            var parentCategory = JsonConvert.DeserializeObject<CategoryEntity>(response.Content.ReadAsStringAsync().Result);
                            var id = parentCategory.SystemId;

                            if (parentCategory.Id == parentCategory2.Id)
                            {
                                retval = id;
                                return retval;
                            }
                        }
                    }
                }
            }
            return retval;
        }

       

     }
    public class CategoryEntity
    {
        public string SystemId { get; set; }
        public CategoryField Fields { get; set; }
        public int Id { get; set; }
    }
    public class FieldTemplateEntity
    {
        public string systemId { get; set; }
        public string Id { get; set; }
    }
}
