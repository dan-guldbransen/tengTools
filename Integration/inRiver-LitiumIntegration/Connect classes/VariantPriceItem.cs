﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class VariantPriceItem
    {
        public string articleNumber { get; set; }
        public string priceListId { get; set; }
        public int price { get; set; }
        public int minimumQuantity { get; set; }
    }
}
