using System.Linq;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Brand;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;

namespace Litium.Accelerator.Builders.Brand
{
    public class BrandViewModelBuilder : IViewModelBuilder<BrandViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductItemViewModelBuilder _productItemViewModelBuilder;
        private readonly ProductSearchService _productSearchService;

        public BrandViewModelBuilder(RequestModelAccessor requestModelAccessor, ProductItemViewModelBuilder productItemViewModelBuilder, ProductSearchService productSearchService)
        {
            _requestModelAccessor = requestModelAccessor;
            _productItemViewModelBuilder = productItemViewModelBuilder;
            _productSearchService = productSearchService;
        }

        public virtual BrandViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<BrandViewModel>();

            var pageSize = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<int?>(AcceleratorWebsiteFieldNameConstants.ProductsPerPage) ?? DefaultWebsiteFieldValueConstants.ProductsPerPage;
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            searchQuery.CategoryShowRecursively = true;
            searchQuery.PageSize = pageSize;

            var navigationType = _requestModelAccessor.RequestModel.WebsiteModel.GetNavigationType();
            var showBlocks = model.Blocks.Any() && !searchQuery.ContainsFilter();

            model.ContainsFilter = searchQuery.ContainsFilter();
            model.ShowFilterHeader = navigationType == NavigationType.Filter && searchQuery.ContainsFilter();
            model.ShowRegularHeader = navigationType == NavigationType.Filter ? !model.Blocks.Any() && !searchQuery.ContainsFilter() : !showBlocks;

            var searchResponse = _productSearchService.Search(searchQuery, searchQuery.Tags,true, true, true);
            var searchResults = searchResponse != null ? _productSearchService.Transform(searchQuery, searchResponse) : null;
            if (searchResults == null)
            {
                return model;
            }
            
            model.Products = searchResults.Items.Value.Cast<ProductSearchResult>().Select(c => _productItemViewModelBuilder.Build(c.Item)).ToList();
            model.Pagination = new PaginationViewModel(searchResults.Total, searchQuery.PageNumber, searchResults.PageSize);

            return model;
        }
    }
}
