using inRiver.Remoting.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.Litium
{
    public class Category
    {
        [JsonProperty(PropertyName = "systemId", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemId { get; set; }

        [JsonProperty(PropertyName = "assortmentSystemId", NullValueHandling = NullValueHandling.Ignore)]
        public string AssortmentSystemId { get; set; }

        [JsonProperty(PropertyName = "fields", NullValueHandling = NullValueHandling.Ignore)]
        public CategoryField Fields { get; set; } = new CategoryField();

        [JsonProperty(PropertyName = "fieldTemplateSystemId", NullValueHandling = NullValueHandling.Ignore)]
        public string FieldTemplateSystemId { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "parentCategorySystemId", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentCategorySystemId { get; set; }

        [JsonProperty(PropertyName = "channelLinks", NullValueHandling = NullValueHandling.Ignore)]
        public List<ChannelLink> ChannelLinks { get; set; }

        //[JsonProperty(PropertyName = "productLinks")]
        //public List<ProductLink> ProductLinks { get; set; } = new List<ProductLink>();

        public Category(string assortmentsystemId, string id = null, string parentCategorySystemId = null, string systemId = null)
        {
            FieldTemplateSystemId = "793a09cb-aaec-437d-a2eb-7e23ff9cee31";
            AssortmentSystemId = assortmentsystemId;
            Id = id;
            ParentCategorySystemId = parentCategorySystemId;
            SystemId = systemId;
        }

        public Category() { }
    }

    public class ChannelLink
    {
        [JsonProperty(PropertyName = "channelSystemId")]
        public string ChannelSystemId { get; set; }
    }

    public class CategoryField
    {
        [JsonProperty(PropertyName = "_name")]
        public Dictionary<string, string> Name { get; set; } = new Dictionary<string, string>();
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
