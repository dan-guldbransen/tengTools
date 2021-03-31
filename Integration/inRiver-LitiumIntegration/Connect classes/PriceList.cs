using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class PriceList
    {
        public string id { get; set; }
        public bool active { get; set; }
        public string name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string currency { get; set; }
        public int priority { get; set; }
        public List<Item> items { get; set; }
        public bool includeVat { get; set; }
    }
}
