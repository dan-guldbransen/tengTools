using Litium.Websites;
using Litium.FieldFramework;
using System.Collections.Generic;
using Litium.Accelerator.Constants;
using Litium.FieldFramework.FieldTypes;

namespace Litium.Accelerator.Definitions.Websites
{
    internal class AcceleratorWebsiteFieldDefinitionSetup : FieldDefinitionSetup
    {
        public override IEnumerable<FieldDefinition> GetFieldDefinitions()
        {
            var fields = new[]
            {
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.LogotypeMain, SystemFieldTypeConstants.MediaPointerImage),
                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.LogotypeAlt, SystemFieldTypeConstants.MediaPointerImage),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.LogotypeIcon, SystemFieldTypeConstants.MediaPointerImage),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.CheckoutPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.MyPagesPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.AdditionalHeaderLinks, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = true }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.SearchResultPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterTopText, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterGetOrganised, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterNewsletter, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterSocialMediaText, SystemFieldTypeConstants.Text)
                {
                   MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterVisionHeader, SystemFieldTypeConstants.Text)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterVisionText, SystemFieldTypeConstants.Text)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterLegal, SystemFieldTypeConstants.Text)
                {
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterHeader, SystemFieldTypeConstants.LimitedText)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterLinkList, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = true }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FooterText, SystemFieldTypeConstants.Editor)
                {
                    MultiCulture = true,
                },
                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.Footer, SystemFieldTypeConstants.MultiField)
                {
                    Option = new MultiFieldOption { IsArray = true, Fields = new List<string>(){AcceleratorWebsiteFieldNameConstants.FooterHeader, AcceleratorWebsiteFieldNameConstants.FooterLinkList } }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.NavigationTheme, SystemFieldTypeConstants.TextOption)
                {
                    Option = new TextOption
                    {
                        MultiSelect = false,
                        Items = new List<TextOption.Item>
                        {
                            new TextOption.Item
                            {
                                Value = NavigationConstants.CategoryBased,
                                Name = new Dictionary<string, string> { { "en-US", "Category based" }, { "sv-SE", "Baserat på kategori" } }
                            },
                            new TextOption.Item
                            {
                                Value = NavigationConstants.FilterBased,
                                Name = new Dictionary<string, string> { { "en-US", "Filter based" }, { "sv-SE", "Baserat på filter" } }
                            }
                        }
                    }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.InFirstLevelCategories, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.InBrandPages, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.InProductListPages, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.InArticlePages, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.ProductsPerPage, SystemFieldTypeConstants.Int),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.ShowBuyButton, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.ShowQuantityFieldProductList, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.ShowQuantityFieldProductPage, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FiltersOrdering, FieldTypeConstants.Filters),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.FiltersIndexedBySearchEngines, FieldTypeConstants.Filters),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.CheckoutMode, SystemFieldTypeConstants.IntOption)
                {
                    Option = new IntOption
                    {
                        MultiSelect = false,
                        Items = new List<IntOption.Item>
                        {
                            new IntOption.Item
                            {
                                Value = (int)CheckoutMode.PrivateCustomers,
                                Name = new Dictionary<string, string> { { "en-US", "Private customers / B2C only" }, { "sv-SE", "Private customers / B2C only" } }
                            },
                            new IntOption.Item
                            {
                                Value = (int)CheckoutMode.CompanyCustomers,
                                Name = new Dictionary<string, string> { { "en-US", "Company customers / B2B only" }, { "sv-SE", "Företag/endast B2B" } }
                            },
                            new IntOption.Item
                            {
                                Value = (int)CheckoutMode.Both,
                                Name = new Dictionary<string, string> { { "en-US", "Both" }, { "sv-SE", "Båda" } }
                            }
                        }
                    }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.AllowCustomersEditLogin, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.SenderEmailAddress, SystemFieldTypeConstants.Text),
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.OrderConfirmationPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(CheckoutPageFieldNameConstants.TermsAndConditionsStatement, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                    Option = new TextOption()
                },
                new FieldDefinition<WebsiteArea>(CheckoutPageFieldNameConstants.TermsAndConditionsLinkText, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                    Option = new TextOption()
                },
                new FieldDefinition<WebsiteArea>(CheckoutPageFieldNameConstants.TermsAndConditionsPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.CookieNotificationHeader, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                    Option = new TextOption()
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.CookieNotificationText, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = true,
                    MultiCulture = true,
                    Option = new TextOption()
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.CookieNotificationPolicyPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.GetOrganisedPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.ExternalB2BLink, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true,
                    Option = new TextOption()
                },


                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerTitle, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                  new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerName, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                   new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerPhone, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                    new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerEmail, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                     new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerWebsite, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                      new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerStreetAddress, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                       new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerZipcode, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                        new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerCity, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                         new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerInstagram, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                          new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerFacebook, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                           new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerLinkedIn, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                            new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerTwitter, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                             new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerYoutube, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false
                },
                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerCountry, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.PartnerFlagIcon, SystemFieldTypeConstants.MediaPointerImage),
                   new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.Partners, SystemFieldTypeConstants.MultiField)
                {
                    Option = new MultiFieldOption { IsArray = true,
                    Fields = new List<string>()
                    {
                        AcceleratorWebsiteFieldNameConstants.PartnerCountry, 
                        AcceleratorWebsiteFieldNameConstants.PartnerFlagIcon,
                        AcceleratorWebsiteFieldNameConstants.PartnerPhone,
                        AcceleratorWebsiteFieldNameConstants.PartnerEmail,
                        AcceleratorWebsiteFieldNameConstants.PartnerWebsite,
                        AcceleratorWebsiteFieldNameConstants.PartnerName,
                        AcceleratorWebsiteFieldNameConstants.PartnerStreetAddress,
                        AcceleratorWebsiteFieldNameConstants.PartnerZipcode,
                        AcceleratorWebsiteFieldNameConstants.PartnerCity,
                        AcceleratorWebsiteFieldNameConstants.PartnerInstagram,
                        AcceleratorWebsiteFieldNameConstants.PartnerFacebook,
                        AcceleratorWebsiteFieldNameConstants.PartnerLinkedIn,
                        AcceleratorWebsiteFieldNameConstants.PartnerTwitter,
                        AcceleratorWebsiteFieldNameConstants.PartnerYoutube,
                    }
                 }
                },
                 new FieldDefinition<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.BlogTagList, SystemFieldTypeConstants.TextOption)
                {
                    MultiCulture = true,
                    Option = new TextOption()
                    {
                        MultiSelect = false
                    }
                },
            };
            return fields;
        }
    }
}
