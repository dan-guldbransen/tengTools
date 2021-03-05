using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Mvc.Controllers.Checkout;
using Litium.Accelerator.Payments;
using Litium.Accelerator.Routing;
using Litium.AddOns.Klarna;
using Litium.AddOns.Klarna.Abstractions;
using Litium.AddOns.Klarna.Configuration;
using Litium.AddOns.Klarna.Kco;
using Litium.AddOns.Klarna.PaymentArgs;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Payments;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Mvc;
using Litium.Web.Routing;
using Litium.Web.Models.Websites;
using Litium.Websites;
using Litium.Runtime.DependencyInjection;
using Klarna.Rest.Core.Model;
using Litium.Runtime.DistributedLock;

namespace Litium.Accelerator.Mvc.App_Start
{
    [Service(ServiceType = typeof(KlarnaPaymentConfigV3))]
    public class KlarnaPaymentConfigV3
    {
        private readonly IErrorPageResolver _errorPageResolver;
        private readonly ChannelService _channelService;
        private readonly LanguageService _languageService;
        private readonly WebsiteService _websiteService;

        public KlarnaPaymentConfigV3(IErrorPageResolver errorPageResolver,
            ChannelService channelService,
            LanguageService languageService,
            WebsiteService websiteService)
        {
            _errorPageResolver = errorPageResolver;
            _channelService = channelService;
            _languageService = languageService;
            _websiteService = websiteService;
        }

        public void AddOrUpdateAdditionalOrderInfo(OrderCarrier orderCarrier, string key, string value)
        {
            var item = orderCarrier.AdditionalOrderInfo.Find(x => x.Key == key && !x.CarrierState.IsMarkedForDeleting);
            if (item == null)
            {
                orderCarrier.AdditionalOrderInfo.Add(new AdditionalOrderInfoCarrier(key, orderCarrier.ID, value));
            }
            else
            {
                if (item.Value != value)
                {
                    item.Value = value;
                }
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void AddressUpdate(CheckoutOrder checkoutOrderData)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // If the campaigns depend on address location, re-calculate the cart, after updating delivery addresses.
            // if you recalculate order totals, the checkoutOrderData must contain the correct order rows and order total values.
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void UpdateDataSentToKlarna(UrlHelper urlHelper, OrderCarrier orderCarrier, KlarnaPaymentArgs paymentArgs, CheckoutOrder klarnaCheckoutOrder)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //if the project has specific data to be sent to Klarna, outside the order carrier,
            //or has them as additional order info, and is not handled by the Klarna addOn, modify the klarnaCheckoutOrder parameter.
            //it is the klarnaCheckoutOrder object that will be sent to Klarna checkout api at Klarna.
            //the format of klarnaCheckoutOrder parameter is described in Klarna API documentation https://developers.klarna.com.

            //Set the checkout options here.
            klarnaCheckoutOrder.CheckoutOptions = new CheckoutOptions
            {
                AllowSeparateShippingAddress = true,
                ColorButton = "#ff69b4",
                DateOfBirthMandatory = true
            };

            //External payment methods should be configured for each Merchant account by Klarna before they are used.
            AddCashOnDeliveryExternalPaymentMethod(urlHelper, orderCarrier, klarnaCheckoutOrder);
        }

        /// <summary>
        ///     Validates the checkout order.
        ///     you may save additional order info into the order, during validations.
        ///     following is a sample which saves the date of birth as additional order info.
        /// </summary>
        /// <remarks>
        ///     The method need to return within 3 seconds to be able to cancel placed an order at Klarna.
        /// </remarks>
        /// <param name="order">The order.</param>
        /// <returns>ValidationResult.</returns>
        public ValidationResult ValidateCheckoutOrder(ILitiumKcoOrder order)
        {
            var kcoOrder = order as LitiumKcoOrder;
            if (!string.IsNullOrEmpty(kcoOrder?.CheckoutOrder?.CheckoutCustomer?.DateOfBirth))
            {
                AddOrUpdateAdditionalOrderInfo(order.OrderCarrier, "DateOfBirth", kcoOrder.CheckoutOrder.CheckoutCustomer.DateOfBirth);
            }

            var channel = _channelService.Get(order.OrderCarrier.ChannelID);
            var language = channel.WebsiteLanguageSystemId.HasValue ? _languageService.Get(channel.WebsiteLanguageSystemId.Value) : null;
            CultureInfo.CurrentUICulture = language != null ? language.CultureInfo : CultureInfo.CurrentUICulture;
            var routeRequestLookupInfo = new RouteRequestLookupInfo()
            {
                Channel = channel,
                IsSecureConnection = true,
            };
            _errorPageResolver.TryGet(routeRequestLookupInfo, out var routeRequestInfo);

            // following result is used by Klarna AddOn to send the validation result back to Klarna.
            // ReSharper disable once ConvertToLambdaExpression
            return new ValidationResult
            {
                IsOrderValid = true,
                RedirectToUrlOnValidationFailure = routeRequestInfo.DataPath,
            };
        }

        /// <summary>
        ///     Adds the cash on delivery external payment method., by using Litium default "DirectPay" as the payment method.
        ///     Note: To use, Klarna has to configure the "Cash on delivery" external payment method for the merchant account.
        /// </summary>
        private void AddCashOnDeliveryExternalPaymentMethod(UrlHelper urlHelper, OrderCarrier orderCarrier, CheckoutOrder klarnaCheckoutOrder)
        {
            var checkoutPage = _websiteService.Get(orderCarrier.WebSiteID)?.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage)?.EntitySystemId.MapTo<Page>();
            var channel = _channelService.Get(orderCarrier.ChannelID);
            if (checkoutPage == null || channel == null)
            {
                return;
            }

            var redirectUrl = urlHelper.Action(checkoutPage, routeValues: new { action = nameof(CheckoutController.PlaceOrderDirect) }, channel: channel);

            var routeValues = new
            {
                PaymentProvider = "DirectPay",
                PaymentMethod = "DirectPayment",
                RedirectUrl = redirectUrl
            };

            var changePaymentProviderUrl = new Uri(urlHelper.Action("ChangePaymentMethod", "KlarnaPayment", routeValues, Uri.UriSchemeHttps)).AbsoluteUri;
            var cashOnDeliveryExternalPayment = new PaymentProvider
            {
                Name = "Cash on delivery",
                RedirectUrl = changePaymentProviderUrl,
                Fee = 0
            };

            klarnaCheckoutOrder.ExternalPaymentMethods = new List<PaymentProvider>
            {
                cashOnDeliveryExternalPayment
            };
        }

