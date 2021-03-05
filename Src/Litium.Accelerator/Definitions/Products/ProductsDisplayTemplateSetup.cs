using System.Collections.Generic;
using Litium.Accelerator.Constants;
using Litium.Products;

namespace Litium.Accelerator.Definitions.Products
{
    internal class ProductsDisplayTemplateSetup : DisplayTemplateSetup
    {
        public override IEnumerable<DisplayTemplate> GetDisplayTemplates()
        {
            var displayTemplates = new DisplayTemplate[]
            {
                new CategoryDisplayTemplate
                {
                    Id = ProductTemplateNameConstants.Category,
                    Localizations =
                    {
                        ["sv-SE"] = { Name = "Kategori" },
                        ["en-US"] = { Name = "Category" }
                    }
                },
                new ProductDisplayTemplate
                {
                    Id = ProductTemplateNameConstants.Product,
                    Localizations =
                    {
                        ["sv-SE"] = { Name = "Produkt" },
                        ["en-US"] = { Name = "Product" }
                    },
                    UseVariantUrl = true
                },
                new ProductDisplayTemplate
                {
                    Id = ProductTemplateNameConstants.ProductWithVariantList,
                    Localizations =
                    {
                        ["sv-SE"] = { Name = "Produkt med variantlista" },
                        ["en-US"] = { Name = "Product with variant list" }
                    }
                }
            };
            return displayTemplates;
        }
    }
}
