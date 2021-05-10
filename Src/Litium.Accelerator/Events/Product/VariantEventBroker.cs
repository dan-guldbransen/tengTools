using Litium.Accelerator.Constants;
using Litium.Events;
using Litium.Globalization;
using Litium.Products;
using Litium.Products.Events;
using Litium.Runtime;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Studio.FieldFramework.FieldConverters;
using Litium.Web;
using Litium.Web.Models.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Events.Product
{
    [Autostart]
    public class VariantEventBroker : IDisposable
    {
        private readonly ISubscription<VariantCreated> _createdSubscription;
        private readonly ISubscription<VariantUpdated> _updatedSubscription;

        private readonly CategoryService _categoryService;
        private readonly BaseProductService _baseProductService;
        private readonly VariantService _variantService;
        private readonly ChannelService _channelService;
        private readonly SlugifyService _slugService;
        private readonly SecurityContextService _securityContextService;
        private readonly LanguageService _lanugageService;

        public VariantEventBroker(EventBroker eventBroker,
             BaseProductService baseProductService,
             VariantService variantService,
             CategoryService categoryService,
            ChannelService channelService,
            SlugifyService slugService,
            SecurityContextService securityContextService,
            LanguageService languageService)
        {
            _baseProductService = baseProductService;
            _categoryService = categoryService;
            _variantService = variantService;
            _channelService = channelService;
            _slugService = slugService;
            _securityContextService = securityContextService;
            _lanugageService = languageService;

            _createdSubscription = eventBroker.Subscribe<VariantCreated>(variantCreatedEvent => SetupNewVariant(variantCreatedEvent));
            _updatedSubscription = eventBroker.Subscribe<VariantUpdated>(variantCreatedEvent => SetupUpdatedVariant(variantCreatedEvent));

        }

        private void SetupNewVariant(VariantCreated variantCreatedEvent)
        {
            // Publish to channels based on markets
            var channels = _channelService.GetAll().ToList();
            var variant = variantCreatedEvent.Item;

            // Categories
            var baseProduct = _baseProductService.Get(variant.BaseProductSystemId);
            var productCategoryNumber = baseProduct.Fields.GetValue<string>("ProductCategoryNumber");
            var productGroupNumber = baseProduct.Fields.GetValue<string>("ProductGroupNumber");

            // Get existing categories if set
            var categories = _categoryService.GetByBaseProduct(variant.BaseProductSystemId);
            
            // Get categories to add/update based on baseproduct attributes
            var mainCat = _categoryService.Get(productCategoryNumber);
            var subCat = _categoryService.Get(productGroupNumber);

            if(categories == null || !categories.Any())
            {
                // if we get here we need set link to baseproduct also
                var mainClone = mainCat.MakeWritableClone();
                var subClone = subCat.MakeWritableClone();
                
                var mainLink = new CategoryToProductLink(baseProduct.SystemId);
                mainLink.ActiveVariantSystemIds.Add(variant.SystemId);
                mainClone.ProductLinks.Add(mainLink);

                var subLink = new CategoryToProductLink(baseProduct.SystemId);
                subLink.ActiveVariantSystemIds.Add(variant.SystemId);

                // lowest categoy in tree is main category
                subLink.MainCategory = true;
                subClone.ProductLinks.Add(subLink);

                using (_securityContextService.ActAsSystem())
                {
                    _categoryService.Update(mainClone);
                    _categoryService.Update(subClone);
                }
            }
            else
            {
                var main = categories.FirstOrDefault(x => x.Id == mainCat.Id);
                var sub = categories.FirstOrDefault(x => x.Id == subCat.Id);

                // set main category if doesnt exist
                if(main != null)
                {
                    var link = main.ProductLinks.First(c => c.BaseProductSystemId == baseProduct.SystemId);
                    if(!link.ActiveVariantSystemIds.Contains(variant.SystemId))
                    {
                        var mainClone = main.MakeWritableClone();
                        mainClone.ProductLinks.First(c => c.BaseProductSystemId == baseProduct.SystemId).ActiveVariantSystemIds.Add(variant.SystemId);

                        using (_securityContextService.ActAsSystem())
                            _categoryService.Update(mainClone);
                    }
                }

                // set sub category if doesnt exist
                if(sub != null)
                {
                    var link = sub.ProductLinks.First(c => c.BaseProductSystemId == baseProduct.SystemId);
                    if (!link.ActiveVariantSystemIds.Contains(variant.SystemId))
                    {
                        var subClone = sub.MakeWritableClone();
                        subClone.ProductLinks.First(c => c.BaseProductSystemId == baseProduct.SystemId).ActiveVariantSystemIds.Add(variant.SystemId);
                        
                        using(_securityContextService.ActAsSystem())
                            _categoryService.Update(subClone);
                    }
                }
            }

            // get markets where variant should be published
            var marketField = variant.Fields.GetValue<string>("ItemApprovedForMarket");
            var marketList = new List<string>();
            if (!string.IsNullOrEmpty(marketField))
            {
                marketList = marketField.Split(',').ToList();
            }

            var clone = variant.MakeWritableClone();
            
            // check each channel if variant should have link
            foreach(var channel in channels)
            {
                var isInternational = channel.Fields.GetValue<bool>(ChannelFieldNameConstants.IsInternational);

                // international gets all
                if (isInternational)
                {
                    // add channellink
                    clone.ChannelLinks.Add(new VariantToChannelLink(channel.SystemId));
                }
                else
                {
                    // based on language on channel we set active channellinks, channel should have a correct set language in litium
                    var languageId = channel.ProductLanguageSystemId ?? channel.WebsiteLanguageSystemId ?? null;
                    if(!languageId.HasValue)
                        continue;

                    var language = _lanugageService.Get(languageId.Value);
                    if(marketList.Contains(language.CultureInfo.Name.ToLower()))
                    {
                        // add channellink
                        clone.ChannelLinks.Add(new VariantToChannelLink(channel.SystemId));
                    }
                    
                }
            }

            // Create url
            foreach (var localization in clone.Localizations)
            {
                var name = clone.Fields.GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name, localization.Key);
                var slug = _slugService.Slugify(new System.Globalization.CultureInfo(localization.Key), name);

                clone.Fields.AddOrUpdateValue(FieldFramework.SystemFieldDefinitionConstants.Url, localization.Key, slug);
            }

            // Save
            using (_securityContextService.ActAsSystem())
            {
                // Update entity
                _variantService.Update(clone);
            }
        }

        private void SetupUpdatedVariant(VariantUpdated variantCreatedEvent)
        {
            var hasChanges = false;

            // update publish to channels based on markets if missmatch

            // update category link if changed category ids TODO Move this to base produt updated and move all variants if category changed

            // update url if name changed

            // Save
        }

        public void Dispose()
        {
            // unregister the event from event broker
            _createdSubscription.Dispose();
            _updatedSubscription.Dispose();
        }
    }
}
