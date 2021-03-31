using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class Inventory
    {
        public string inventoryId { get; set; }
        public List<Item> items { get; set; }
    }
}
