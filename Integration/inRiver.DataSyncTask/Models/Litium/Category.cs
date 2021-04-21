using inRiver.Remoting.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.Litium
{
    public class Category
    {
        [JsonProperty(PropertyName = "assortmentSystemId")]
        public string AssortmentSystemId { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public List<CategoryField> Fields { get; set; } = new List<CategoryField>();

        [JsonProperty(PropertyName = "fieldTemplateSystemId")]
        public string FieldTemplateSystemId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "parentCategorySystemId")]
        public string ParentCategorySystemId { get; set; }

        [JsonProperty(PropertyName = "productLinks")]
        public List<ProductLink> ProductLinks { get; set; } = new List<ProductLink>();

        public Category(string assortmentsystemId, string id, string parentCategorySystemId = null)
        {
            FieldTemplateSystemId = "Category";
            AssortmentSystemId = assortmentsystemId;
            Id = id;
            ParentCategorySystemId = parentCategorySystemId;
        }
    }

    public class CategoryField
    {
        [JsonProperty(PropertyName = "_name")]
        public object Name { get; set; }

        [JsonProperty(PropertyName = "_description", NullValueHandling = NullValueHandling.Ignore)]
        public object Description { get; set; }
    }

    public class ProductLink
    {
        [JsonProperty(PropertyName = "activeVariantSystemIds")]
        public List<string> ActiveVariantSystemIds { get; set; }

        [JsonProperty(PropertyName = "baseProductSystemId")]
        public string BaseProductSystemId { get; set; }

        [JsonProperty(PropertyName = "mainCategory")]
        public bool MainCategory { get; set; }
    }
}
