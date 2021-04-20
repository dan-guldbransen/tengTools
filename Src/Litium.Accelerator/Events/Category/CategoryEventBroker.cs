using Litium.Products.Events;
using Litium.Events;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.MetadataServices;
using System.Text;
using System.Threading.Tasks;
using Litium.Products;
using Litium.Globalization;
using Litium.Web;
using Litium.Security;

namespace Litium.Accelerator.Events.Category
{
    [Autostart]
    public class CategoryEventBroker : IDisposable
    {
        private readonly ISubscription<CategoryCreated> _createdSubscription;

        private readonly CategoryService _categoryService;
        private readonly ChannelService _channelService;
        private readonly SlugifyService _slugService;
        private readonly SecurityContextService _securityContextService;

        public CategoryEventBroker(EventBroker eventBroker,
            CategoryService categoryService,
            ChannelService channelService,
            SlugifyService slugService,
            SecurityContextService securityContextService)
        {
            _createdSubscription = eventBroker.Subscribe<CategoryCreated>(categoryCreatedEvent => SetupNewCategory(categoryCreatedEvent));

            _categoryService = categoryService;
            _channelService = channelService;
            _slugService = slugService;
            _securityContextService = securityContextService;
        }

        private void SetupNewCategory(CategoryCreated categoryCreatedEvent)
        {
            var channels = _channelService.GetAll().ToList();
            var category = _categoryService.Get(categoryCreatedEvent.SystemId);

            var clone = category.MakeWritableClone();

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

                clone.Fields.AddOrUpdateValue(FieldFramework.SystemFieldDefinitionConstants.Url, localization.Key, slug);
            }

            using(_securityContextService.ActAsSystem())
            {
                // Update entity
                _categoryService.Update(clone);
            }

        }

        public void Dispose()
        {
            // unregister the event from event broker
            _createdSubscription.Dispose();
        }
    }
}
