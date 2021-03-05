using System;
using System.Linq;
using Litium.Accelerator.Builders.LandingPage;
using Litium.Accelerator.Caching;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Product;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework.FieldTypes;
using Litium.Media;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;

namespace Litium.Accelerator.Builders.Product
{
    public class CategoryPageViewModelBuilder : PageModelBuilder<CategoryPageViewModel>
    {
        private readonly CategoryService _categoryService;
        private readonly LandingPageViewModelBuilder _landingPageViewModelBuilder;
        private readonly FileService _fileService;
        private readonly CategoryItemViewModelBuilder _categoryItemBuider;
        private readonly ProductItemViewModelBuilder _productItemBuilder;
        private readonly ProductSearchService _productSearchService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageByFieldTemplateCache<LandingPageByFieldTemplateCache> _landingPageByFieldTemplateCache;

        public CategoryPageViewModelBuilder(CategoryService categoryService,
            FileService fileService,
            CategoryItemViewModelBuilder categoryItemBuider,
            ProductItemViewModelBuilder productItemBuilder,
            LandingPageViewModelBuilder landingPageViewModelBuilder,
            ProductSearchService productSearchService,
            RequestModelAccessor requestModelAccessor,
            PageByFieldTemplateCache<LandingPageByFieldTemplateCache> landingPageByFieldTemplateCache)
        {
            _categoryService = categoryService;
            _fileService = fileService;
            _categoryItemBuider = categoryItemBuider;
            _productItemBuilder = productItemBuilder;
            _landingPageViewModelBuilder = landingPageViewModelBuilder;
            _productSearchService = productSearchService;
            _requestModelAccessor = requestModelAccessor;
            _landingPageByFieldTemplateCache = landingPageByFieldTemplateCache;
        }

        public override CategoryPageViewModel Build(Guid categorySystemId, DataFilterBase dataFilter = null)
        {
            if (categorySystemId == Guid.Empty)
            {
                return null;
            }
            var entity = _categoryService.Get(categorySystemId);
            if (entity == null)
            {
                return null;
            }
            var pageModel = new CategoryPageViewModel() { SystemId = categorySystemId };
            BuildFields(pageModel, entity, dataFilter?.Culture);
            BuildProducts(pageModel);
            BuildBlocks(pageModel, entity);
            BuildAdditionProperties(pageModel, entity);
            return pageModel;
        }

        private void BuildAdditionProperties(CategoryPageViewModel pageModel, Category entity)
        {
            var navigationType = _requestModelAccessor.RequestModel.WebsiteModel.GetNavigationType();
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery;
            var isFilterType = navigationType == NavigationType.Filter;
            var hasSections = pageModel.Blocks != null && pageModel.Blocks.Any() && !searchQuery.ContainsFilter();

            pageModel.Name = entity.Localizations.CurrentUICulture.SeoTitle ?? entity.Localizations.CurrentUICulture.Name;
            pageModel.Description = entity.Localizations.CurrentUICulture.Description;
            pageModel.ShowRegularHeader = isFilterType ? !hasSections && !searchQuery.ContainsFilter() : !hasSections;
            pageModel.ShowFilterHeader = isFilterType && searchQuery.ContainsFilter();
            pageModel.ShowSections = pageModel.Blocks != null && pageModel.Blocks.Any() && !searchQuery.ContainsFilter();
        }

        private void BuildFields(CategoryPageViewModel pageModel, Category entity, string culture)
        {
            var fields = entity.Fields;
            pageModel.Name = fields.GetName(culture);
            pageModel.Description = fields.GetDescription(culture);
            pageModel.Images = fields.GetImageUrls(_fileService);
        }

        private void BuildProducts(CategoryPageViewModel pageModel)
        {
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            if (searchQuery.PageSize == null)
            {
                var pageSize = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<int?>(AcceleratorWebsiteFieldNameConstants.ProductsPerPage) ?? DefaultWebsiteFieldValueConstants.ProductsPerPage;
                searchQuery.PageSize = pageSize;
            }

            var searchResponse = _productSearchService.Search(searchQuery, searchQuery.Tags, true, true, true);
            var searchResults = searchResponse == null ? null : _productSearchService.Transform(searchQuery, searchResponse);

            if (searchResults == null)
            {
                return;
            }

            pageModel.Products = searchResults.Items.Value.Cast<ProductSearchResult>().Select(c => _productItemBuilder.Build(c.Item)).ToList();
            pageModel.Pagination = new PaginationViewModel(searchResults.Total, searchQuery.PageNumber, searchResults.PageSize);
        }

        private void BuildBlocks(CategoryPageViewModel pageModel, Category entity)
        {
            if (_landingPageByFieldTemplateCache.TryGetPage(
                page => page.Fields.GetValue<PointerItem>(PageFieldNameConstants.CategoryPointer)?.EntitySystemId == entity.SystemId,
                out var landingPage))
            {
                var landingPageModel = _landingPageViewModelBuilder.Build(landingPage.MapTo<PageModel>());
                pageModel.Blocks = landingPageModel?.Blocks;
            }
        }
    }
}
