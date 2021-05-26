using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Websites;
using System.Collections.Generic;

namespace Litium.Accelerator.Definitions.Pages
{
    internal class PageFieldDefinitionSetup : FieldDefinitionSetup
    {
        public override IEnumerable<FieldDefinition> GetFieldDefinitions()
        {
            var fields = new List<FieldDefinition>();

            fields.AddRange(GeneralFields());
            fields.AddRange(LoginPageFields());
            fields.AddRange(MegaMenuPageFields());

            return fields;
        }

        private IEnumerable<FieldDefinition> GeneralFields()
        {
            var fields = new List<FieldDefinition>
            {
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.Title, SystemFieldTypeConstants.Text)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.Introduction, SystemFieldTypeConstants.MultirowText)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.Text, SystemFieldTypeConstants.Editor)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.Links, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = true }
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.Image, SystemFieldTypeConstants.MediaPointerImage),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.Files, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.MediaFile, MultiSelect = true }
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.PageSize, SystemFieldTypeConstants.Int),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.TitleFilterSelector, SystemFieldTypeConstants.Text),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.ErrorMessage, SystemFieldTypeConstants.Editor)
                {
                    MultiCulture = true,
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.NumberOfNewsPerPage, SystemFieldTypeConstants.Int),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.FeaturedBlog, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.NewsDate, SystemFieldTypeConstants.DateTime),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.CategoryPointer, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.ProductsCategory, MultiSelect = false }
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.AlternativeImageDescription, SystemFieldTypeConstants.Text),
                new FieldDefinition<WebsiteArea>(WelcomeEmailPageFieldNameConstants.Subject, SystemFieldTypeConstants.Text)
                { 
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(WelcomeEmailPageFieldNameConstants.WelcomeEmailText, SystemFieldTypeConstants.Editor)
                { 
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.NumberOfOrdersPerPage, SystemFieldTypeConstants.Int),
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.OrderLink, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = false }
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.OrderHistoryLink, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = false }
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.ProductListPointer, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.ProductsProductList, MultiSelect = false }
                },
                new FieldDefinition<WebsiteArea>(PageFieldNameConstants.MayUserEditLogin, SystemFieldTypeConstants.Boolean)
            };
            return fields;
        }

        private IEnumerable<FieldDefinition> LoginPageFields()
        {
            var fields = new List<FieldDefinition>
            {
                new FieldDefinition<WebsiteArea>(LoginPageFieldNameConstants.ForgottenPasswordLink, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(LoginPageFieldNameConstants.RedirectLink, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = false }
                }
            };
            return fields;
        }

        private IEnumerable<FieldDefinition> MegaMenuPageFields()
        {
            var fields = new List<FieldDefinition<WebsiteArea>>
            {
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuCategory, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.ProductsCategory }
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuShowContent, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuShowSubCategories, SystemFieldTypeConstants.Boolean),
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuColumnHeader, SystemFieldTypeConstants.LimitedText)
                { 
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuCategories, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.ProductsCategory, MultiSelect = true }
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuPages, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage, MultiSelect = true }
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuFilters, FieldTypeConstants.Filters),
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuEditor, SystemFieldTypeConstants.Editor)
                 {
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuAdditionalLink, SystemFieldTypeConstants.LimitedText)
                {
                    MultiCulture = true
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuLinkToCategory, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.ProductsCategory }
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuLinkToPage, SystemFieldTypeConstants.Pointer)
                {
                    Option = new PointerOption { EntityType = PointerTypeConstants.WebsitesPage }
                },
                new FieldDefinition<WebsiteArea>(MegaMenuPageFieldNameConstants.MegaMenuColumn, SystemFieldTypeConstants.MultiField)
                {
                     Option = new MultiFieldOption { IsArray = true, Fields = new List<string>(){ MegaMenuPageFieldNameConstants.MegaMenuColumnHeader,
                        MegaMenuPageFieldNameConstants.MegaMenuCategories, MegaMenuPageFieldNameConstants.MegaMenuPages, MegaMenuPageFieldNameConstants.MegaMenuFilters, MegaMenuPageFieldNameConstants.MegaMenuEditor,
                        MegaMenuPageFieldNameConstants.MegaMenuAdditionalLink, MegaMenuPageFieldNameConstants.MegaMenuLinkToCategory, MegaMenuPageFieldNameConstants.MegaMenuLinkToPage} }
                },
            };

            return fields;
        }
    }
}
