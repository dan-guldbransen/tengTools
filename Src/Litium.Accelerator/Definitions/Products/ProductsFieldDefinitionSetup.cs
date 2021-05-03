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
                new FieldDefinition<ProductArea>("Brand", SystemFieldTypeConstants.TextOption)
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
                                Value = "Adrian Hammond",
                                Name = new Dictionary<string, string> { { "en-US", "Adrian Hammond" }, { "sv-SE", "Adrian Hammond" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Dolce & Gabbana",
                                Name = new Dictionary<string, string> { { "en-US", "Dolce & Gabbana" }, { "sv-SE", "Dolce & Gabbana" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Le specs",
                                Name = new Dictionary<string, string> { { "en-US", "Le specs" }, { "sv-SE", "Le specs" } }
                            },
                            new TextOption.Item
                            {
                                Value = "MaQ",
                                Name = new Dictionary<string, string> { { "en-US", "MaQ" }, { "sv-SE", "MaQ" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Nolita",
                                Name = new Dictionary<string, string> { { "en-US", "Nolita" }, { "sv-SE", "Nolita" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Odd Molly",
                                Name = new Dictionary<string, string> { { "en-US", "Odd Molly" }, { "sv-SE", "Odd Molly" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Saint Tropez",
                                Name = new Dictionary<string, string> { { "en-US", "Saint Tropez" }, { "sv-SE", "Saint Tropez" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Vila",
                                Name = new Dictionary<string, string> { { "en-US", "Vila" }, { "sv-SE", "Vila" } }
                            },
                            new TextOption.Item
                            {
                                Value = "HÅG",
                                Name = new Dictionary<string, string> { { "en-US", "HÅG" }, { "sv-SE", "HÅG" } }
                            },
                            new TextOption.Item
                            {
                                Value = "RBM",
                                Name = new Dictionary<string, string> { { "en-US", "RBM" }, { "sv-SE", "RBM" } }
                            },
                            new TextOption.Item
                            {
                                Value = "RH",
                                Name = new Dictionary<string, string> { { "en-US", "RH" }, { "sv-SE", "RH" } }
                            }
                        }
                    }
                },
                new FieldDefinition<ProductArea>("Color", SystemFieldTypeConstants.TextOption)
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
                                Value = "White",
                                Name = new Dictionary<string, string> { { "en-US", "White" }, { "sv-SE", "Vit" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Beige",
                                Name = new Dictionary<string, string> { { "en-US", "Beige" }, { "sv-SE", "Beige" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Black",
                                Name = new Dictionary<string, string> { { "en-US", "Black" }, { "sv-SE", "Svart" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Grey",
                                Name = new Dictionary<string, string> { { "en-US", "Grey" }, { "sv-SE", "Grå" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Rose",
                                Name = new Dictionary<string, string> { { "en-US", "Rose" }, { "sv-SE", "Rosa" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Brown",
                                Name = new Dictionary<string, string> { { "en-US", "Brown" }, { "sv-SE", "Brun" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Red",
                                Name = new Dictionary<string, string> { { "en-US", "Red" }, { "sv-SE", "Röd" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Blue",
                                Name = new Dictionary<string, string> { { "en-US", "Blue" }, { "sv-SE", "Blå" } }
                            },
                            new TextOption.Item
                            {
                                Value = "Green",
                                Name = new Dictionary<string, string> { { "en-US", "Green" }, { "sv-SE", "Grön" } }
                            }
                        }
                    }
                },
                new FieldDefinition<ProductArea>("News", SystemFieldTypeConstants.Date)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = false,
                    MultiCulture = true,
                },
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
                new FieldDefinition<ProductArea>("Size", SystemFieldTypeConstants.TextOption)
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
                                Value = "S",
                                Name = new Dictionary<string, string> { { "en-US", "S" }, { "sv-SE", "S" } }
                            },
                            new TextOption.Item
                            {
                                Value = "One Size",
                                Name = new Dictionary<string, string> { { "en-US", "One Size" }, { "sv-SE", "One Size" } }
                            },
                            new TextOption.Item
                            {
                                Value = "M",
                                Name = new Dictionary<string, string> { { "en-US", "M" }, { "sv-SE", "M" } }
                            },
                            new TextOption.Item
                            {
                                Value = "L",
                                Name = new Dictionary<string, string> { { "en-US", "L" }, { "sv-SE", "L" } }
                            }
                        }
                    }
                },
                new FieldDefinition<ProductArea>("Weight", SystemFieldTypeConstants.Decimal)
                {
                    CanBeGridColumn = true,
                    CanBeGridFilter = true,
                    MultiCulture = false,
                },

                //new fields  
                // new FieldDefinition<ProductArea>("ItemWorkingDescription", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemId", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemShortDescription", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemLongDescription", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemStatus", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "10",
                //                Name = new Dictionary<string, string> { { "en-US", "Under development" }, { "sv-SE", "Under utveckling" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "20",
                //                Name = new Dictionary<string, string> { { "en-US", "Active" }, { "sv-SE", "Aktiv" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "30",
                //                Name = new Dictionary<string, string> { { "en-US", "Stopped" }, { "sv-SE", "Stoppad" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "40",
                //                Name = new Dictionary<string, string> { { "en-US", "To be discontinued" }, { "sv-SE", "Utgående" } }
                //            },
                //             new TextOption.Item
                //            {
                //                Value = "50",
                //                Name = new Dictionary<string, string> { { "en-US", "Discontinued" }, { "sv-SE", "Utgått" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemReplacementsItems", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemRegistrationDate", SystemFieldTypeConstants.DateTime )
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemToBeDiscontinuedDate", SystemFieldTypeConstants.DateTime )
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemDiscontinuedDate", SystemFieldTypeConstants.DateTime )
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemApprovedForMarket", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                // new FieldDefinition<ProductArea>("ItemOrderType", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "Stock",
                //                Name = new Dictionary<string, string> { { "en-US", "Stock" }, { "sv-SE", "Lager" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "OrderItem",
                //                Name = new Dictionary<string, string> { { "en-US", "Order item" }, { "sv-SE", "Order item" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "OnRequest",
                //                Name = new Dictionary<string, string> { { "en-US", "On request" }, { "sv-SE", "On request" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "Transit",
                //                Name = new Dictionary<string, string> { { "en-US", "Transit" }, { "sv-SE", "Transit" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemOperation", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "AU",
                //                Name = new Dictionary<string, string> { { "en-US", "AUS" }, { "sv-SE", "AUS" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "TW",
                //                Name = new Dictionary<string, string> { { "en-US", "TW" }, { "sv-SE", "TW" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "GB",
                //                Name = new Dictionary<string, string> { { "en-US", "UK" }, { "sv-SE", "UK" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemWarehouse", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "AU",
                //                Name = new Dictionary<string, string> { { "en-US", "AUS" }, { "sv-SE", "AUS" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "TW",
                //                Name = new Dictionary<string, string> { { "en-US", "TW" }, { "sv-SE", "TW" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "GB",
                //                Name = new Dictionary<string, string> { { "en-US", "UK" }, { "sv-SE", "UK" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemOnlyFromUKforEU", SystemFieldTypeConstants.Boolean)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemCountryOfOrigin", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //             new TextOption.Item
                //            {
                //                Value = "TW",
                //                Name = new Dictionary<string, string> { { "en-US", "TW" }, { "sv-SE", "TW" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "CN",
                //                Name = new Dictionary<string, string> { { "en-US", "CN" }, { "sv-SE", "CN" } }
                //            },

                //            new TextOption.Item
                //            {
                //                Value = "GB",
                //                Name = new Dictionary<string, string> { { "en-US", "UK" }, { "sv-SE", "UK" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemTarriffCodeEurope", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemUNSPSC", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemNSN", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemStandardType", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemCertificates", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "CE",
                //                Name = new Dictionary<string, string> { { "en-US", "CE" }, { "sv-SE", "CE" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "UKCA",
                //                Name = new Dictionary<string, string> { { "en-US", "UKCA" }, { "sv-SE", "UKCA" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "REACH",
                //                Name = new Dictionary<string, string> { { "en-US", "REACH" }, { "sv-SE", "REACH" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "RoHS",
                //                Name = new Dictionary<string, string> { { "en-US", "RoHS" }, { "sv-SE", "RoHS" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemElectricRecycling", SystemFieldTypeConstants.Boolean)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("Environmental_fee_Y/N", SystemFieldTypeConstants.Boolean)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemSellingMinQuantityTW", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemOrderMultipleTW", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemLunaArticleNumber", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemProductOwner", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "DALA",
                //                Name = new Dictionary<string, string> { { "en-US", "DALA" }, { "sv-SE", "DALA" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "KUCH",
                //                Name = new Dictionary<string, string> { { "en-US", "KUCH" }, { "sv-SE", "KUCH" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "MARO",
                //                Name = new Dictionary<string, string> { { "en-US", "MARO" }, { "sv-SE", "MARO" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "PHFA",
                //                Name = new Dictionary<string, string> { { "en-US", "PHFA" }, { "sv-SE", "PHFA" } }
                //            },
                //             new TextOption.Item
                //            {
                //                Value = "DAKI",
                //                Name = new Dictionary<string, string> { { "en-US", "DAKI" }, { "sv-SE", "DAKI" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "STST",
                //                Name = new Dictionary<string, string> { { "en-US", "STST" }, { "sv-SE", "STST" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "HACA",
                //                Name = new Dictionary<string, string> { { "en-US", "HACA" }, { "sv-SE", "HACA" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemBaseBarcode", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseQuantity", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseNumberOfPcsInUnit", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseNetWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseGrossWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseWeightUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "Kg",
                //                Name = new Dictionary<string, string> { { "en-US", "Kg" }, { "sv-SE", "Kg" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "g",
                //                Name = new Dictionary<string, string> { { "en-US", "g" }, { "sv-SE", "g" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemBaseLength", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseWidth", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseHeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseVolume", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemBaseVolumeUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "cuft",
                //                Name = new Dictionary<string, string> { { "en-US", "cuft" }, { "sv-SE", "cuft" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "dm3",
                //                Name = new Dictionary<string, string> { { "en-US", "dm3" }, { "sv-SE", "dm3" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1Barcode", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1Quantity", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1NetWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1GrossWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1Length", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1Width", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1Height", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1Volume", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1VolumeUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "cuft",
                //                Name = new Dictionary<string, string> { { "en-US", "cuft" }, { "sv-SE", "cuft" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "dm3",
                //                Name = new Dictionary<string, string> { { "en-US", "dm3" }, { "sv-SE", "dm3" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle2Barcode", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle2Quantity", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle2NetWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},

                //  // TODO
                // new FieldDefinition<ProductArea>("ItemBaseUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {

                //            new TextOption.Item
                //            {
                //                //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "Fyll i när den är godkänd för import i PIM",
                //                Name = new Dictionary<string, string> { { "en-US", "Fyll i när den är godkänd för import i PIM" }, { "sv-SE", "Fyll i när den är godkänd för import i PIM" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemBasePackageUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {

                //            new TextOption.Item
                //            {
                //                //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "Fyll i när den är godkänd för import i PIM",
                //                Name = new Dictionary<string, string> { { "en-US", "Fyll i när den är godkänd för import i PIM" }, { "sv-SE", "Fyll i när den är godkänd för import i PIM" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemBasePackageType", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {

                //            new TextOption.Item
                //            {
                //                //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "Fyll i när den är godkänd för import i PIM",
                //                Name = new Dictionary<string, string> { { "en-US", "Fyll i när den är godkänd för import i PIM" }, { "sv-SE", "Fyll i när den är godkänd för import i PIM" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle1PackageUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {

                //            new TextOption.Item
                //            {
                //                //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "Fyll i när den är godkänd för import i PIM",
                //                Name = new Dictionary<string, string> { { "en-US", "Fyll i när den är godkänd för import i PIM" }, { "sv-SE", "Fyll i när den är godkänd för import i PIM" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle2PackageUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {

                //            new TextOption.Item
                //            {
                //                //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "Fyll i när den är godkänd för import i PIM",
                //                Name = new Dictionary<string, string> { { "en-US", "Fyll i när den är godkänd för import i PIM" }, { "sv-SE", "Fyll i när den är godkänd för import i PIM" } }
                //            }
                //        }
                //    }
                //},
                // new FieldDefinition<ProductArea>("ItemMiddle2GrossWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2PackageUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            //TODO: Fyll i när den är godkänd för import i PIM
                //            new TextOption.Item
                //            {
                //                Value = "Fyll i när den är godkänd för import i PIM"
                //            }

                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2NetWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2GrossWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2WeightUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "kg",
                //                Name = new Dictionary<string, string>{ {"GB", "kg" }, {"US", "kg" } }
                //            }
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2Length", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2Width", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2Height", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2Volume", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2VolumeUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "cuft",
                //                Name = new Dictionary<string, string>{{"GB", "cuft"}, {"US", "cuft"}}
                //            }
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemTopBarcode", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopQuantity", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopPackageUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            //TODO: Fyll i när den är godkänd för import i PIM
                //            new TextOption.Item
                //            {
                //                Value = "Fyll i när den är godkänd för import i PIM"
                //            }

                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemTopNetWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopGrossWeight", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopWeightUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "kg",
                //                Name = new Dictionary<string, string>{ {"GB", "kg" }, {"US", "kg" } }
                //            }
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemTopLenght", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopWidth", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopHeigth", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopVolume", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemTopVolumeUnit", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "cuft",
                //                Name = new Dictionary<string, string>{{"GB", "cuft"}, {"US", "cuft"}}
                //            }
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemPackagingText", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemBasePlasticMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemBaseCartonMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemBaseMetalMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemBaseWoodenMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle1PlasticMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle1CartonMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle1MetalMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle1WoodenMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2PlasticMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2CartonMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2MetalMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMiddle2WoodenMaterial", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemPlasticMaterialTop", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemCartonMaterialTop", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemMetalMaterialTop", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemWoodenMaterialTop", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemSupplierNumber", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemProductionLeadTime", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemPurchasingMinimumOrderQuantity", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemProductPrice", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemProductPriceCurrency", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "NTD",
                //                Name = new Dictionary<string, string>{ {"GB", "NTD" }, {"US", "NTD" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "USD",
                //                Name = new Dictionary<string, string>{ {"GB", "USD" }, {"US", "USD" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "SEK",
                //                Name = new Dictionary<string, string>{ {"GB", "SEK" }, {"US", "SEK" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "EUR",
                //                Name = new Dictionary<string, string>{ {"GB", "EUR" }, {"US", "EUR" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "GBP",
                //                Name = new Dictionary<string, string>{ {"GB", "GBP" }, {"US", "GBP" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "CNY",
                //                Name = new Dictionary<string, string>{ {"GB", "CNY" }, {"US", "CNY" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemPackagingPrice", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemPackagingPurchasingCurrency", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "NTD",
                //                Name = new Dictionary<string, string>{ {"GB", "NTD" }, {"US", "NTD" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "USD",
                //                Name = new Dictionary<string, string>{ {"GB", "USD" }, {"US", "USD" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "SEK",
                //                Name = new Dictionary<string, string>{ {"GB", "SEK" }, {"US", "SEK" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "EUR",
                //                Name = new Dictionary<string, string>{ {"GB", "EUR" }, {"US", "EUR" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "GBP",
                //                Name = new Dictionary<string, string>{ {"GB", "GBP" }, {"US", "GBP" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "CNY",
                //                Name = new Dictionary<string, string>{ {"GB", "CNY" }, {"US", "CNY" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemCompleteProductPrice", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemCompleteProductPurchasingCurrency", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "NTD",
                //                Name = new Dictionary<string, string>{ {"GB", "NTD" }, {"US", "NTD" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "USD",
                //                Name = new Dictionary<string, string>{ {"GB", "USD" }, {"US", "USD" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "SEK",
                //                Name = new Dictionary<string, string>{ {"GB", "SEK" }, {"US", "SEK" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "EUR",
                //                Name = new Dictionary<string, string>{ {"GB", "EUR" }, {"US", "EUR" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "GBP",
                //                Name = new Dictionary<string, string>{ {"GB", "GBP" }, {"US", "GBP" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "CNY",
                //                Name = new Dictionary<string, string>{ {"GB", "CNY" }, {"US", "CNY" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemMarginPRBNet", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemPRBPriceUSD", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemBCPriceUSD", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ItemPriceList", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = true,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "StandardItems",
                //                Name = new Dictionary<string, string>{ {"GB", "Standard Items" }, {"US", "Standard Items" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "SpareParts",
                //                Name = new Dictionary<string, string>{ {"GB", "Spare parts" }, {"US", "Spare parts" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "TengCollection",
                //                Name = new Dictionary<string, string>{ {"GB", "Teng Collection" }, {"US", "Teng Collection" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "DisplayEquipment",
                //                Name = new Dictionary<string, string>{ {"GB", "Display Equipment" }, {"US", "Display Equipment" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "ToBePriced",
                //                Name = new Dictionary<string, string>{ {"GB", "To Be Priced" }, {"US", "To Be Priced" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ItemCompleteProductPriceUSD", SystemFieldTypeConstants.Decimal)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                
                //new FieldDefinition<ProductArea>("ProductId", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductItemType", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "StandardItem",
                //                Name = new Dictionary<string, string>{ {"GB", "Standard item" }, {"US", "Standard item" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "SparePart",
                //                Name = new Dictionary<string, string>{ {"GB", "Spare part" }, {"US", "Spare part" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "MarketingMaterial",
                //                Name = new Dictionary<string, string>{ {"GB", "Marketing material" }, {"US", "Marketing material" } }
                //            },
                //            new TextOption.Item
                //            {
                //                Value = "BrandingProduct",
                //                Name = new Dictionary<string, string>{ {"GB", "Branding product" }, {"US", "Branding product" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ProductBrand", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                Value = "TengTools",
                //                Name = new Dictionary<string, string>{ {"GB", "Teng Tools" }, {"US", "Teng Tools" } }
                //            },
                //        }
                //    }
                //},
                new FieldDefinition<ProductArea>("ProductMarket", SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                //new FieldDefinition<ProductArea>("ProductShortDescription", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductLongDescription", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductShortHeading", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductLongHeading", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductBullet1", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductBullet2", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductBullet3", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductBullet4", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductBullet5", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductMaterial", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductKeywords", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductMetaTitle", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductMetaDescription", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ProductPriority", SystemFieldTypeConstants.Int)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},

                //Resources ska de vara productarea??
                //new FieldDefinition<ProductArea>("ResourceName", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ResourceFileName", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},
                //new FieldDefinition<ProductArea>("ResourceMimeType", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},

               // fil, vilken datatype??
                //new FieldDefinition<ProductArea>("ResourceField", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},

                // TODO  
                //new FieldDefinition<ProductArea>("ProductSymbols", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                 //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "",
                //                Name = new Dictionary<string, string>{ {"GB", "" }, {"US", "" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ProductBranchFocused", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                 //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "",
                //                Name = new Dictionary<string, string>{ {"GB", "" }, {"US", "" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ProductProfessionFocused", SystemFieldTypeConstants.TextOption)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,

                //    Option = new TextOption
                //    {
                //        MultiSelect = false,
                //        Items = new List<TextOption.Item>
                //        {
                //            new TextOption.Item
                //            {
                //                 //TODO: Fyll i när den är godkänd för import i PIM
                //                Value = "",
                //                Name = new Dictionary<string, string>{ {"GB", "" }, {"US", "" } }
                //            },
                //        }
                //    }
                //},
                //new FieldDefinition<ProductArea>("ProductCategoryNumber", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false,
                //},
                //new FieldDefinition<ProductArea>("ProductGroupNumber", SystemFieldTypeConstants.Text)
                //{
                //    CanBeGridColumn = true,
                //    CanBeGridFilter = true,
                //    MultiCulture = false
                //},

                // G O O G L E  S H O P P I N G  F I E L D S
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
