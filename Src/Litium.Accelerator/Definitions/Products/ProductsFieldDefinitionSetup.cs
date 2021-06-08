using System.Collections.Generic;
using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Products;

namespace Litium.Accelerator.Definitions.Products
{
    internal class ProductsFieldDefinitionSetup : FieldDefinitionSetup
    {
        public override IEnumerable<FieldDefinition> GetFieldDefinitions()
        {
            var fields = new List<FieldDefinition>
            {
                
                #region Litium Defaults - Removed some. Keep any??
                new FieldDefinition<ProductArea>("ProductSheet", SystemFieldTypeConstants.MediaPointerFile)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                },
                new FieldDefinition<ProductArea>("Specification", SystemFieldTypeConstants.MultirowText)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                },
                new FieldDefinition<ProductArea>("AcceleratorFilterFields", "FilterFields")
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                },
                #endregion


                new FieldDefinition<ProductArea>("HideLeftColumn", SystemFieldTypeConstants.Boolean )
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },

                #region Shared fields product and variant req by inRiver
                #endregion

                #region ProductFields req by mapping inRiver 
                  new FieldDefinition<ProductArea>("ProductMarket", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                // Sets number for product category level 2
                 new FieldDefinition<ProductArea>("ProductCategoryNumber", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                },
                 // Sets number for product category level 3
                new FieldDefinition<ProductArea>("ProductGroupNumber", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                new FieldDefinition<ProductArea>("ProductLongDescription", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductShortHeading", SystemFieldTypeConstants.Text)
                {
                   CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductLongHeading", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductBullet1", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductBullet2", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductBullet3", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductBullet4", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductBullet5", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ProductMaterial", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                #endregion

                #region VariantFields req by mapping inRiver
                // Non visible field, saves markets where variant should be visible
                new FieldDefinition<ProductArea>("ItemApprovedForMarket", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                 new FieldDefinition<ProductArea>("ItemWorkingDescription", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                  new FieldDefinition<ProductArea>("ItemShortDescription", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<ProductArea>("ItemLongDescription", SystemFieldTypeConstants.Text)
                {
                  CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },// HOW TO HANDEL STATUS TODO: have check in Litium and/or flag not sure, just sync for now
                new FieldDefinition<ProductArea>("ItemStatus", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                #endregion

                // G O O G L E  S H O P P I N G  F I E L D S LITIUM STANDARD
                new FieldDefinition<ProductArea>("AgeGroup", SystemFieldTypeConstants.TextOption)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    Option = new TextOption
                    {
                        MultiSelect = false,
                        Items = new List<TextOption.Item>
                        {
                            new TextOption.Item
                            {
                                Value = "Newborn",
                                Name = new Dictionary<string, string> { { "en-US", "Newborn" }, { "sv-SE", "Nyfödd" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Infant",
                                Name = new Dictionary<string, string> { { "en-US", "Infant" }, { "sv-SE", "Spädbarn" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Toddler",
                                Name = new Dictionary<string, string> { { "en-US", "Toddler" }, { "sv-SE", "Småbarn" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Kids",
                                Name = new Dictionary<string, string> { { "en-US", "Kids" }, { "sv-SE", "Ungar" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Adult",
                                Name = new Dictionary<string, string> { { "en-US", "Adult" }, { "sv-SE", "Vuxen" } }
                            }
                        }
                    }
                },
                new FieldDefinition<ProductArea>("Adult", SystemFieldTypeConstants.Boolean)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                },
                new FieldDefinition<ProductArea>("Condition", SystemFieldTypeConstants.TextOption)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                    Option = new TextOption
                    {
                        MultiSelect = false,
                        Items = new List<TextOption.Item>
                        {
                            new TextOption.Item
                            {
                                Value = "New",
                                Name = new Dictionary<string, string> { { "en-US", "New" }, { "sv-SE", "Ny" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Refurbished",
                                Name = new Dictionary<string, string> { { "en-US", "Refurbished" }, { "sv-SE", "Restaurerad" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Used",
                                Name = new Dictionary<string, string> { { "en-US", "Used" }, { "sv-SE", "Begagnad" } }
                            }
                        }
                    }
                },
                new FieldDefinition<ProductArea>("Gender", SystemFieldTypeConstants.TextOption)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    Option = new TextOption
                    {
                        MultiSelect = false,
                        Items = new List<TextOption.Item>
                        {
                            new TextOption.Item
                            {
                                Value = "Male",
                                Name = new Dictionary<string, string> { { "en-US", "Male" }, { "sv-SE", "Herr" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Female",
                                Name = new Dictionary<string, string> { { "en-US", "Female" }, { "sv-SE", "Dam" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Unisex",
                                Name = new Dictionary<string, string> { { "en-US", "Unisex" }, { "sv-SE", "Unisex" } }
                            }
                        }
                    }
                },
                new FieldDefinition<ProductArea>("GTIN", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                },
                new FieldDefinition<ProductArea>("IsBundle", SystemFieldTypeConstants.Boolean)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                },
                new FieldDefinition<ProductArea>("MPN", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                },
                new FieldDefinition<ProductArea>("GoogleProductCategory", SystemFieldTypeConstants.TextOption)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                    Option = new TextOption
                    {
                        MultiSelect = false
                    }
                },
                new FieldDefinition<ProductArea>(ProductFieldNameConstants.OrganizationsPointer, SystemFieldTypeConstants.Pointer)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new PointerOption
                    {
                        EntityType = PointerTypeConstants.CustomersOrganization,
                        MultiSelect = true
                    }
                },
            };

            return fields;
        }
    }
}
