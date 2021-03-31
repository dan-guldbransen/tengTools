using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class Variant
    {
        public string articleNumber { get; set; }
        public string productArticleNumber { get; set; }
        public string unitOfMeasurementId { get; set; }
        public int sortIndex { get; set; }
        public List<Field> fields { get; set; }
    }
}
