using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Common.Events;
using Litium.Events;
using Litium.FieldFramework.Events;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products.Events;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Search.Filtering
{
    [Autostart]
    [Service(ServiceType = typeof(CategoryFilterService), Lifetime = DependencyLifetime.Singleton)]
    public class CategoryFilterService
    {
        private readonly ConcurrentDictionary<Guid, string[]> _cache = new ConcurrentDictionary<Guid, string[]>();
        private readonly FilterService _filterService;

        public CategoryFilterService(EventBroker eventBroker, FilterService filterService)
        {
            _filterService = filterService;

            eventBroker.Subscribe<PurgeFilterCache>(_ => _cache.Clear());
            eventBroker.Subscribe<CategoryCreated>(_ => ClearCache());
            eventBroker.Subscribe<CategoryDeleted>(_ => ClearCache());
            eventBroker.Subscribe<CategoryUpdated>(_ => ClearCache());
            eventBroker.Subscribe<FieldDefinitionCreated>(_ => ClearCache());
            eventBroker.Subscribe<FieldDefinitionDeleted>(_ => ClearCache());
            eventBroker.Subscribe<FieldDefinitionUpdated>(_ => ClearCache());
            eventBroker.Subscribe<SettingChanged>(x =>
            {
                if (string.Equals(FilterService._key, x.Key, StringComparison.OrdinalIgnoreCase) && x.PersonSystemId == Guid.Empty)
                {
                    ClearCache();
                }
            });

            void ClearCache()
            {
                _cache.Clear();
                eventBroker.Publish(EventScope.Remote, new PurgeFilterCache());
            }
        }

        public string[] GetFilters(Guid categorySystemId)
        {
            return _cache.GetOrAdd(categorySystemId, key =>
            {
                var filterFields = _filterService.GetProductFilteringFields();
                var customFilters = GetCustomFilterValues(key);
                return customFilters != null ? filterFields.Intersect(customFilters, StringComparer.OrdinalIgnoreCase).ToArray() : (filterFields?.ToArray() ?? new string[0]);
            });
        }

        private static IList<string> GetCustomFilterValues(Guid categorySystemId)
        {
            var category = categorySystemId.GetCategory();
            while (category != null)
            {
                var customFilterOptions = category.Fields.GetValue<IList<string>>(NavigationConstants.AcceleratorFilterFieldDefinitionName);
                if (customFilterOptions != null)
                {
                    return customFilterOptions;
                }

                if (category.ParentCategorySystemId == Guid.Empty)
                {
                    break;
                }

                category = category.ParentCategorySystemId.GetCategory();
            }

            return null;
        }

        private class PurgeFilterCache : IMessage { }
    }
}
