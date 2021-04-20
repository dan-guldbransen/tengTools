using Litium.Events;
using Litium.Globalization;
using Litium.Products;
using Litium.Products.Events;
using Litium.Security;
using Litium.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Events.Product
{
    public class VariantEventBroker
    {
        private readonly CategoryService _categoryService;
        private readonly VariantService _variantService;
        private readonly BaseProductService _baseProductService;
        private readonly ChannelService _channelService;
        private readonly SlugifyService _slugService;
        private readonly SecurityContextService _securityContextService;

        public VariantEventBroker(EventBroker eventBroker,
            VariantService variantService,
            BaseProductService baseProductService,
            CategoryService categoryService,
            ChannelService channelService,
            SlugifyService slugService,
            SecurityContextService securityContextService)
        {

            _variantService = variantService;
            _baseProductService = baseProductService;
            _channelService = channelService;
            _slugService = slugService;
            _securityContextService = securityContextService;
            _categoryService = categoryService;

            eventBroker.Subscribe<VariantCreated>(variantCreatedEvent => SetupNewVariant(variantCreatedEvent));
            eventBroker.Subscribe<VariantUpdated>(variantCreatedEvent => SetupUpdatedVariant(variantCreatedEvent));

        }

        private void SetupNewVariant(VariantCreated variantCreatedEvent)
        {
            // Publish to channels based on markets
            var channels = _channelService.GetAll().ToList();
            var variant = _variantService.Get(variantCreatedEvent.SystemId);

            var marketField = variant.Fields.GetValue<string>("ItemApprovedForMarket");
            var marketList = new List<string>();
            if (!string.IsNullOrEmpty(marketField))
            {
                marketList = marketField.Split(',').ToList();
            }

            var clone = variant.MakeWritableClone();
            foreach (var market in marketList)
            {
                var channel = channels.FirstOrDefault(x => x.Fields.GetValue<string>("_name", "en-US") == market);
                if(channel == null)
                    continue;

                // add channellink
                clone.ChannelLinks.Add(new VariantToChannelLink(channel.SystemId));
            }

            // Set category link (on baseproduct)
            var baseProduct = _baseProductService.Get(variant.BaseProductSystemId);
            var productCategoryNumber = baseProduct.Fields.GetValue<string>("ProductCategoryNumber");
            var productGroupNumber = baseProduct.Fields.GetValue<string>("ProductGroupNumber");

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
            // update publish to channels based on markets if missmatch

            // update category link if changed category ids

            // update url if name changed

            // Save
        }
    }
}
