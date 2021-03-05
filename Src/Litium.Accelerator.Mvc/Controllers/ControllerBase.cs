using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Litium.Accelerator.Builders.Menu;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web.Routing;
using Litium.Web.Runtime;

namespace Litium.Accelerator.Mvc.Controllers
{
    /// <summary>
    /// Controller base class that helps out to set correct layout for rendering.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public abstract class ControllerBase : Controller
    {
        [FromService]
        public MenuViewModelBuilder MenuViewModelBuilder { get; set; }

        [FromService]
        public CartAccessor CartAccessor { get; set; }

        [FromService]
        public RouteRequestLookupInfoAccessor RouteRequestLookupInfoAccessor { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var cart = CartAccessor.Cart;
            if (cart.HasOrderCarrier)
            {
                var routeRequestLookupInto = RouteRequestLookupInfoAccessor.RouteRequestLookupInfo;
                var channel = routeRequestLookupInto.Channel;
                
                if (cart.OrderCarrier.ChannelID != channel.SystemId)
                {
                    var countryId = cart.OrderCarrier?.CountryID;
                    //if cart have no country then using the first country link of request lookup channel
                    if (countryId == null || countryId == Guid.Empty)
                    {
                        countryId = channel.CountryLinks.FirstOrDefault()?.CountrySystemId;
                    }

                    cart.SetChannel(
                        channel,
                        countryId.MapTo<Country>(),
                        SecurityToken.CurrentSecurityToken);
                }
            }
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (string.IsNullOrEmpty(masterName))
            {
                var menuModel = MenuViewModelBuilder.Build();
                masterName = menuModel.ShowLeftColumn
                    ? "~/Views/Shared/_LayoutWithLeftColumn.cshtml"
                    : "~/Views/Shared/_Layout.cshtml";
            }
            return base.View(viewName, masterName, model);
        }
    }
}