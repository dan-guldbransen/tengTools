using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Builders.LandingPage;
using Litium.Accelerator.Caching;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Accelerator.ViewModels.Product;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Web.Models;
using Litium.Web.Models.Websites;
using Litium.Web.Rendering;

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
        private readonly MarketService _marketService;
        private readonly AuthorizationService _authorizationService;
        private readonly ICollection<IRenderingValidator<Category>> _renderingValidators;

        private List<Guid> _selectedStructureId;
        private Guid _currentCategorySystemId;

        public CategoryPageViewModelBuilder(CategoryService categoryService,
            FileService fileService,
            CategoryItemViewModelBuilder categoryItemBuider,
            ProductItemViewModelBuilder productItemBuilder,
            LandingPageViewModelBuilder landingPageViewModelBuilder,
            ProductSearchService productSearchService,
            RequestModelAccessor requestModelAccessor,
            MarketService marketService,
            AuthorizationService authorizationService,
            ICollection<IRenderingValidator<Category>> renderingValidators,
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
            _marketService = marketService;
            _authorizationService = authorizationService;
            _renderingValidators = renderingValidators;
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
            var linksAndShow = ShowProducts(entity);
            pageModel.ShowProducts = linksAndShow.Showproducts;
            pageModel.Links = linksAndShow.Links;
            pageModel.Image = linksAndShow.Image;
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
            if (!pageModel.ShowProducts)
            {
                pageModel.ShowRegularHeader = false;
                pageModel.ShowFilterHeader = false;
            }
        }

        private void BuildFields(CategoryPageViewModel pageModel, Category entity, string culture)
        {
            var fields = entity.Fields;
            pageModel.Name = fields.GetName(culture);
            pageModel.Description = fields.GetDescription(culture);
            pageModel.Images = fields.GetImageUrls(_fileService);
            pageModel.ShowProducts = true;
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

        private SubCategoryLinksAndShowProducts ShowProducts(Category category)
        {
            var output = new SubCategoryLinksAndShowProducts();
            var show = false;
            var market = _marketService.Get(_requestModelAccessor.RequestModel.ChannelModel.Channel.MarketSystemId.Value);
            var firstLevelCategories = _categoryService.GetChildCategories(Guid.Empty, market.AssortmentSystemId)
                                                      .Where(x => x.IsPublished(_requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId)
                                                      && _authorizationService.HasOperation<Category>(Operations.Entity.Read, x.SystemId)
                                                      && _renderingValidators.Validate(x));

            var currentCategoryParentSystemId = category.ParentCategorySystemId;
            _selectedStructureId = category.GetParents().Select(x => x.SystemId).ToList();
            _currentCategorySystemId = category.SystemId;
            _selectedStructureId.Add(category.SystemId);

            var contentLink = new SubNavigationLinkModel
            {
                IsSelected = true,
                Name = category.Localizations.CurrentCulture.Name,
                Links = firstLevelCategories
                            .Where(x => currentCategoryParentSystemId == Guid.Empty || _selectedStructureId.Contains(x.SystemId))
                            .Select(x =>
                            {
                                var showAll = currentCategoryParentSystemId == Guid.Empty && _selectedStructureId.Contains(x.SystemId);
                                return new SubNavigationLinkModel
                                {
                                    IsSelected = _selectedStructureId.Contains(x.SystemId),
                                    Name = x.Localizations.CurrentCulture.Name,
                                    Url = x.GetUrl(_requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId),
                                    Links = _selectedStructureId.Contains(x.SystemId) ?
                                    GetChildLinks(x, showAll, showAll ? 0 : 1).ToList() :
                                    (x.GetChildren().Any() ? new List<SubNavigationLinkModel>() : null)
                                };
                            }).ToList()
            };

            SubNavigationLinkModel selectedLink = new   SubNavigationLinkModel();
            if (contentLink.Links != null && contentLink.Links.Any())
            {
                foreach (var link in contentLink.Links)
                {
                    if (link.IsSelected)
                    {
                        selectedLink = link;
                        if (link.Links != null && link.Links.Any())
                        {
                            foreach (var subLink in link.Links)
                            {
                                if (subLink.IsSelected)
                                {
                                    selectedLink = subLink;
                                    if (subLink.Links != null && subLink.Links.Any())
                                    {
                                        foreach (var thirdLvLink in subLink.Links)
                                        {
                                            if (thirdLvLink.IsSelected)
                                            {
                                                selectedLink = thirdLvLink;
                                                if (thirdLvLink.Links != null && thirdLvLink.Links.Any())
                                                {
                                                    // more subcategories?
                                                }
                                                else
                                                {
                                                    show = true;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        show = true;
                                    }
                                }
                                break;
                            }
                        }
                        else
                        {
                            show = true;
                        }
                    }
                    break;
                }
            }
            output.Showproducts = show;
            output.Links = contentLink.Links;
            output.Image = selectedLink.Image;
            return output;
        }

        private IEnumerable<SubNavigationLinkModel> GetChildLinks(Category category, bool showAll = false, int level = int.MaxValue)
        {
            var res = new List<SubNavigationLinkModel>();

            foreach (var child in category.GetChildren()
                                          .Where(pg => pg.IsPublished(_requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId)
                                                       && _authorizationService.HasOperation<Category>(Operations.Entity.Read, pg.SystemId)
                                                       && _renderingValidators.Validate(pg)))
            {
                if (showAll || _selectedStructureId.Contains(child.SystemId))
                {
                    var link = new SubNavigationLinkModel
                    {
                        Name = child.Localizations.CurrentCulture.Name,
                        Description = child.Localizations.CurrentUICulture.Description,
                        Image = child.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images).MapTo<IList<ImageModel>>()?.FirstOrDefault(),
                        Url = child.GetUrl(_requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId),
                        IsSelected = _selectedStructureId.Contains(child.SystemId),
                        Links = _selectedStructureId.Contains(child.SystemId) ?
                                GetChildLinks(child, _currentCategorySystemId == child.SystemId, level + 1).ToList() :
                                (category.GetChildren().Any() ? new List<SubNavigationLinkModel>() : null)
                    };
                    res.Add(link);
                }
            }
            return res;
        }
    }
     class SubCategoryLinksAndShowProducts
    {
        public bool Showproducts { get; set; }
        public IList<SubNavigationLinkModel> Links { get; set; } = new List<SubNavigationLinkModel>();
        public ImageModel Image { get; set; }
    }
}
