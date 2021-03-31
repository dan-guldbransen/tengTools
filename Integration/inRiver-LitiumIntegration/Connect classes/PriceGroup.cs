using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class PriceGroup
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<string> priceListIds { get; set; }
    }
}
