using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Search;

namespace Litium.Accelerator.Builders.Search
{
    public class SearchResultViewModelBuilder : IViewModelBuilder<SearchResultViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductSearchService _productSearchService;
        private readonly ProductItemViewModelBuilder _productItemBuilder;
        private readonly PageSearchService _pageSearchService;
        private readonly CategorySearchService _categorySearchService;

        public SearchResultViewModelBuilder(RequestModelAccessor requestModelAccessor, ProductSearchService productSearchService, ProductItemViewModelBuilder productItemBuilder,
           PageSearchService pageSearchService, CategorySearchService categorySearchService)
        {
            _requestModelAccessor = requestModelAccessor;
            _productSearchService = productSearchService;
            _productItemBuilder = productItemBuilder;
            _pageSearchService = pageSearchService;
            _categorySearchService = categorySearchService;
        }

        public virtual SearchResultViewModel Build()
        {
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            var searchResult = new SearchResultViewModel() { SearchTerm = searchQuery.Text, ContainsFilter = searchQuery.ContainsFilter() };

            if (!string.IsNullOrWhiteSpace(searchQuery.Text))
            {
                BuildProducts(searchResult, searchQuery);
                BuildOtherSearchResult(searchResult, searchQuery);
            }

            return searchResult;
        }

        private void BuildProducts(SearchResultViewModel searchResult, SearchQuery searchQuery)
        {
            if (searchQuery.PageSize == null)
            {
                var pageSize = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<int?>(AcceleratorWebsiteFieldNameConstants.ProductsPerPage) ?? DefaultWebsiteFieldValueConstants.ProductsPerPage;
                searchQuery.PageSize = pageSize;
            }

            var searchResponse = _productSearchService.Search(searchQuery, searchQuery.Tags, true, true, true);
            var searchResults = searchResponse == null ? null : _productSearchService.Transform(searchQuery, searchResponse);

            if (searchResults != null)
            {
                searchResult.Products = searchResults.Items.Value.Cast<ProductSearchResult>().Select(c => _productItemBuilder.Build(c.Item)).ToList();
                searchResult.Pagination = new PaginationViewModel(searchResults.Total, searchQuery.PageNumber, searchResults.PageSize);
            }
        }

        private void BuildOtherSearchResult(SearchResultViewModel searchResult, SearchQuery searchQuery)
        {
            var p = searchQuery.PageSize;
            searchQuery.PageNumber = 1;
            searchQuery.PageSize = 100;

            var searchResponse = _pageSearchService.Search(searchQuery);
            var pageSearchResult = searchResponse != null ? _pageSearchService.Transform(searchQuery, searchResponse, true) : null;

            searchResponse = _categorySearchService.Search(searchQuery);
            var categorySearchResult = searchResponse != null ? _categorySearchService.Transform(searchQuery, searchResponse, true) : null;

            searchQuery.PageSize = p;
            searchResult.OtherSearchResult = new SearchResult
            {
                PageSize = 100,
                Total = (pageSearchResult?.Total ?? 0) + (categorySearchResult?.Total ?? 0),
                Items = new Lazy<IEnumerable<SearchResultItem>>(() => (pageSearchResult == null ? new SearchResultItem[0] : pageSearchResult.Items.Value).Concat(categorySearchResult == null ? new SearchResultItem[0] : categorySearchResult.Items.Value).OrderByDescending(x => x.Score))
            };
        }
    }
}
