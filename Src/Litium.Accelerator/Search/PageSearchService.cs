using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Foundation.Modules.CMS.Search;
using Litium.Web.Customers.TargetGroups;
using Litium.Web.Customers.TargetGroups.Events;
using Litium.Foundation.Search;
using Litium.Framework.Search;
using Litium.Accelerator.Routing;
using Litium.Web;
using Litium.Accelerator.ViewModels.Search;
using Litium.Websites;
using System.Globalization;
using Litium.Accelerator.Constants;
using Litium.Common;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Search
{
    [Service(ServiceType = typeof(PageSearchService), Lifetime = DependencyLifetime.Singleton)]
    public abstract class PageSearchService
    {
        public abstract SearchResponse Search(SearchQuery searchQuery, bool? onlyBrands = null);
        public abstract SearchResult Transform(SearchQuery searchQuery, SearchResponse searchResponse, bool includeScore = false);
    }

    internal class PageSearchServiceImpl : PageSearchService
    {
        private readonly SearchService _searchService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly UrlService _urlService;
        private readonly SearchQueryBuilderFactory _searchQueryBuilderFactory;
        private readonly PageService _pageService;

        public PageSearchServiceImpl(
            SearchService searchService,
            RequestModelAccessor requestModelAccessor,
            UrlService urlService,
            SearchQueryBuilderFactory searchQueryBuilderFactory,
            PageService pageService)
        {
            _searchService = searchService;
            _requestModelAccessor = requestModelAccessor;
            _urlService = urlService;
            _searchQueryBuilderFactory = searchQueryBuilderFactory;
            _pageService = pageService;
        }

        public override SearchResponse Search(SearchQuery searchQuery, bool? onlyBrands = null)
        {
            if (!string.IsNullOrEmpty(searchQuery.PageType))
            {
                return SearchByPageType(searchQuery);
            }
            else if (!string.IsNullOrEmpty(searchQuery.Text))
            {
                return SearchByText(searchQuery, onlyBrands);
            }

            return null;
        }

        private SearchResponse SearchByText(SearchQuery searchQuery, bool? onlyBrands = null)
        {
            var websiteId = _requestModelAccessor.RequestModel.WebsiteModel.SystemId;
            var channelId = _requestModelAccessor.RequestModel.ChannelModel.SystemId;

            var searchQueryBuilder = _searchQueryBuilderFactory.Create(CultureInfo.CurrentUICulture, CmsSearchDomains.Pages, searchQuery);
            searchQueryBuilder.ApplyFreeTextSearchTags();
            searchQueryBuilder.ApplyDefaultSortOrder();

            var request = searchQueryBuilder.Build(pageSize: searchQuery.PageSize.GetValueOrDefault(100000), pageNumber: searchQuery.PageNumber);
            request.FilterTags.Add(new Tag(TagNames.Status, (int)ContentStatus.Published));
            request.FilterTags.Add(new Tag(TagNames.IsInTrashcan, false));
            request.FilterTags.Add(new Tag(TagNames.WebSiteId, websiteId));
            request.FilterTags.Add(new Tag(TagNames.IsSearchable, true));
            request.FilterTags.Add(new RangeTag(TagNames.GetTagNameForChannel(channelId, TagNames.ActiveChannelStartDate), DateTimeOffset.MinValue.DateTime, DateTimeOffset.UtcNow.DateTime));
            request.FilterTags.Add(new RangeTag(TagNames.GetTagNameForChannel(channelId, TagNames.ActiveChannelEndDate), DateTimeOffset.UtcNow.DateTime, DateTimeOffset.MaxValue.DateTime));

            if (onlyBrands != null)
            {
                const string brandPageType = PageTemplateNameConstants.Brand;
                if (onlyBrands == true)
                {
                    request.FilterTags.Add(new Tag(TagNames.TemplateId, brandPageType));
                }
                else
                {
                    request.ExcludeTags.Add(new Tag(TagNames.TemplateId, brandPageType));
                }
            }

            return _searchService.Search(request);
        }

        private SearchResponse SearchByPageType(SearchQuery searchQuery)
        {
            if (searchQuery.PageType == PageTemplateNameConstants.NewsList && searchQuery.PageSystemId.HasValue)
            {
                var channelId = _requestModelAccessor.RequestModel.ChannelModel.SystemId;
                var searchQueryBuilder = _searchQueryBuilderFactory.Create(CultureInfo.CurrentUICulture, CmsSearchDomains.Pages, searchQuery);

                var request = searchQueryBuilder.Build(pageSize: searchQuery.PageSize.GetValueOrDefault(100000), pageNumber: searchQuery.PageNumber);
                request.FilterTags.Add(new Tag(TagNames.ActiveChannelSystemId, channelId));
                request.FilterTags.Add(new Tag(TagNames.PageParentTreeId, searchQuery.PageSystemId.Value));
                request.FilterTags.Add(new Tag(TagNames.TemplateId, PageTemplateNameConstants.News));
                request.FilterTags.Add(new RangeTag(TagNames.GetTagNameForChannel(channelId, TagNames.ActiveChannelStartDate), DateTimeOffset.MinValue.DateTime, DateTimeOffset.UtcNow.DateTime));
                request.FilterTags.Add(new RangeTag(TagNames.GetTagNameForChannel(channelId, TagNames.ActiveChannelEndDate), DateTimeOffset.UtcNow.DateTime, DateTimeOffset.MaxValue.DateTime));
                request.Sortings.Add(new Sorting(TagNames.GetTagNameForProperty(PageFieldNameConstants.NewsDate), SortDirection.Descending, SortingFieldType.Date));

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

                    var pages = _pageService.Get(searchResponse.Hits.Select(x => new Guid(x.Id)).ToList());
                    return pages.Select(x => new PageSearchResult
                    {
                        Item = x,
                        Id = x.SystemId,
                        Name = x.Localizations.CurrentUICulture.Name,
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
