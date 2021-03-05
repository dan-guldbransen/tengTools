using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.ViewModels.Search;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search;
using Litium.Framework.Search;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Customers.TargetGroups;
using Litium.Web.Customers.TargetGroups.Events;

namespace Litium.Accelerator.Search
{
    [Service(ServiceType = typeof(CategorySearchService), Lifetime = DependencyLifetime.Singleton)]
    public abstract class CategorySearchService
    {
        public abstract SearchResponse Search(SearchQuery searchQuery);
        public abstract SearchResult Transform(SearchQuery searchQuery, SearchResponse searchResponse, bool includeScore = false);
    }

    internal class CategorySearchServiceImpl : CategorySearchService
    {
        private readonly SearchService _searchService;
        private readonly UrlService _urlService;
        private readonly SearchQueryBuilderFactory _searchQueryBuilderFactory;

        public CategorySearchServiceImpl(
            SearchService searchService,
            UrlService urlService,
            SearchQueryBuilderFactory searchQueryBuilderFactory)
        {
            _searchService = searchService;
            _urlService = urlService;
            _searchQueryBuilderFactory = searchQueryBuilderFactory;
        }

        public override SearchResponse Search(SearchQuery searchQuery)
        {
            if (!string.IsNullOrEmpty(searchQuery.Text))
            {
                var searchQueryBuilder = _searchQueryBuilderFactory.Create(CultureInfo.CurrentCulture, ProductCatalogSearchDomains.Categories, searchQuery);

                searchQueryBuilder.ApplyProductCatalogDefaultSearchTag();
                searchQueryBuilder.ApplyFreeTextSearchTags();
                searchQueryBuilder.ApplyCategoryTags();
                searchQueryBuilder.ApplySelectedFilterTags();
                searchQueryBuilder.ApplyOrganizationSearchTags();
                searchQueryBuilder.ApplyDefaultSortOrder();

                var request = searchQueryBuilder.Build(pageNumber: searchQuery.PageNumber, pageSize: searchQuery.PageSize.GetValueOrDefault(100000));
                return _searchService.Search(request);
            }

            return null;
        }

        public override SearchResult Transform(SearchQuery searchQuery, SearchResponse searchResponse, bool includeScore = false)
        {
            return new SearchResult
            {
                Items = new Lazy<IEnumerable<SearchResultItem>>(() =>
                {
                    IoC.Resolve<TargetGroupEngine>().Process(new SearchEvent
                    {
                        SearchText = searchQuery.Text,
                        TotalHits = searchResponse.TotalHitCount
                    });

                    var categories = searchResponse.Hits.Select(x => new Guid(x.Id).GetCategory());
                    return categories.Where(c => c != null).Select(x => new CategorySearchResult
                    {
                        Item = x,
                        Id = x.SystemId,
                        Name = x.Localizations.CurrentCulture.Name,
                        Url = _urlService.GetUrl(x),
                        Score = includeScore ? searchResponse.Hits.Where(z => z.Id == x.SystemId.ToString()).Select(z => z.Score).FirstOrDefault() : default
                    });
                }),
                PageSize = searchQuery.PageSize.GetValueOrDefault(100000),
                Total = searchResponse.TotalHitCount
            };
        }
    }
}
