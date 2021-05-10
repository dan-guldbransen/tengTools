using System.Collections.Generic;

namespace inRiver.DataSyncTask.Utils
{
    public class FieldMapper
    {
        public List<PimLitiumFieldMap> ProductFields { get; set; }
        public List<PimLitiumFieldMap> VariantFields { get; set; }
    }

    public class PimLitiumFieldMap
    {
        public string InRiverFieldId { get; set; }
        public string LitiumFieldId { get; set; }
        public bool IsMultiLingual { get; set; }
    }
}