        public class KlarnaWidget : IPaymentWidget<KlarnaProvider>
        {
            private readonly IPaymentInfoCalculator _paymentInfoCalculator;
            private readonly KlarnaPaymentConfigV3 _paymentConfig;
            private readonly SecurityToken _securityToken;
            private readonly RequestModelAccessor _requestModelAccessor;
            private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
            private readonly UrlService _urlService;
            private readonly PaymentWidgetService _paymentWidgetService;
            private readonly PageService _pageService;
            private readonly CartAccessor _cartAccessor;
            private readonly DistributedLockService _distributedLockService;

            public KlarnaWidget(
                IPaymentInfoCalculator paymentInfoCalculator,
                KlarnaPaymentConfigV3 paymentConfig,
                SecurityToken securityToken,
                RequestModelAccessor requestModelAccessor,
                RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
                UrlService urlService,
                PaymentWidgetService paymentWidgetService,
                PageService pageService,
                CartAccessor cartAccessor,
                DistributedLockService distributedLockService)
            {
                _paymentInfoCalculator = paymentInfoCalculator;
                _paymentConfig = paymentConfig;
                _securityToken = securityToken;
                _requestModelAccessor = requestModelAccessor;
                _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
                _paymentWidgetService = paymentWidgetService;
                _urlService = urlService;
                _pageService = pageService;
                _cartAccessor = cartAccessor;
                _distributedLockService = distributedLockService;
            }

            public PaymentWidgetResult GetWidget(OrderCarrier order, PaymentInfoCarrier paymentInfo)
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                var paymentAccountId = _paymentWidgetService.GetPaymentAccountId(paymentInfo.PaymentMethod);
                var klarnaCheckout = LitiumKcoApi.CreateFrom(
                    paymentAccountId, 
                    (orderCarrier, payment, kcoOrder) => _paymentConfig.UpdateDataSentToKlarna(urlHelper, orderCarrier, payment, kcoOrder));

                var checkoutOrder = string.IsNullOrEmpty(paymentInfo.TransactionNumber) ? null : klarnaCheckout.FetchKcoOrder(order);
                if (checkoutOrder == null || checkoutOrder.KlarnaOrderStatus == KlarnaOrderStatus.Incomplete)
                {
                    using (_distributedLockService.AcquireLock($"{nameof(PaymentWidgetService)}:{order.ID}", TimeSpan.FromMinutes(1)))
                    {
                        _paymentInfoCalculator.CalculateFromCarrier(order, _securityToken);
                        var args = CreatePaymentArgs(order, paymentAccountId);

                        checkoutOrder = klarnaCheckout.CreateOrUpdateKcoOrder(order, args);
                    }
                }

