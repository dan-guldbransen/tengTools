using Litium.Accelerator.Mvc.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Websites;
using Litium.Web.Routing;
using Litium.Websites;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Litium.Web.Products.Routing;

namespace Litium.Accelerator.Mvc.Routing
{
    public class RequestModelHandler : DelegatingHandler
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
        private readonly ISecureConnectionResolver _secureConnectionResolver;
        private readonly ChannelService _channelService;
        private readonly DomainNameService _domainNameService;
        private readonly LanguageService _languageService;
        private readonly CartAccessor _cartAccessor;
        private readonly PageService _pageService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly UrlService _urlService;

        public RequestModelHandler(
            ISecureConnectionResolver secureConnectionResolver,
            ChannelService channelService,
            DomainNameService domainNameService,
            LanguageService languageService,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            RouteRequestInfoAccessor routeRequestInfoAccessor,
            CartAccessor cartAccessor,
            RequestModelAccessor requestModelAccessor,
            PageService pageService,
            FieldTemplateService fieldTemplateService,
            UrlService urlService)
        {
            _secureConnectionResolver = secureConnectionResolver;
            _channelService = channelService;
            _domainNameService = domainNameService;
            _languageService = languageService;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _routeRequestInfoAccessor = routeRequestInfoAccessor;
            _cartAccessor = cartAccessor;
            _requestModelAccessor = requestModelAccessor;
            _pageService = pageService;
            _fieldTemplateService = fieldTemplateService;
            _urlService = urlService;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var siteSettingViewModel = request.Headers.GetSiteSettingViewModel();
            if (siteSettingViewModel != null)
            {
                var url = request.RequestUri;
                var channel = _channelService.Get(siteSettingViewModel.ChannelSystemId);
                var domainNameLink = channel?.DomainNameLinks.FirstOrDefault();

                _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = new RouteRequestLookupInfo
                {
                    AbsolutePath = HttpUtility.UrlDecode(url.AbsolutePath),
                    IsSecureConnection = _secureConnectionResolver.IsUsingSecureConnection(),
                    QueryString = url.ParseQueryString(),
                    RawUrl = url.PathAndQuery,
                    Uri = url,
                    Channel = channel,
                    DomainNameLink = domainNameLink,
                    DomainName = _domainNameService.Get(domainNameLink?.DomainNameSystemId ?? Guid.Empty),
                    IsInAdministration = siteSettingViewModel.PreviewPageData != null,
                    PreviewPageData = siteSettingViewModel.PreviewPageData
                };

                if (siteSettingViewModel.CurrentPageSystemId != Guid.Empty)
                {
                    var page = _pageService.Get(siteSettingViewModel.CurrentPageSystemId);
                    if (page != null)
                    {
                        var fieldTemplate = _fieldTemplateService.Get<PageFieldTemplate>(page.FieldTemplateSystemId);
                        _routeRequestInfoAccessor.RouteRequestInfo = new RouteRequestInfo
                        {
                            PageSystemId = siteSettingViewModel.CurrentPageSystemId,
                            TemplateFileName = fieldTemplate.TemplatePath,
                            DataPath = _urlService.GetUrl(page, new PageUrlArgs(channel.SystemId))
                        };
                    }
                }

                if (siteSettingViewModel.ProductCategorySystemId != null)
                {
                    if (!(_routeRequestInfoAccessor.RouteRequestInfo.Data is ProductPageData productData))
                    {
                        _routeRequestInfoAccessor.RouteRequestInfo.Data = productData = new ProductPageData();
                    }
                    productData.CategorySystemId = siteSettingViewModel.ProductCategorySystemId.Value;
                }

                _requestModelAccessor.RequestModel = new RequestModelImpl(_cartAccessor)
                {
                    _channelModel = new Lazy<ChannelModel>(() => channel.MapTo<ChannelModel>()),
                    _searchQuery = new Lazy<SearchQuery>(() => request.MapTo<SearchQuery>()),
                    _currentPageModel = new Lazy<PageModel>(() => siteSettingViewModel.CurrentPageSystemId.MapTo<PageModel>()),
                };

                CultureInfo.CurrentUICulture = _languageService.Get(channel.WebsiteLanguageSystemId.GetValueOrDefault())?.CultureInfo;
                CultureInfo.CurrentCulture = _languageService.Get(channel.ProductLanguageSystemId.GetValueOrDefault())?.CultureInfo ?? CultureInfo.CurrentUICulture;
                request.RegisterForDispose(new AccessorCleanup(_requestModelAccessor, _routeRequestLookupInfoAccessor, _routeRequestInfoAccessor));
            }

            return base.SendAsync(request, cancellationToken);
        }

        private class AccessorCleanup : IDisposable
        {
            private readonly RequestModelAccessor _requestModelAccessor;
            private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
            private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;

            public AccessorCleanup(
                RequestModelAccessor requestModelAccessor,
                RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
                RouteRequestInfoAccessor routeRequestInfoAccessor)
            {
                _requestModelAccessor = requestModelAccessor;
                _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
                _routeRequestInfoAccessor = routeRequestInfoAccessor;
            }

            public void Dispose()
            {
                _requestModelAccessor.RequestModel = null;
                _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = null;
                _routeRequestInfoAccessor.RouteRequestInfo = null;
            }
        }

        private class RequestModelImpl : RequestModel
        {
            public Lazy<ChannelModel> _channelModel;
            public Lazy<SearchQuery> _searchQuery;
            public Lazy<PageModel> _currentPageModel;

            public RequestModelImpl(CartAccessor cartAccessor) : base(cartAccessor)
            {
            }

            public override ChannelModel ChannelModel => _channelModel?.Value;
            public override SearchQuery SearchQuery => _searchQuery?.Value;
            public override PageModel CurrentPageModel => _currentPageModel?.Value;
        }
    }
}