using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.Litium
{
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
