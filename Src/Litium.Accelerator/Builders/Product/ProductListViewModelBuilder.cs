using System;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Product;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.Builders.Product
{
    public class ProductListViewModelBuilder : IViewModelBuilder<ProductListViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductSearchService _productSearchService;
        private readonly ProductItemViewModelBuilder _productItemBuilder;

        public ProductListViewModelBuilder(RequestModelAccessor requestModelAccessor, ProductSearchService productSearchService, ProductItemViewModelBuilder productItemBuilder)
        {
            _requestModelAccessor = requestModelAccessor;
            _productSearchService = productSearchService;
            _productItemBuilder = productItemBuilder;
        }

        public ProductListViewModel Build()
        {
            var page = _requestModelAccessor.RequestModel.CurrentPageModel;
            var viewModel = page.MapTo<ProductListViewModel>();

            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            if (searchQuery.PageSize == null)
            {
                var pageSize = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<int?>(AcceleratorWebsiteFieldNameConstants.ProductsPerPage) ?? DefaultWebsiteFieldValueConstants.ProductsPerPage;
                searchQuery.PageSize = pageSize;
            }

            if (!searchQuery.ContainsFilter())
            {
                searchQuery.CategoryShowRecursively = false;
            }

            var searchResponse = _productSearchService.Search(searchQuery, searchQuery.Tags, true, true, true);
            var searchResults = searchResponse == null ? null : _productSearchService.Transform(searchQuery, searchResponse);

            if (searchResults != null)
            {
                viewModel.Products = searchResults.Items.Value.Cast<ProductSearchResult>().Select(c => _productItemBuilder.Build(c.Item)).ToList();
                viewModel.Pagination = new PaginationViewModel(searchResults.Total, searchQuery.PageNumber, searchResults.PageSize);
            }

            return viewModel;
        }
    }
}
