using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Common;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.Routing;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Web;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Websites;
using Litium.Web.Rendering;
using Litium.Web.Routing;
using Litium.Websites;

namespace Litium.Accelerator.Builders.Search
{
    public class SubNavigationViewModelBuilder : IViewModelBuilder<NavigationViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly CategoryService _categoryService;
        private readonly MarketService _marketService;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;
        private readonly SearchQueryBuilderFactory _searchQueryBuilderFactory;
        private readonly PageSearchService _pageSearchService;
        private readonly AuthorizationService _authorizationService;
        private readonly ICollection<IRenderingValidator<Category>> _renderingValidators;

        private List<Guid> _selectedStructureId;
        private Guid _currentCategorySystemId;
        private WebsiteModel _website;
        private ChannelModel _channel;
        private PageModel _page;
        private Guid _currentSelectedPageId;

        public SubNavigationViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            CategoryService categoryService,
            MarketService marketService,
            PageService pageService,
            UrlService urlService,
            SearchQueryBuilderFactory searchQueryBuilderFactory,
            PageSearchService pageSearchService,
            AuthorizationService authorizationService,
            ICollection<IRenderingValidator<Category>> renderingValidators)
        {
            _requestModelAccessor = requestModelAccessor;
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _categoryService = categoryService;
            _marketService = marketService;
            _pageService = pageService;
            _urlService = urlService;
            _searchQueryBuilderFactory = searchQueryBuilderFactory;
            _pageSearchService = pageSearchService;
            _website = _requestModelAccessor.RequestModel.WebsiteModel;
            _channel = _requestModelAccessor.RequestModel.ChannelModel;
            _page = _requestModelAccessor.RequestModel.CurrentPageModel;
            _authorizationService = authorizationService;
            _renderingValidators = renderingValidators;
        }

        /// <summary>
        /// Buid Sub Navigation Model
        /// </summary>
        /// <param name="currentPageModel"></param>
        /// <returns></returns>
        public SubNavigationLinkModel Build()
        {
            SubNavigationLinkModel contentLink = new SubNavigationLinkModel();
            var filterNavigation = _website.GetNavigationType();
            var productCatalogData = _routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData;
            var pageTypeName = _page.GetPageType();

            // sub navigation
            if (productCatalogData != null)
            {
                contentLink = filterNavigation == NavigationType.Category ? CreateProductCategoryNavigation(productCatalogData) : CreateProductFilterNavigation(productCatalogData);
            }
            else if (pageTypeName != PageTemplateNameConstants.Brand && pageTypeName != PageTemplateNameConstants.ProductList
                && pageTypeName != PageTemplateNameConstants.SearchResult)
            {
                contentLink = CreatePageNavigation();
            }

            return contentLink;
        }

        private SubNavigationLinkModel CreateProductCategoryNavigation(ProductPageData productCatalogData)
        {
            var category = _categoryService.Get(productCatalogData.CategorySystemId.Value);
            if (category == null)
            {
                return null;
            }

            _selectedStructureId = category.GetParents().Select(x => x.SystemId).ToList();
            _currentCategorySystemId = category.SystemId;
            _selectedStructureId.Add(_currentCategorySystemId);

            var market = _marketService.Get(_channel.Channel.MarketSystemId.Value);
            var firstLevelCategories = _categoryService
                .GetChildCategories(Guid.Empty, market.AssortmentSystemId)
                .Where(x => x.ChannelLinks.Any(z => z.ChannelSystemId == _channel.SystemId)
                            && _authorizationService.HasOperation<Category>(Operations.Entity.Read, x.SystemId)
                            && _renderingValidators.Validate(x))
                .ToList();
            if (firstLevelCategories.Count == 0)
            {
                return null;
            }

            return new SubNavigationLinkModel
            {
                IsSelected = true,
                Name = _website.Texts.GetValue("ProductCategories") ?? "Product Categories",
                Links = firstLevelCategories.Select(x => new SubNavigationLinkModel
                {
                    IsSelected = _selectedStructureId.Contains(x.SystemId),
                    Name = x.Localizations.CurrentCulture.Name,
                    Url = x.GetUrl(_channel.SystemId),
                    Links = _selectedStructureId.Contains(x.SystemId)
                        ? GetChildLinks(x).ToList()
                        : (x.GetChildren().Any() ? new List<SubNavigationLinkModel>() : null)
                }).ToList()
            };
        }

        private IEnumerable<SubNavigationLinkModel> GetChildLinks(Category category)
        {
            var res = new List<SubNavigationLinkModel>();

            foreach (var child in category
                .GetChildren()
                .Where(x => x.IsPublished(_channel.SystemId)
                            && _renderingValidators.Validate(x)))
            {
                var link = new SubNavigationLinkModel
                {
                    Name = child.Localizations.CurrentCulture.Name,
                    Url = child.GetUrl(_channel.SystemId)
                };

                if (_selectedStructureId.Contains(child.SystemId))
                {
                    link.IsSelected = true;
                    link.Links = GetChildLinks(child).ToList();
                }
                else if (child.GetChildren().Any())
                {
                    link.Links = new List<SubNavigationLinkModel>();
                }
                res.Add(link);
            }

            return res;
        }

        private SubNavigationLinkModel CreateProductFilterNavigation(ProductPageData productCatalogData)
        {
            var category = _categoryService.Get(productCatalogData.CategorySystemId.Value);
            if (category == null)
            {
                return null;
            }

            _selectedStructureId = category.GetParents().Select(x => x.SystemId).ToList();
            _currentCategorySystemId = category.SystemId;
            _selectedStructureId.Add(category.SystemId);

            var market = _marketService.Get(_channel.Channel.MarketSystemId.Value);
            var firstLevelCategories = _categoryService.GetChildCategories(Guid.Empty, market.AssortmentSystemId)
                                                       .Where(x => x.IsPublished(_channel.SystemId) 
                                                                   && _authorizationService.HasOperation<Category>(Operations.Entity.Read, x.SystemId)
                                                                   && _renderingValidators.Validate(x));

            var currentCategoryParentSystemId = category.ParentCategorySystemId;
            return new SubNavigationLinkModel
            {
                IsSelected = true,
                Name = _website.Texts.GetValue("ProductCategories") ?? "Product Categories",
                Links = firstLevelCategories
                            .Where(x => currentCategoryParentSystemId == Guid.Empty || _selectedStructureId.Contains(x.SystemId))
                            .Select(x =>
                            {
                                var showAll = currentCategoryParentSystemId == Guid.Empty && _selectedStructureId.Contains(x.SystemId);
                                return new SubNavigationLinkModel
                                {
                                    IsSelected = _selectedStructureId.Contains(x.SystemId),
                                    Name = x.Localizations.CurrentCulture.Name,
                                    Url = x.GetUrl(_channel.SystemId),
                                    Links = _selectedStructureId.Contains(x.SystemId) ?
                                    GetChildLinks(x, showAll, showAll ? 0 : 1).ToList() :
                                    (x.GetChildren().Any() ? new List<SubNavigationLinkModel>() : null)
                                };
                            }).ToList()
            };
        }

        private IEnumerable<SubNavigationLinkModel> GetChildLinks(Category category, bool showAll = false, int level = int.MaxValue)
        {
            var res = new List<SubNavigationLinkModel>();

            foreach (var child in category.GetChildren()
                                          .Where(pg => pg.IsPublished(_channel.SystemId) 
                                                       && _authorizationService.HasOperation<Category>(Operations.Entity.Read, pg.SystemId)
                                                       && _renderingValidators.Validate(pg)))
            {
                if (showAll || _selectedStructureId.Contains(child.SystemId))
                {
                    var link = new SubNavigationLinkModel
                    {
                        Name = child.Localizations.CurrentCulture.Name,
                        Url = child.GetUrl(_channel.SystemId),
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

        private SubNavigationLinkModel CreatePageNavigation()
        {
            _currentSelectedPageId = _page.SystemId;
            _selectedStructureId = _page.GetAncestors().Select(c => c.SystemId).ToList();

            if (!_selectedStructureId.Any())
            {
                return null; // startpage, don't select the menu
            }

            _selectedStructureId.Insert(0, _page.SystemId);

            var rootPage = _selectedStructureId.Count > 1
                ? _pageService.Get(_selectedStructureId.Skip(_selectedStructureId.Count - 2).First())
                : _page.Page;

            return new SubNavigationLinkModel()
            {
                IsSelected = _currentSelectedPageId == rootPage.SystemId,
                Name = rootPage.Localizations.CurrentUICulture.Name,
                Url = _urlService.GetUrl(rootPage, new PageUrlArgs(_channel.SystemId)),
                Links = GetChildLinks(rootPage).ToList()
            };
        }

        private IEnumerable<SubNavigationLinkModel> GetChildLinks(Page rootPage)
        {
            var pageTypeName = rootPage.GetPageType();
            if (pageTypeName == PageTemplateNameConstants.NewsList)
            {
                return GetNewsChildLinks(rootPage);
            }

            if (pageTypeName == PageTemplateNameConstants.BrandList)
            {
                return new List<SubNavigationLinkModel>();
            }

            return GetRegularChildLinks(rootPage).ToList();
        }

        private IEnumerable<SubNavigationLinkModel> GetNewsChildLinks(Page rootPage)
        {
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            searchQuery.PageSize = 5;
            searchQuery.PageNumber = 1;
            searchQuery.PageType = PageTemplateNameConstants.NewsList;
            searchQuery.PageSystemId = rootPage.SystemId;

            var searchResponse = _pageSearchService.Search(searchQuery);
            var searchResult = _pageSearchService.Transform(searchQuery, searchResponse);

            if(searchResult == null || searchResult.Total < 1)
            {
                return new List<SubNavigationLinkModel>();
            }

            return _pageService.Get(searchResult.Items.Value.Select(x => x.Id).ToList())
                               .Where(p => p.IsActive(_channel.SystemId))
                               .Select(child =>
                                   new SubNavigationLinkModel
                                   {
                                       Name = child.Localizations.CurrentUICulture.Name,
                                       Url = _urlService.GetUrl(child),
                                       IsSelected = _currentSelectedPageId == child.SystemId
                                   })
                               .ToList();
        }

        private IEnumerable<SubNavigationLinkModel> GetRegularChildLinks(Page rootPage)
        {
            var res = new List<SubNavigationLinkModel>();
            foreach (var child in _pageService
                .GetChildPages(rootPage.SystemId)
                .Where(x => x.Status == ContentStatus.Published 
                            && x.IsActive(_channel.SystemId) 
                            && _authorizationService.HasOperation<Page>(Operations.Entity.Read, x.SystemId)))
            {
                var link = new SubNavigationLinkModel
                {
                    Name = child.Localizations.CurrentUICulture.Name,
                    Url = _urlService.GetUrl(child),
                    IsSelected = _currentSelectedPageId == child.SystemId
                };

                if (_selectedStructureId.Contains(child.SystemId))
                {
                    link.Links = GetChildLinks(child).ToList();
                }
                else if (_pageService.GetChildPages(child.SystemId).Any(x => x.MapTo<PageModel>() != null))
                {
                    link.Links = new List<SubNavigationLinkModel>();
                }

                res.Add(link);
            }

            return res;
        }
    }
}
