using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.LitiumEntities
{
    public class WebsiteEntity : BaseModel
    {
        [JsonProperty("fields")]
        public Field Fields { get; set; }
    }

    public class Field
    {
        [JsonProperty("_name")]
        public Dictionary<string, string> Name { get; set; }
    }
}
