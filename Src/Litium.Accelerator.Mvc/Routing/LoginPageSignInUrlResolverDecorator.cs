using Litium.Accelerator.Caching;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Routing;
using System.Net;
using System.Threading;
using System.Web;

namespace Litium.Accelerator.Mvc.Routing
{
    [ServiceDecorator(typeof(ISignInUrlResolver))]
    internal class LoginPageSignInUrlResolverDecorator : ISignInUrlResolver
    {
        private readonly UrlService _urlService;
        private readonly PageByFieldTemplateCache<LoginPageByFieldTemplateCache> _pageByFieldType;
        private readonly ISignInUrlResolver _parentResolver;

        public LoginPageSignInUrlResolverDecorator(
            ISignInUrlResolver parentResolver,
            UrlService urlService,
            PageByFieldTemplateCache<LoginPageByFieldTemplateCache> pageByFieldType)
        {
            _parentResolver = parentResolver;
            _urlService = urlService;
            _pageByFieldType = pageByFieldType;
        }

        public bool TryGet(RouteRequestLookupInfo routeRequestLookupInfo, out string redirectUrl)
        {
            string resultUrl = null;
            if (_pageByFieldType.TryFindPage(page =>
            {
                var url = _urlService.GetUrl(page, new PageUrlArgs(routeRequestLookupInfo.Channel.SystemId));
                if (url == null)
                {
                    return false;
                }
                resultUrl = string.Concat(url, url.Contains("?") ? "&" : "?", "RedirectUrl=", HttpUtility.UrlEncode(routeRequestLookupInfo.RawUrl));
                var statusCode = HttpContext.Current?.Response.StatusCode;
                if (Thread.CurrentPrincipal.Identity.IsAuthenticated || statusCode == (int)HttpStatusCode.Unauthorized)
                {
                    resultUrl += $"&code={statusCode}";
                }
                return true;
            }))
            {
                redirectUrl = resultUrl;
                return true;
            }

            return _parentResolver.TryGet(routeRequestLookupInfo, out redirectUrl);
        }
    }
}