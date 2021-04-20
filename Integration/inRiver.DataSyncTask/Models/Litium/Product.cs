using inRiver.DataSyncTask.Constants;
using inRiver.Remoting.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.Litium
{
    public class Product
    {
        [JsonProperty(PropertyName = "articleNumber")]
        public string ArticleNumber { get; set; }

        [JsonProperty(PropertyName = "fieldTemplateId")]
        public string FieldTemplateId { get; set; }

        [JsonProperty(PropertyName = "taxClassId")]
        public string TaxClassId { get; set; }

        [JsonProperty(PropertyName = "fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<Field> Fields { get; set; } = new List<Field>();

        // Fields to keep track of variants and markets, not mapped to Json
        [JsonIgnore]
        public List<int> ProductÍtemIds { get; set; } = new List<int>();
        [JsonIgnore]
        public List<string> Markets { get; set; } = new List<string>();

        public Product(Entity entity, bool multipleVariants)
        {
            ArticleNumber = entity.GetField(InRiver.InRiverField.Product.ProductId).Data.ToString();
            FieldTemplateId = multipleVariants ? "ProductWithVariants" : "ProductWithOneVariant";
        }
    }
}
