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
        public static void ProcessProducts(List<Entity> products, Data data, List<string> cultures, string assortmentId, List<string> existingCategorys, List<Models.Litium.Category> categories)
        {
            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.GetField(InRiver.InRiverField.Product.ProductId).Data?.ToString()))
                    continue;

                var multipleVariants = product.OutboundLinks.Where(l => l.LinkType.Id == InRiver.LinkType.ProductItem).Count() > 1;

                var litiumProduct = new Product(product, multipleVariants);
                litiumProduct.ProductÍtemIds = product.OutboundLinks.Where(l => l.LinkType.Id == InRiver.LinkType.ProductItem).Select(c => c.Target.Id).ToList();

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

                // Category
             
                var productCategoryNumbers = product.GetField("ProductCategoryNumber").Data as LocaleString;
                string productCategoryNumber;
                if (productCategoryNumbers != null)
                {
                     productCategoryNumber = productCategoryNumbers[productCategoryNumbers.Languages.First()];
                }
               


                // Other Fields
                foreach (var inRiverField in product.Fields)
                {

                }

                if (data.Products == null)
                    data.Products = new List<Product>();

                data.Products.Add(litiumProduct);

            }
        }
    }
}
