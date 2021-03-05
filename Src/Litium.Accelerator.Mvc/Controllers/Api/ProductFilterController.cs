using System.Linq;
using System.Web.Http;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Builders.Search;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Mvc.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;
using Litium.Studio.Extenssions;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [RoutePrefix("api/productFilter")]
    public class ProductFilterController : ApiControllerBase
    {
        private readonly FilterViewModelBuilder _filterViewModelBuilder;
        private readonly CategoryFilteringViewModelBuilder _categoryFilteringViewModelBuilder;
        private readonly SubNavigationViewModelBuilder _subNavigationViewModelBuilder;
        private readonly FilterProductViewModelBuilder _filterProductViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;

        public ProductFilterController(FilterViewModelBuilder filterViewModelBuilder, CategoryFilteringViewModelBuilder categoryFilteringViewModelBuilder, SubNavigationViewModelBuilder subNavigationViewModelBuilder, FilterProductViewModelBuilder filterProductViewModelBuilder,
            RequestModelAccessor requestModelAccessor)
        {
            _filterViewModelBuilder = filterViewModelBuilder;
            _categoryFilteringViewModelBuilder = categoryFilteringViewModelBuilder;
            _subNavigationViewModelBuilder = subNavigationViewModelBuilder;
            _filterProductViewModelBuilder = filterProductViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
        }

        /// <summary>
        /// Gets the product filter without the HTML result.
        /// </summary>
        [Route]
        [HttpGet]
        public IHttpActionResult Get() => Get(false);

        /// <summary>
        /// Gets the product filter with the result as HTML in the returned object.
        /// </summary>
        [HttpGet]
        [Route("withHtmlResult")]
        public IHttpActionResult WithHtmlResult() => Get(true);

        private IHttpActionResult Get(bool withHtmlResult)
        {
            if (_requestModelAccessor.RequestModel.CurrentPageModel == null)
            {
                return Ok();
            }
            FacetSearchResult result = new FacetSearchResult();
            var productFilter = _filterProductViewModelBuilder.Build();
            if (productFilter != null)
            {
                if (withHtmlResult)
                {
                    result.ProductsView = this.RenderViewToString(GetViewName(), productFilter.ViewData);
                }
                result.SortCriteria = _categoryFilteringViewModelBuilder.Build(productFilter.TotalCount);
            }
            result.FacetFilters = _filterViewModelBuilder.Build()?.Items.Select(c => c.MapTo<FacetGroupFilter>());
            result.SubNavigation = _subNavigationViewModelBuilder.Build();
            result.NavigationTheme = _requestModelAccessor.RequestModel.WebsiteModel.GetNavigationType().ToString().ToCamelCase();

            return Ok(result);
        }

        private string GetViewName()
        {
            var page = _requestModelAccessor.RequestModel.CurrentPageModel;
            if (page.IsBrandPageType())
            {
                //Brand Page Type
                return "~/Views/Brand/Index.cshtml";
            }
            else if (page.IsSearchResultPageType())
            {
                //Search Result Page Type
                return "~/Views/Search/Index.cshtml";
            }
            else if (page.IsProductListPageType())
            {
                //Product List Page Type
                return "~/Views/ProductList/Index.cshtml";
            }
            else
            {
                //Category page
                return "~/Views/Category/Index.cshtml";
            }
        }
    }
}
