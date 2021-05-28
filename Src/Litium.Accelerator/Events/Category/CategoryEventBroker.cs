using Litium.Products.Events;
using Litium.Events;
using Litium.Runtime;
using System;
using System.Linq;
using Litium.Products;
using Litium.Globalization;
using Litium.Web;
using Litium.Security;
using Litium.Studio.Plugins.Suggestions;

namespace Litium.Accelerator.Events.Category
{
    [Autostart]
    public class CategoryEventBroker : IDisposable
    {
        private readonly ISubscription<CategoryCreated> _createdSubscription;
        private readonly ISubscription<CategoryUpdated> _updatedSubscription;

        private readonly CategoryService _categoryService;
        private readonly ChannelService _channelService;
        private readonly SlugifyService _slugService;
        private readonly SecurityContextService _securityContextService;
        private readonly UrlValidator _urlValidator;

        public CategoryEventBroker(EventBroker eventBroker,
            CategoryService categoryService,
            ChannelService channelService,
            SlugifyService slugService,
            SecurityContextService securityContextService,
            UrlValidator urlValidator)
        {
            _createdSubscription = eventBroker.Subscribe<CategoryCreated>(categoryCreatedEvent => SetupNewCategory(categoryCreatedEvent));
            _updatedSubscription = eventBroker.Subscribe<CategoryUpdated>(categoryUpdatedEvent => SetupUpdatedCategory(categoryUpdatedEvent));


            _categoryService = categoryService;
            _channelService = channelService;
            _slugService = slugService;
            _securityContextService = securityContextService;
            _urlValidator = urlValidator;
        }

        private void SetupUpdatedCategory(CategoryUpdated categoryUpdatedEvent)
        {
            var hasChanges = false;

            var channels = _channelService.GetAll().ToList();
            var clone = categoryUpdatedEvent.Item.MakeWritableClone();

            // Channels
            foreach (var channel in channels)
            {
                var link = new CategoryToChannelLink(channel.SystemId);

                if (!clone.ChannelLinks.Contains(link))
                {
                    clone.ChannelLinks.Add(link);
                    hasChanges = true;
                }
            }

            // URL - Slugify
            foreach (var localization in clone.Localizations)
            {
                var name = clone.Fields.GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name, localization.Key);
                var slug = _slugService.Slugify(new System.Globalization.CultureInfo(localization.Key), name);
                var currentSlug = clone.Fields.GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Url, localization.Key);

                if(currentSlug != slug)
                {
                    var validation = _urlValidator.Validate(new System.Globalization.CultureInfo(localization.Key), slug);

                    var suffix = 0;
                    while (!validation.IsValid)
                    {
                        suffix++;
                        slug = $"{slug}_{suffix}";
                        validation = _urlValidator.Validate(new System.Globalization.CultureInfo(localization.Key), slug);
                    }

                    clone.Fields.AddOrUpdateValue(FieldFramework.SystemFieldDefinitionConstants.Url, localization.Key, slug);
                    hasChanges = true;
                }
            }

            if(hasChanges)
                using (_securityContextService.ActAsSystem())
                    _categoryService.Update(clone);
        }

        private void SetupNewCategory(CategoryCreated categoryCreatedEvent)
        {
            var channels = _channelService.GetAll().ToList();
            var clone = categoryCreatedEvent.Item.MakeWritableClone();

            // Channels
            foreach (var channel in channels)
            {
                clone.ChannelLinks.Add(new CategoryToChannelLink(channel.SystemId));
            }

            // URL - Slugify
            foreach (var localization in clone.Localizations)
            {
                var name = clone.Fields.GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name, localization.Key);
                var slug = _slugService.Slugify(new System.Globalization.CultureInfo(localization.Key), name);

                var validation = _urlValidator.Validate(new System.Globalization.CultureInfo(localization.Key), slug);

                var suffix = 0;
                while (!validation.IsValid)
                {
                    suffix++;
                    slug = $"{slug}_{suffix}";
                    validation = _urlValidator.Validate(new System.Globalization.CultureInfo(localization.Key), slug);
                }

                clone.Fields.AddOrUpdateValue(FieldFramework.SystemFieldDefinitionConstants.Url, localization.Key, slug);
            }

            using(_securityContextService.ActAsSystem())
                _categoryService.Update(clone);
        }

        public void Dispose()
        {
            // unregister the event from event broker
            _createdSubscription.Dispose();
            _updatedSubscription.Dispose();
        }
    }
}
