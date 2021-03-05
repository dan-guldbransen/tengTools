using System;
using System.Linq;
using System.Web.Http;
using Litium.Accelerator.Builders.Framework;
using Litium.Accelerator.Mvc.Attributes;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Conditions;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Security;
using Litium.Web.Routing;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [RoutePrefix("api/cart")]
    public class CartController : ApiControllerBase
    {
        private readonly CartViewModelBuilder _cartViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly ModuleECommerce _moduleECommerce;

        public CartController(CartViewModelBuilder miniCartViewModelBuilder, RequestModelAccessor requestModelAccessor, RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor, ModuleECommerce moduleECommerce)
        {
            _cartViewModelBuilder = miniCartViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _moduleECommerce = moduleECommerce;
        }

        /// <summary>
        /// Gets the current shopping cart.
        /// </summary>
        [Route]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(_cartViewModelBuilder.Build(Cart));
        }

        /// <summary>
        /// Adds an article to the current shopping cart.
        /// </summary>
        /// <param name="model">
        ///     Object containing the variant system identifier, language that is used when the product is
        ///     published on the current web site and quantity of the article.
        /// </param>
        [HttpPost]
        [Route("add")]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Add(AddToCartViewModel model)
        {
            if (model?.Quantity >= 0)
            {
                Cart.Add(model.ArticleNumber, model.Quantity, string.Empty, _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.Channel.WebsiteLanguageSystemId ?? Guid.Empty);
            }

            return Ok(_cartViewModelBuilder.Build(Cart));
        }

        /// <summary>
        /// Adds all articles from an order to the current shopping cart.
        /// </summary>
        /// <param name="model">Object containing the order system identifier that is used to get the articles.</param>
        [HttpPost]
        [Route("reorder")]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Reorder(ReorderRequestViewModel model)
        {
            if (model?.OrderId != null && model.OrderId != Guid.Empty)
            {
                var order = _moduleECommerce.Orders.GetOrder(model.OrderId, Solution.Instance.SystemToken);
                foreach (var orderRow in order.OrderRows)
                {
                    var languageSystemId = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.Channel.WebsiteLanguageSystemId ?? Guid.Empty;
                    Cart.Add(orderRow.ArticleNumber, orderRow.Quantity, string.Empty, languageSystemId);
                }
            }

            return Ok(_cartViewModelBuilder.Build(Cart));
        }

        /// <summary>
        /// Updates the quantity of an article and refresh the current shopping cart.
        /// </summary>
        /// <param name="model">Object containing the article number and the quantity.</param>
        [HttpPut]
        [Route("update")]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult Update(OrderRowViewModel model)
        {
            if (model.Quantity >= 0)
            {
                Cart.UpdateRowQuantity(model.RowSystemId, model.Quantity);
                Cart.UpdateChangedRows();
                if (model.Quantity <= 0 && !string.IsNullOrEmpty(Cart.OrderCarrier.CampaignInfo) && ClearCampaignCode())
                {
                    // Campaign code must be cleared if order doesn't have applied voucher code.
                    Cart.OrderCarrier.CampaignInfo = string.Empty;
                    Cart.UpdateChangedRows();
                }
            }
            
            return Ok(_cartViewModelBuilder.Build(Cart));
        }

        private bool CampaignHasVoucherCodeCondition(Guid campaignId)
        {
            return _moduleECommerce.Campaigns.GetCampaign(campaignId, SecurityToken.CurrentSecurityToken).ConditionInfos.Any(x => x.TypeName == typeof(VoucherCodeCondition).FullName);
        }

        private bool ClearCampaignCode()
        {
            if (Cart.OrderCarrier.OrderRows.Where(x => !x.CarrierState.IsMarkedForDeleting && x.CampaignID != Guid.Empty).Any(y => CampaignHasVoucherCodeCondition(y.CampaignID)))
            {
                return false;
            }
            if (Cart.OrderCarrier.Deliveries.Where(x => !x.CarrierState.IsMarkedForDeleting && x.CampaignID != Guid.Empty).Any(y => CampaignHasVoucherCodeCondition(y.CampaignID)))
            {
                return false;
            }
            if (Cart.OrderCarrier.Fees.Where(x => !x.CarrierState.IsMarkedForDeleting && x.CampaignID != Guid.Empty).Any(y => CampaignHasVoucherCodeCondition(y.CampaignID)))
            {
                return false;
            }
            if (Cart.OrderCarrier.OrderDiscounts.Where(x => !x.CarrierState.IsMarkedForDeleting && x.CampaignID != Guid.Empty).Any(y => CampaignHasVoucherCodeCondition(y.CampaignID)))
            {
                return false;
            }

            return true;
        }

        private Cart Cart => _requestModelAccessor.RequestModel.Cart;
    }
}