                switch (checkoutOrder.KlarnaOrderStatus)
                {
                    case KlarnaOrderStatus.Incomplete:
                        return new PaymentWidgetResult
                        {
                            Id = nameof(PaymentMethod.KlarnaCheckout),
                            IsChangeable = true,
                            ResponseString = checkoutOrder.HtmlSnippet,
                        };
                    case KlarnaOrderStatus.Error:
                        throw new Exception(checkoutOrder.HtmlSnippet);
                    case KlarnaOrderStatus.Authorized:
                    case KlarnaOrderStatus.Cancelled:
                    case KlarnaOrderStatus.Captured:
                    case KlarnaOrderStatus.Complete:
                    case KlarnaOrderStatus.Created:
                        _cartAccessor.Cart.Clear();
                        return new PaymentWidgetResult
                        {
                            Id = nameof(PaymentMethod.KlarnaCheckout),
                            IsChangeable = false,
                            ResponseString = checkoutOrder.HtmlSnippet,
                        };
                }

                throw new Exception(checkoutOrder.HtmlSnippet);
            }

            bool IPaymentWidget.IsEnabled(string paymentMethod)
            {
                return paymentMethod.EndsWith(nameof(PaymentMethod.KlarnaCheckout), StringComparison.OrdinalIgnoreCase);
            }

#pragma warning disable IDE0060 // Remove unused parameter
            private ExecutePaymentArgs CreatePaymentArgs(OrderCarrier order, string paymentAccountId)
#pragma warning restore IDE0060 // Remove unused parameter
            {
                var tlsUsage = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsSecureConnection;
                if (!tlsUsage)
                {
                    this.Log().Trace("Klarna Checkout Validation is disabled. To enable the validate you need to use https on the checkout page.");
                }

                var checkoutFlowInfo = _cartAccessor.Cart.CheckoutFlowInfo;
                checkoutFlowInfo.ExecutePaymentMode = ExecutePaymentMode.Reserve;
                checkoutFlowInfo.RequireConsumerConfirm = false;

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                var checkoutPage = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
                var checkoutPageUrl = GetAbsolutePageUrl(checkoutPage);
                var checkoutPageEntity = checkoutPage.EntitySystemId.MapTo<PageModel>();
                var termsAndConditionPage = checkoutPageEntity.GetValue<PointerPageItem>(CheckoutPageFieldNameConstants.TermsAndConditionsPage);
                var termsAndConditionPageUrl = GetAbsolutePageUrl(termsAndConditionPage);

                var channelUri = new Uri(_urlService.GetUrl(_requestModelAccessor.RequestModel.ChannelModel.Channel, new ChannelUrlArgs() { AbsoluteUrl = true }));
                var confirmationUrl = new Uri(channelUri, urlHelper.Action("Confirmation", "KlarnaPayment", null, Uri.UriSchemeHttps));
                var validateUrl = new Uri(channelUri, urlHelper.Action("Validate", "KlarnaPayment", null, Uri.UriSchemeHttps));
                var pushUrl = new Uri(channelUri, urlHelper.Action("PushNotification", "KlarnaPayment", null, Uri.UriSchemeHttps));
                var updateAddressUrl = new Uri(channelUri, urlHelper.Action("AddressUpdate", "KlarnaPayment", null, Uri.UriSchemeHttps));

                checkoutFlowInfo.SetValue(ConstantsAndKeys.TermsUrlKey, termsAndConditionPageUrl);
                checkoutFlowInfo.SetValue(ConstantsAndKeys.CheckoutUrlKey, checkoutPageUrl);
                checkoutFlowInfo.SetValue(ConstantsAndKeys.ConfirmationUrlKey, confirmationUrl.AbsoluteUri);
                checkoutFlowInfo.SetValue(ConstantsAndKeys.PushUrlKey, pushUrl.AbsoluteUri);

                checkoutFlowInfo.SetValue(ConstantsAndKeys.ValidationUrlKey, validateUrl.AbsoluteUri);
                checkoutFlowInfo.SetValue(ConstantsAndKeys.AddressUpdateUrlKey, updateAddressUrl.AbsoluteUri);

                return new KlarnaPaymentArgsCreator().CreatePaymentArgs(checkoutFlowInfo);
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

                var channelSystemId = pointer.ChannelSystemId != Guid.Empty ? pointer.ChannelSystemId : _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.Channel.SystemId;
                return _urlService.GetUrl(page, new PageUrlArgs(channelSystemId) { AbsoluteUrl = true });
            }
        }
    }
}
