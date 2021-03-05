using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Common;
using Litium.Runtime.DependencyInjection;
using System.Collections.Generic;

namespace Litium.Accelerator.Search.Filtering
{
    [Service(ServiceType = typeof(FilterService), Lifetime = DependencyLifetime.Singleton)]
    public class FilterService
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly SettingService _settingsService;

        internal const string _key = "Accelerator.ProductFiltering";

        public FilterService(RequestModelAccessor requestModelAccessor, SettingService settingsService)
        {
            _requestModelAccessor = requestModelAccessor;
            _settingsService = settingsService;
        }

        public bool IndexFilter(string filterName)
        {
            var filters = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<IList<string>>(AcceleratorWebsiteFieldNameConstants.FiltersIndexedBySearchEngines);
            return filters != null && filters.Contains(filterName);
        }

        public IList<string> GetProductFilteringFields()
        {
            return _settingsService.Get<IList<string>>(_key) ?? new List<string>();
        }

        public void SaveProductFilteringFields(ICollection<string> items)
        {
            _settingsService.Set(_key, items);
        }
    }
}
