using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models
{
    public class ProductData
    {
        [JsonProperty(PropertyName = "products")]
        public List<Product> Products { get; set; } = new List<Product>();


        [JsonProperty(PropertyName = "variants")]
        public List<Variant> Variants { get; set; } = new List<Variant>();

        [JsonProperty(PropertyName = "productSettings")]
        public ProductSettings ProductSettings { get; set; } = new ProductSettings();
    }

    public class ProductSettings
    {
        [JsonProperty(PropertyName = "createUrls")]
        public bool CreateUrls { get; set; } = true;
    }

    public class Variant
    {
        [JsonProperty(PropertyName = "articleNumber")]
        public string ArticleNumber { get; set; }

        [JsonProperty(PropertyName = "productArticleNumber")]
        public string ProductArticleNumber { get; set; }

        [JsonProperty(PropertyName = "unitOfMeasurementId")]
        public string UnitOfMeasurementId { get; set; }

        [JsonProperty(PropertyName = "sortIndex")]
        public int SortIndex { get; set; }


        [JsonProperty(PropertyName = "fields")]
        public List<Field> Fields { get; set; } = new List<Field>();
    }

    public class Product
    {
        [JsonProperty(PropertyName = "articleNumber")]
        public string ArticleNumber { get; set; }

        [JsonProperty(PropertyName = "fieldTemplateId")]
        public string FieldTemplateId { get; set; }

        [JsonProperty(PropertyName = "taxClassId")]
        public string TaxClassId { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public List<Field> Fields { get; set; } = new List<Field>();
    }

    public class Field
    {
        [JsonProperty(PropertyName = "fieldDefinitionId")]
        public string FieldDefinitionId { get; set; }
        
        [JsonProperty(PropertyName = "culture")]
        public string Culture { get; set; }
        
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
    }
}
