using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class ProductSettings
    {
        public bool createUrls { get; set; }
        public List<string> urlFieldDefinitionIds { get; set; }
        public string taxClassId { get; set; }
        public string fieldTemplateId { get; set; }
    }
}
