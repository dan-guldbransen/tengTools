using System;
using System.Web.Mvc;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Websites;
using Litium.Web.Routing;

namespace Litium.Accelerator.Mvc.Routing
{
    public class RequestModelActionFilter : IActionFilter, IResultFilter
    {
        private static readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor = IoC.Resolve<RouteRequestLookupInfoAccessor>();
        private static readonly RequestModelAccessor _requestModelAccessor = new RequestModelAccessor();
        private static readonly RouteRequestInfoAccessor _routeRequestInfoAccessor = IoC.Resolve<RouteRequestInfoAccessor>();
        private static readonly CartAccessor _cartAccessor = IoC.Resolve<CartAccessor>();

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }

            var routeRequestLookupInfo = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo;
            if (routeRequestLookupInfo != null)
            {
                _requestModelAccessor.RequestModel = new RequestModelImpl(_cartAccessor)
                {
                    _channelModel = new Lazy<ChannelModel>(() => routeRequestLookupInfo.Channel.MapTo<ChannelModel>()),
                    _searchQuery = new Lazy<SearchQuery>(() => filterContext.HttpContext.MapTo<SearchQuery>()),
                    _currentPageModel = new Lazy<PageModel>(() => _routeRequestInfoAccessor.RouteRequestInfo.PageSystemId.MapTo<PageModel>()),
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }

            _requestModelAccessor.RequestModel = null;
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
