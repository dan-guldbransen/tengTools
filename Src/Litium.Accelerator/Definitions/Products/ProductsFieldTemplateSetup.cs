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
                                "HideLeftColumn",
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
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "InRiver Information",
                            Collapsed = false,
                            Fields =
                            {
                                "ProductLongDescription",
                                "ProductShortHeading",
                                "ProductLongHeading",
                                "ProductBullet1",
                                "ProductBullet2",
                                "ProductBullet3",
                                "ProductBullet4",
                                "ProductBullet5",
                                "ProductMaterial"
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
                                "Specification"
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "InRiver Information",
                            Collapsed = false,
                            Fields =
                            {
                               "ItemWorkingDescription",
                               "ItemShortDescription",
                               "ItemLongDescription",
                               "ItemStatus"
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
                                "ProductSheet"
                            }
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "InRiver Information",
                            Collapsed = false,
                            Fields =
                            {
                                "ProductLongDescription",
                                "ProductShortHeading",
                                "ProductLongHeading",
                                "ProductBullet1",
                                "ProductBullet2",
                                "ProductBullet3",
                                "ProductBullet4",
                                "ProductBullet5",
                                "ProductMaterial"
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
                            Fields = {}
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "InRiver Information",
                            Collapsed = false,
                            Fields =
                            {
                               "ItemWorkingDescription",
                               "ItemShortDescription",
                               "ItemLongDescription",
                               "ItemStatus"
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
                        },
                        new FieldTemplateFieldGroup
                        {
                            Id = "InRiver Information",
                            Collapsed = false,
                            Fields =
                            {
                                "ProductLongDescription",
                                "ProductShortHeading",
                                "ProductLongHeading",
                                "ProductBullet1",
                                "ProductBullet2",
                                "ProductBullet3",
                                "ProductBullet4",
                                "ProductBullet5",
                                "ProductMaterial"
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
                                "_name"
                            }
                        }
                    }
                }
            };
            return fieldTemplates;
        }
    }
}
