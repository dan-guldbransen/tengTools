using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.Litium
{
    public class ProductSettings
    {
        [JsonProperty(PropertyName = "createUrls")]
        public bool CreateUrls { get; set; } = true;
    }
}
