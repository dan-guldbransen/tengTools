using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Search;

namespace Litium.Accelerator.Searchers
{
    public class PageSearcher : BaseSearcher<PageSearchResult>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageSearchService _pageSearchService;

        public PageSearcher(PageSearchService pageSearchService, RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
            _pageSearchService = pageSearchService;
        }

        public override int SortOrder => 400;

        public override int PageSize => 10;

        public override string ModelKey => "Pages";

        public virtual bool? OnlyBrands => false;

        public override SearchResult QueryCompact(string query, bool includeScore = false)
        {
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            searchQuery.Text = query;
            searchQuery.PageNumber = 1;
            searchQuery.PageSize = PageSize;

            var searchResponse = _pageSearchService.Search(searchQuery, onlyBrands: OnlyBrands);
            if (searchResponse == null)
            {
                return null;
            }

            return _pageSearchService.Transform(searchQuery, searchResponse, includeScore: includeScore);
        }
    }
}
