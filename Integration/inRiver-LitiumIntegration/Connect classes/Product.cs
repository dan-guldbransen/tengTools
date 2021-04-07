using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class Product
    {
        public string articleNumber { get; set; }
        public string fieldTemplateId { get; set; }
        public string taxClassId { get; set; }
        public List<Field> fields { get; set; } = new List<Field>();
    }
}
