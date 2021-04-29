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
        [JsonProperty(PropertyName = "systemId")]
        public string SystemId { get; set; }

        [JsonProperty(PropertyName = "assortmentSystemId")]
        public string AssortmentSystemId { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public CategoryField Fields { get; set; } = new CategoryField();

        [JsonProperty(PropertyName = "fieldTemplateSystemId")]
        public string FieldTemplateSystemId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "parentCategorySystemId")]
        public string ParentCategorySystemId { get; set; }

        [JsonProperty(PropertyName = "productLinks")]
        public List<ProductLink> ProductLinks { get; set; } = new List<ProductLink>();

        [JsonProperty(PropertyName = "parentLitiumId")]
        public string ParentLitiumId { get; set; }

        public Category(string assortmentsystemId, string id, string parentCategorySystemId = null)
        {
           // FieldTemplateSystemId = "Category";
            FieldTemplateSystemId = "54F43468-43B8-4D6A-8E40-A64929C2D418";
            AssortmentSystemId = assortmentsystemId;
            Id = id;
            ParentCategorySystemId = parentCategorySystemId;
        }
    }

    public class CategoryField
    {
        [JsonProperty(PropertyName = "_name")]
        public Dictionary<string, string> Name { get; set; }

        [JsonProperty(PropertyName = "_description", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Description { get; set; }
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
