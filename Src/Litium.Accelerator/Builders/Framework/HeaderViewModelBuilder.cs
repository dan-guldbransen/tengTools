using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Litium.Accelerator.Caching;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Data;
using Litium.Data.Queryable;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Products;
using Litium.Web.Models.Websites;
using Litium.Web.Routing;
using Litium.Websites;

namespace Litium.Accelerator.Builders.Framework
{
    public class HeaderViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : HeaderViewModel
    {
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageByFieldTemplateCache<LoginPageByFieldTemplateCache> _pageByFieldType;
        private readonly PageByFieldTemplateCache<MegaMenuPageFieldTemplateCache> _pageByFieldTypeMega;
        private readonly AuthorizationService _authorizationService;
        private readonly CategoryService _categoryService;
        private readonly PageService _pageService;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly DataService _dataService;
        private readonly List<Guid> _selectedStructureId = new List<Guid>();
        private readonly Guid _channelSystemId;

        public HeaderViewModelBuilder(
            UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            PageByFieldTemplateCache<LoginPageByFieldTemplateCache> pageByFieldType,
            PageByFieldTemplateCache<MegaMenuPageFieldTemplateCache> pageByFieldTypeMega,
        PageService pageService,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            DataService dataService,
            AuthorizationService authorizationService,
            CategoryService categoryService)
        {
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _pageByFieldType = pageByFieldType;
            _pageByFieldTypeMega = pageByFieldTypeMega;
            _pageService = pageService;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _dataService = dataService;
            _categoryService = categoryService;
            _authorizationService = authorizationService;
            _channelSystemId = _requestModelAccessor.RequestModel.ChannelModel.SystemId;
        }

        public HeaderViewModel Build()
        {
            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            var myPage = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.MyPagesPage)?.MapTo<LinkModel>();
            LinkModel loginPage = null;
            _pageByFieldType.TryFindPage(login =>
            {
                if (login == null)
                {
                    return false;
                }

                loginPage = CreateLoginLink(login.MapTo<LinkModel>());
                return loginPage != null;
            }, true);

            var getOrganisedPage = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.GetOrganisedPage)?.MapTo<LinkModel>();
            var externalB2Blink = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.ExternalB2BLink);

            var topLinkList = website.GetValue<IList<PointerItem>>(AcceleratorWebsiteFieldNameConstants.AdditionalHeaderLinks)?.OfType<PointerPageItem>().ToList().Select(x => x.MapTo<LinkModel>()).Where(x => x != null).ToList();
            myPage = myPage?.AccessibleByUser == true ? myPage : null;

            var startPage = _pageService.GetChildPages(Guid.Empty, website.SystemId).FirstOrDefault();
            var startPageUrl = _urlService.GetUrl(startPage, new PageUrlArgs(_requestModelAccessor.RequestModel.ChannelModel.SystemId));

            // Get categories
            var categories = new List<ContentLinkModel>();
            var megaMenuPages = GetMegaMenuPages();
            foreach(var megaMenuPage in megaMenuPages)
            {
                var linkName = megaMenuPage?.Page?.Localizations.CurrentCulture.Name;
                if (string.IsNullOrEmpty(linkName))
                {
                    continue;
                }
                var contentLinkModel = new ContentLinkModel
                {
                    Name = linkName
                };
                var categoryModel = megaMenuPage.GetValue<Guid?>(MegaMenuPageFieldNameConstants.MegaMenuCategory)?.MapTo<CategoryModel>();
                
                // If a category is chosen, the link will redirect to the category in the first place
                if (categoryModel != null)
                {
                    if (categoryModel.Category == null || !categoryModel.Category.IsPublished(_channelSystemId))
                    {
                        //Category must be published
                        continue;
                    }
                    contentLinkModel.Url = _urlService.GetUrl(categoryModel.Category);
                    contentLinkModel.IsSelected = _selectedStructureId.Contains(categoryModel.Category.SystemId);
                }

                categories.Add(contentLinkModel);

                if (megaMenuPage.GetValue<bool>(MegaMenuPageFieldNameConstants.MegaMenuShowContent))
                {
                    if (megaMenuPage.GetValue<bool>(MegaMenuPageFieldNameConstants.MegaMenuShowSubCategories))
                    {
                        // Show two levels of sub categories/pages
                        contentLinkModel.Links = categoryModel != null ? GetSubCategoryLinks(categoryModel, true) : new List<ContentLinkModel>();
                    }
                }
            }

            return new HeaderViewModel
            {
                Logo = website.GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.LogotypeMain)?.MapTo<ImageModel>(),
                TopLinkList = topLinkList ?? new List<LinkModel>(),
                LoginPage = loginPage ?? new LinkModel(),
                MyPage = myPage ?? new LinkModel(),
                StartPageUrl = string.IsNullOrWhiteSpace(startPageUrl) ? "/" : startPageUrl,
                IsLoggedIn = !Thread.CurrentPrincipal.Identity.IsAuthenticated,
                QuickSearch = new QuickSearchViewModel
                {
                    SearchTerm = _requestModelAccessor.RequestModel.SearchQuery.Text
                },
                GetOrganised = getOrganisedPage ?? new LinkModel(),
                ExternalB2BLink = externalB2Blink,
                Categories = categories
            };
        }

        private List<ContentLinkModel> GetSubCategoryLinks(CategoryModel categoryModel, bool showNextLevel)
        {
            var links = new List<ContentLinkModel>();
            foreach (var subCategory in _categoryService.GetChildCategories(categoryModel.Category.SystemId, Guid.Empty)
                                                        .Where(x => x.IsPublished(_channelSystemId) && _authorizationService.HasOperation<Category>(Operations.Entity.Read, x.SystemId))
                                                        .Select(x => x.MapTo<CategoryModel>()))
            {
                var subLinkModel = new ContentLinkModel
                {
                    Name = subCategory.Category.Localizations.CurrentCulture.Name,
                    IsSelected = _selectedStructureId.Contains(subCategory.Category.SystemId),
                    Url = _urlService.GetUrl(subCategory.Category)
                };
                if (showNextLevel)
                {
                    subLinkModel.Links = GetSubCategoryLinks(subCategory, false);
                }
                links.Add(subLinkModel);
            }

            return links;
        }

        private List<PageModel> GetMegaMenuPages()
        {
            var megaMenuPages = new List<PageModel>();
            _pageByFieldTypeMega.TryFindPage(page =>
            {
                if (page != null)
                {
                    var pageModel = page.MapTo<PageModel>();
                    // pageModel can be null if current user does not have access to it
                    // or if the page is not published on the current channel
                    if (pageModel != null)
                    {
                        megaMenuPages.Add(pageModel);
                    }
                }

                return false;
            });
            return megaMenuPages.OrderBy(p => p.Page.SortIndex).ToList();
        }

        private LinkModel CreateLoginLink(LinkModel loginPage)
        {
            if (loginPage == null || !loginPage.AccessibleByUser)
            {
                return null;
            }

            if (loginPage.Href != null)
            {
                loginPage.Href += string.Concat(loginPage.Href.Contains("?") ? "&" : "?", "RedirectUrl=", System.Web.HttpUtility.UrlEncode(_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.RawUrl));
            }
            return loginPage;
        }
    }
}
