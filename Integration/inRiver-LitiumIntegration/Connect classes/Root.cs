using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class Root
    {
        public List<Inventory> inventories { get; set; }
        public List<InventoryItem> inventoryItems { get; set; }
        public List<Organization> organizations { get; set; }
        public OrganizationSettings organizationSettings { get; set; }
        public List<Person> persons { get; set; }
        public PersonSettings personSettings { get; set; }
        public List<PriceGroup> priceGroups { get; set; }
        public List<PriceList> priceList { get; set; }
        public List<Product> products { get; set; }
        public ProductSettings productSettings { get; set; }
        public List<VariantPriceItem> variantPriceItems { get; set; }
        public List<Variant> variants { get; set; }
        public VariantSettings variantSettings { get; set; }
        public string importBehavior { get; set; }
    }
}
