using System;
using System.Linq;
using System.Collections.Generic;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Accelerator.Constants;

namespace Litium.Accelerator.Definitions.Products
{
    internal class ProductsFieldTemplateSetup : FieldTemplateSetup
    {
        private readonly DisplayTemplateService _displayTemplateService;

        public ProductsFieldTemplateSetup(DisplayTemplateService displayTemplateService)
        {
            _displayTemplateService = displayTemplateService;
        }
        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            var categoryDisplayTemplateId = _displayTemplateService.Get<CategoryDisplayTemplate>("Category")?.SystemId ?? Guid.Empty;
            var productDisplayTemplateId = _displayTemplateService.Get<ProductDisplayTemplate>("Product")?.SystemId ?? Guid.Empty;
            var productWithVariantListDisplayTemplateId = _displayTemplateService.Get<ProductDisplayTemplate>("ProductWithVariantList")?.SystemId ?? Guid.Empty;

            if (categoryDisplayTemplateId == Guid.Empty || productDisplayTemplateId == Guid.Empty || productWithVariantListDisplayTemplateId == Guid.Empty)
            {
                return Enumerable.Empty<FieldTemplate>();
            }

            var fieldTemplates = new FieldTemplate[]
            {
                new CategoryFieldTemplate("Category", categoryDisplayTemplateId)
                {
                    CategoryFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "_name",
                                "_description",
                                "_url",
                                "_seoTitle",
                                "_seoDescription",
                                "AcceleratorFilterFields",
                                ProductFieldNameConstants.OrganizationsPointer
                            }
                        }
                    }
                },
                new ProductFieldTemplate("ProductWithOneVariant", productDisplayTemplateId)
                {
                    ProductFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "_name",
                                ProductFieldNameConstants.OrganizationsPointer
                            }
                        }
                    },
                    VariantFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "_name",
                                "_description",
                                "_url",
                                "_seoTitle",
                                "_seoDescription"
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product information",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktinformation" },
                                ["en-US"] = { Name = "Product information" }
                            },
                            Fields =
                            {
                                "News",
                                "Brand",
                                "Color",
                                "Size",
                                "ProductSheet"
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product specification",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktspecifikation" },
                                ["en-US"] = { Name = "Product specification" }
                            },
                            Fields =
                            {
                                "Specification",
                                "Weight"
                            }
                        }
                    }
                },
                new ProductFieldTemplate("ProductWithVariants", productDisplayTemplateId)
                {
                    ProductFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "_name",
                                "_description",
                                ProductFieldNameConstants.OrganizationsPointer
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product information",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktinformation" },
                                ["en-US"] = { Name = "Product information" }
                            },
                            Fields =
                            {
                                "News",
                                "Brand",
                                "ProductSheet"
                            }
                        }
                    },
                    VariantFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "_name",
                                "_description",
                                "_url",
                                "_seoTitle",
                                "_seoDescription",
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product information",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktinformation" },
                                ["en-US"] = { Name = "Product information" }
                            },
                            Fields =
                            {
                                "Color",
                                "Size"
                            }
                        }
                    }
                },
                new ProductFieldTemplate("ProductWithVariantsList", productWithVariantListDisplayTemplateId)
                {
                    ProductFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "Brand",
                                "_name",
                                "_description",
                                "_url",
                                "_seoTitle",
                                "_seoDescription",
                                ProductFieldNameConstants.OrganizationsPointer
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "Product specification",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Produktspecifikation" },
                                ["en-US"] = { Name = "Product specification" }
                            },
                            Fields =
                            {
                                "Specification",
                                "ProductSheet"
                            }
                        }
                    },
                    VariantFieldGroups = new[]
                    {
                        new FieldTemplateFieldGroup
                        {
                            Id = "General",
                            Collapsed = false,
                            Localizations =
                            {
                                ["sv-SE"] = { Name = "Allmänt" },
                                ["en-US"] = { Name = "General" }
                            },
                            Fields =
                            {
                                "_name",
                                "Color",
                                "Size"
                            }
                        }
                    }
                }
            };
            return fieldTemplates;
        }
    }
}
