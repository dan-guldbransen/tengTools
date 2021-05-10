using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.LitiumEntities
{
    public class ImportReportId
    {
        [JsonProperty("ImportReportId")]
        public string ImportReportIdValue { get; set; }
    }
}
