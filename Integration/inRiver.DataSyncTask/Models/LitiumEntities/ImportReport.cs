using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.LitiumEntities
{
    public class ImportReport
    {
        public string SystemId { get; set; }
        public string UserAgent { get; set; }
        public string Status { get; set; }

        public List<ImportRecord> ImportRecords { get; set; }
    }

    public class ImportRecord
    {
        public string ImportEntityId { get; set; }
        public string ImportEntityType { get; set; }
        public string Status { get; set; }
        public string Operation { get; set; }
    }
}
