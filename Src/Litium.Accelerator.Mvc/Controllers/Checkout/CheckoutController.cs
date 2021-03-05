using Litium.Accelerator.Builders.Checkout;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Plugins.Utilities;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Runtime.AutoMapper;
using Litium.Studio.Extenssions;
using Litium.Web;
using Litium.Web.Models;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web.Mvc;

namespace Litium.Accelerator.Mvc.Controllers.Checkout
{
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutViewModelBuilder _checkoutViewModelBuilder;
        private readonly CheckoutService _checkoutService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly CartAccessor _cartAccessor;

        public CheckoutController(
            CheckoutViewModelBuilder checkoutViewModelBuilder,
            RequestModelAccessor requestModelAccessor,
            CheckoutService checkoutService,
            UrlService urlService,
            PageService pageService,
            ModuleECommerce moduleECommerce,
            CartAccessor cartAccessor)
        {
            _checkoutViewModelBuilder = checkoutViewModelBuilder;
            _checkoutService = checkoutService;
            _requestModelAccessor = requestModelAccessor;
            _urlService = urlService;
            _pageService = pageService;
            _moduleECommerce = moduleECommerce;
            _cartAccessor = cartAccessor;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = _checkoutViewModelBuilder.Build();

            if (!_checkoutService.ValidateOrder(out string message))
            {
                model.ErrorMessages.Add("cart", new List<string> { message });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult HandleResponse()
        {
            var callBackResult = _requestModelAccessor.RequestModel.Cart.PaymentProviderCallbackResult;
            if (callBackResult != null)
            {
                if (!callBackResult.Success)
                {
                    var msg = string.IsNullOrEmpty(callBackResult.ErrorMessage) ? string.Empty : $": {Server.HtmlEncode(callBackResult.ErrorMessage)}";
                    throw new CheckoutException(CheckoutConstants.StringPaymentUnsuccessful.AsWebSiteString() + msg);
                }
                return HandlePaymentResult(callBackResult.Success, callBackResult.ErrorMessage);
            }
            return null;
        }

        [HttpGet]
        public ActionResult HandleCancelResponse()
        {
            var checkoutPageUrl = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage).MapTo<LinkModel>()?.Href;
            return Redirect(checkoutPageUrl);
        }

        [HttpGet]
        public ActionResult PlaceOrderDirect()
        {
            if (!_checkoutService.ValidateOrder(out string message))
            {
                SetDefaultPaymentMethod();
                var model = _checkoutViewModelBuilder.Build();
                model.ErrorMessages.Add("general", new List<string> { message });
                return View("Index", model);
            }

            try
            {
                var checkoutPage = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
                var checkoutPageUrl = GetAbsolutePageUrl(checkoutPage);
                var responseUrl = new QueryString().Add(CheckoutConstants.QueryStringStep, CheckoutConstants.Response).ReplaceQueryString($"{checkoutPageUrl}.HandleResponse");
                var cancelUrl = new QueryString().Add(CheckoutConstants.QueryStringStep, CheckoutConstants.Cancel).ReplaceQueryString($"{checkoutPageUrl}.HandleCancelResponse");
                var executePaymentResult = _checkoutService.PlaceOrder(null, responseUrl, cancelUrl, out string redirectUrl);
                if (executePaymentResult == null)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = _checkoutService.HandlePaymentResult(executePaymentResult.Success, executePaymentResult.ErrorMessage);
                }

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                this.Log().Error("Error when placing an order", ex);
                SetDefaultPaymentMethod();
                var model = _checkoutViewModelBuilder.Build();
                model.ErrorMessages.Add("general", new List<string> { "checkout.generalerror".AsWebSiteString() });
                return View("Index", model);
            }
        }

        private ActionResult HandlePaymentResult(bool isSuccess, string errorMessage)
        {
            var redirectUrl = _checkoutService.HandlePaymentResult(isSuccess, errorMessage);
            return Redirect(redirectUrl);
        }

        private string GetAbsolutePageUrl(PointerPageItem pointer)
        {
            if (pointer == null)
            {
                return null;
            }
            var page = _pageService.Get(pointer.EntitySystemId);
            if (page == null)
            {
                return null;
            }

            var channelSystemId = pointer.ChannelSystemId != Guid.Empty ? pointer.ChannelSystemId : _requestModelAccessor.RequestModel.ChannelModel.SystemId;
            return _urlService.GetUrl(page, new PageUrlArgs(channelSystemId) { AbsoluteUrl = true });
        }

        public virtual void SetDefaultPaymentMethod()
        {
            var ids = _requestModelAccessor.RequestModel.ChannelModel?.Channel?.CountryLinks?.FirstOrDefault(x => x.CountrySystemId == _cartAccessor.Cart.OrderCarrier.CountryID)?.PaymentMethodSystemIds ?? Enumerable.Empty<Guid>();
            var paymentMethods = _moduleECommerce.PaymentMethods.GetAll().Where(x => ids.Contains(x.ID)).Select(x => { return string.Concat(x.PaymentProviderName, ":", x.Name);}).ToList();
            var payment = _requestModelAccessor.RequestModel.Cart.OrderCarrier?.PaymentInfo?.FirstOrDefault();
            // Set default payment in case if DirectPay payment doesn't exist for the channel
            if (payment != null && !paymentMethods.Contains(string.Concat(payment.PaymentProvider, ":", payment.PaymentMethod)))
            {
                _checkoutService.ChangePaymentMethod(paymentMethods.First());
            }
        }
    }
}
