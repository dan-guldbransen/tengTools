using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Search;

namespace Litium.Accelerator.Searchers
{
    public class CategorySearcher : BaseSearcher<CategorySearchResult>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly CategorySearchService _categorySearchService;

        public CategorySearcher(CategorySearchService categorySearchService, RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
            _categorySearchService = categorySearchService;
        }

        public override int SortOrder => 200;

        public override int PageSize => 10;

        public override string ModelKey => "Categories";

        public override SearchResult QueryCompact(string query, bool includeScore = false)
        {
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            searchQuery.Text = query;
            searchQuery.PageNumber = 1;
            searchQuery.PageSize = PageSize;

            var searchResponse = _categorySearchService.Search(searchQuery);
            if (searchResponse == null)
            {
                return null;
            }

            return _categorySearchService.Transform(searchQuery, searchResponse, includeScore: includeScore);
        }
    }
}
