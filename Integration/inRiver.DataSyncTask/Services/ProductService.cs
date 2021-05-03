using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.Litium;
using inRiver.Remoting.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Services
{
    public static class ProductService
    {
        public static void ProcessProducts(List<Entity> products, Data data, List<string> cultures)
        {
            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.GetField(InRiver.InRiverField.Product.ProductId).Data?.ToString()))
                    continue;

                var plattforms = product.GetField(InRiver.InRiverField.Product.ProductPublicPlatforms)?.Data.ToString();
                if(!string.IsNullOrEmpty(plattforms) && !plattforms.ToLower().Contains("web"))
                    continue;

                var multipleVariants = product.OutboundLinks.Where(l => l.LinkType.Id == InRiver.LinkType.ProductItem).Count() > 1;

                var litiumProduct = new Product(product, multipleVariants);
                litiumProduct.ProductÍtemIds = product.OutboundLinks.Where(l => l.LinkType.Id == InRiver.LinkType.ProductItem).Select(c => c.Target.Id).ToList();

                var markets = product.GetField(InRiver.InRiverField.Product.ProductMarket).Data.ToString();
                litiumProduct.Markets = MapToLitiumChannels(markets);
                
                // Name
                foreach (var culture in cultures)
                {
                    var description = product.GetField(InRiver.InRiverField.Product.ProductShortDescription).Data as LocaleString;
                    litiumProduct.Fields.Add(new Models.Litium.Field
                    {
                        FieldDefinitionId = LitiumFieldDefinitions.Name,
                        Culture = culture,
                        Value = description[new System.Globalization.CultureInfo(culture)] ?? description[description.Languages.First()]
                    });
                }

                // Category level 2
                var productCategoryNumber = product.GetField(InRiver.InRiverField.Product.ProductCategoryNumber).Data.ToString();
                litiumProduct.Fields.Add(new Models.Litium.Field
                {
                    FieldDefinitionId = "ProductCategoryNumber",
                    Culture = null,
                    Value = productCategoryNumber
                });

                // Category level 3
                var productGroupNumber = product.GetField(InRiver.InRiverField.Product.ProductGroupNumber).Data.ToString();
                litiumProduct.Fields.Add(new Models.Litium.Field
                {
                    FieldDefinitionId = "ProductGroupNumber",
                    Culture = null,
                    Value = productGroupNumber
                });

                // Markets -> mapped to channels in the create/update event
                litiumProduct.Fields.Add(new Models.Litium.Field
                {
                    FieldDefinitionId = "ProductMarket",
                    Culture = null,
                    Value = string.Join(",", litiumProduct.Markets)
                });

                if (data.Products == null)
                    data.Products = new List<Product>();

                data.Products.Add(litiumProduct);

            }
        }

        private static List<string> MapToLitiumChannels(string markets)
        {
            var retval = new List<string>() { "International "};
            var marketList = markets.Split(';');
            foreach(var market in marketList)
            {
                switch (market.ToUpper())
                {
                    case "AU":
                        retval.Add("Australia");
                        break;
                    case "IE":
                        retval.Add("Ireland");
                        break;
                    case "GB":
                        retval.Add("UK");
                        break;
                    case "NO":
                        retval.Add("Norway");
                        break;
                    case "PL":
                        retval.Add("Poland");
                        break;
                    case "SE":
                        retval.Add("Sweden");
                        break;
                    default:
                        break;
                }
            }

            return retval;
        }
       
    }
}
