using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.Litium
{
    public class Data
    {
        [JsonProperty(PropertyName = "products", NullValueHandling = NullValueHandling.Ignore)]
        public List<Product> Products { get; set; }

        [JsonProperty(PropertyName = "variants", NullValueHandling = NullValueHandling.Ignore)]
        public List<Variant> Variants { get; set; }

        [JsonProperty(PropertyName = "productSettings", NullValueHandling = NullValueHandling.Ignore)]
        public ProductSettings ProductSettings { get; set; }
    }
}
