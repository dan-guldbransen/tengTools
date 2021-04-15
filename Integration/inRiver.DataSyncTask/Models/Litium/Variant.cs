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

        public Variant(Entity entity, string productArticleNumber)
        {
            ArticleNumber = entity.GetField(InRiver.InRiverField.Item.ItemId).Data.ToString();
            ProductArticleNumber = productArticleNumber;
            SortIndex = 0; // this exist on inRiver item
        }
    }
}
