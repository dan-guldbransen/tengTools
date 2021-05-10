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

                // check if product has multiple variants to set correct template
                var multipleVariants = product.OutboundLinks.Where(l => l.LinkType.Id == InRiver.LinkType.ProductItem).Count() > 1;

                // create litiumproduct with req data
                var litiumProduct = new Product(product, multipleVariants);
                litiumProduct.ProductÍtemIds = product.OutboundLinks.Where(l => l.LinkType.Id == InRiver.LinkType.ProductItem).Select(c => c.Target.Id).ToList();

                var markets = product.GetField(InRiver.InRiverField.Product.ProductMarket).Data.ToString();
                var litiumMarkets = MapToLitiumChannelCultures(markets);

                // Get all data from Litium entity and set on Litium product
                ExtractFieldData(product, litiumProduct, cultures, litiumMarkets);

                if (data.Products == null)
                    data.Products = new List<Product>();

                data.Products.Add(litiumProduct);

            }
        }

        private static void ExtractFieldData(Entity product, Product litiumProduct, List<string> cultures, List<string> litiumMarkets)
        {
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
                Value = string.Join(",", litiumMarkets)
            });
        }

        // TODO refactor to json for quickadd
        private static List<string> MapToLitiumChannelCultures(string markets)
        {
            var retval = new List<string>() { "International " };
            var marketList = markets.Split(';');
            foreach (var market in marketList)
            {
                switch (market.ToUpper())
                {
                    case "AU":
                        retval.Add("en-au");
                        break;
                    case "IE":
                        retval.Add("en-ie");
                        break;
                    case "GB":
                        retval.Add("en-gb");
                        break;
                    case "NO":
                        retval.Add("nb-no");
                        break;
                    case "PL":
                        retval.Add("pl-pl");
                        break;
                    case "SE":
                        retval.Add("sv-se");
                        break;
                    default:
                        break;
                }
            }

            return retval;
        }

    }
}
