using System;
using System.Globalization;
using Litium.Runtime.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.Accelerator.Search
{
    [Service(ServiceType = typeof(SearchQueryBuilderFactory), Lifetime = DependencyLifetime.Singleton)]
    public class SearchQueryBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchQueryBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SearchQueryBuilder Create(CultureInfo culture, string searchType, SearchQuery searchQuery)
        {
            var searchQueryBuilder = ActivatorUtilities.CreateInstance<SearchQueryBuilder>(_serviceProvider);
            searchQueryBuilder.Init(culture, searchType, searchQuery);
            return searchQueryBuilder;
        }
    }
}
