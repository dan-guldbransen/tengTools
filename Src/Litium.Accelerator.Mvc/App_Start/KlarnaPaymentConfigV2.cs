using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Mvc.Controllers.Checkout;
using Litium.Accelerator.Payments;
using Litium.Accelerator.Routing;
using Litium.Studio.AddOns.Klarna;
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
using Litium.AddOns.Klarna.Abstractions;
using Litium.Owin.InversionOfControl;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Runtime.DistributedLock;

namespace Litium.Accelerator.Mvc.App_Start
{
    [Service(ServiceType = typeof(KlarnaPaymentConfigV2))]
    public class KlarnaPaymentConfigV2
    {
        private readonly IErrorPageResolver _errorPageResolver;
        private readonly WebsiteService _websiteService;
        private readonly ChannelService _channelService;
        private readonly LanguageService _languageService;

        public KlarnaPaymentConfigV2(IErrorPageResolver errorPageResolver,
            WebsiteService websiteService,
            ChannelService channelService,
            LanguageService languageService)
        {
            _errorPageResolver = errorPageResolver;
            _websiteService = websiteService;
            _channelService = channelService;
            _languageService = languageService;
        }

        /// <summary>
        ///     Gets the checkout options.
        ///     Set the stylling and checkout iframe options using CheckoutOptions object below.
        ///     You may also set the AdditionalCheckbox option here.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>CheckoutOptions.</returns>
        public CheckoutOptions GetCheckoutOptions(OrderCarrier order)
        {
            var klarnaCheckoutOrder = new CheckoutOptions
            {
                AdditionalCheckBoxText = "Newsletter",
                AdditionalCheckBoxChecked = false,
                //AdditionalCheckBoxRequired = true,

                //set the button color to color of accelerator buttons.
                ColorButton = "#ff69b4",
            };

            //External payment methods should be configured for each Merchant account by Klarna before they are used.
            AddCashOnDeliveryExternalPaymentMethod(order, klarnaCheckoutOrder);

            return klarnaCheckoutOrder;
        }

        /// <summary>
        ///     Updates the data sent to klarna.
        ///     if the project has specific data to be sent to Klarna, outside the order carrier,
        ///     or has them as additional order info, and is not handled by the Klarna addOn, modify the klarnaCheckoutOrder
        ///     parameter.
        ///     it is the klarnaCheckoutOrder object which is a "KlarnaCheckoutOrder" that will be sent to Klarna checkout api at
        ///     Klarna.
        ///     the format of klarnaCheckoutOrder parameter is described in Klarna API documentation https://developers.klarna.com.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="klarnaCheckoutOrder">The data sent to klarna.</param>
        /// <exception cref="System.NotImplementedException"></exception>
#pragma warning disable IDE0060 // Remove unused parameter
        public void UpdateDataSentToKlarna(OrderCarrier orderCarrier, Dictionary<string, object> klarnaCheckoutOrder)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //Example:
            //Following test code modify the Klarna article number.

            //if (klarnaCheckoutOrder.ContainsKey("cart"))
            //{
            //    var cart = klarnaCheckoutOrder["cart"] as Dictionary<string, object>;
            //    if (cart != null)
            //    {
            //        if (cart.ContainsKey("items"))
            //        {
            //            var klarnaCartItems = cart["items"] as List<Dictionary<string, object>>;
            //            if (klarnaCartItems != null)
            //            {
            //                foreach (var klarnaOrderRow in klarnaCartItems)
            //                {
            //                    klarnaOrderRow["reference"] = "U-" + klarnaOrderRow["reference"];
            //                }
            //            }
            //        }
            //    }
            //}
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
            //var order = kcoOrder as CheckoutOrder;
            //var customer = order.OrderDetailsRaw["customer"] as JObject;
            //if (customer != null)
            //{
            //    var dateOfBirth = customer["date_of_birth"];
            //    if (dateOfBirth != null)
            //    {
            //        AddOrUpdateAdditionalOrderInfo(order.OrderCarrier, "DateOfBirth", (string)dateOfBirth);
            //    }
            //}

            var channel = _channelService.Get(order.OrderCarrier.ChannelID);
            var language = channel.WebsiteLanguageSystemId.HasValue ? _languageService.Get(channel.WebsiteLanguageSystemId.Value) : null;
            CultureInfo.CurrentUICulture = language != null ? language.CultureInfo : CultureInfo.CurrentUICulture;
            var routeRequestLookupInfo = new RouteRequestLookupInfo()
            {
                Channel = channel,
                IsSecureConnection = true,
            };
            _errorPageResolver.TryGet(routeRequestLookupInfo, out var routeRequestInfo);

            //following result is used by Klarna AddOn to send the validation result back to Klarna.
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
        /// <param name="orderCarrier">The checkout order.</param>
        /// <param name="options">The checkout options</param>
        private void AddCashOnDeliveryExternalPaymentMethod(OrderCarrier orderCarrier, CheckoutOptions options)
        {
            var checkoutPage = _websiteService.Get(orderCarrier.WebSiteID)?.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage)?.EntitySystemId.MapTo<Page>();
            if (checkoutPage == null)
            {
                return;
            }

            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var redirectUrl = urlHelper.Action(checkoutPage, routeValues: new { action = nameof(CheckoutController.PlaceOrderDirect) });
            var routeValues = new
            {
                PaymentProvider = "DirectPay",
                PaymentMethod = "DirectPayment",
                RedirectUrl = redirectUrl
            };

            var cashOnDeliveryExternalPayment = new ExternalPaymentMethod
            {
                Description = "Direct payment description",
                Fee = 0,
                Name = "Postförskott",
                RedirectUrl = new Uri(urlHelper.Action("ChangePaymentMethod", "KlarnaPayment", routeValues, Uri.UriSchemeHttps)).AbsoluteUri,
            };

            options.ExternalPaymentMethods = new List<ExternalPaymentMethod>
            {
                cashOnDeliveryExternalPayment
            };
        }

        internal class KlarnaWidgetV2 : IPaymentWidget<KlarnaProvider>
        {
            private readonly IPaymentInfoCalculator _paymentInfoCalculator;
            private readonly KlarnaPaymentConfigV2 _paymentConfig;
            private readonly PaymentWidgetService _paymentWidgetService;
            private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
            private readonly RequestModelAccessor _requestModelAccessor;
            private readonly SecurityToken _securityToken;
            private readonly UrlService _urlService;
            private readonly PageService _pageService;
            private readonly DistributedLockService _distributedLockService;

            public KlarnaWidgetV2(
                IPaymentInfoCalculator paymentInfoCalculator,
                PaymentWidgetService paymentWidgetService,
                KlarnaPaymentConfigV2 paymentConfig,
                RequestModelAccessor requestModelAccessor,
                RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
                SecurityToken securityToken,
                UrlService urlService,
                PageService pageService,
                DistributedLockService distributedLockService)
            {
                _paymentInfoCalculator = paymentInfoCalculator;
                _paymentConfig = paymentConfig;
                _paymentWidgetService = paymentWidgetService;
                _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
                _requestModelAccessor = requestModelAccessor;
                _securityToken = securityToken;
                _urlService = urlService;
                _pageService = pageService;
                _distributedLockService = distributedLockService;
            }

            private Cart Cart => _requestModelAccessor.RequestModel.Cart;

            public PaymentWidgetResult GetWidget(OrderCarrier order, PaymentInfoCarrier paymentInfo)
            {
                var paymentAccountId = _paymentWidgetService.GetPaymentAccountId(paymentInfo.PaymentMethod);
                var klarnaCheckout = StudioKlarnaCheckoutApi.CreateFrom(paymentAccountId, _paymentConfig.UpdateDataSentToKlarna);

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
                            Id = nameof(PaymentMethod.KLARNA_CHECKOUT),
                            IsChangeable = true,
                            ResponseString = checkoutOrder.HtmlSnippet
                        };
                    case KlarnaOrderStatus.Error:
                        throw new Exception(checkoutOrder.HtmlSnippet);
                    case KlarnaOrderStatus.Authorized:
                    case KlarnaOrderStatus.Cancelled:
                    case KlarnaOrderStatus.Captured:
                    case KlarnaOrderStatus.Complete:
                    case KlarnaOrderStatus.Created:
                        Cart.Clear();
                        return new PaymentWidgetResult
                        {
                            Id = nameof(PaymentMethod.KLARNA_CHECKOUT),
                            IsChangeable = false,
                            ResponseString = checkoutOrder.HtmlSnippet
                        };
                }

                throw new Exception(checkoutOrder.HtmlSnippet);
            }

            bool IPaymentWidget.IsEnabled(string paymentMethod)
            {
                return paymentMethod.EndsWith(nameof(PaymentMethod.KLARNA_CHECKOUT), StringComparison.OrdinalIgnoreCase);
            }

            public KlarnaPaymentArgs CreatePaymentArgs(OrderCarrier order, string paymentAccountId)
            {
                var tlsUsage = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsSecureConnection;
                if (!tlsUsage)
                {
                    this.Log().Trace("Klarna Checkout Validation is disabled. To enable the validate you need to use https on the checkout page.");
                }

                var checkoutFlowInfo = Cart.CheckoutFlowInfo;
                checkoutFlowInfo.ExecutePaymentMode = ExecutePaymentMode.Reserve;
                checkoutFlowInfo.RequireConsumerConfirm = false;

                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                var checkoutPage = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
                var checkoutPageUrl = GetAbsolutePageUrl(checkoutPage);
                var checkoutPageEntity = checkoutPage.EntitySystemId.MapTo<PageModel>();
                var termsAndConditionPage = checkoutPageEntity.GetValue<PointerPageItem>(CheckoutPageFieldNameConstants.TermsAndConditionsPage);
                var termsAndConditionPageUrl = GetAbsolutePageUrl(termsAndConditionPage);

                var channelUri = new Uri(_urlService.GetUrl(_requestModelAccessor.RequestModel.ChannelModel.Channel, new ChannelUrlArgs() { AbsoluteUrl = true }));
                var channelUrl = channelUri.AbsoluteUri;
                var confirmationUrl = new Uri(channelUri, urlHelper
                    .Action("Confirmation", "KlarnaPayment", new { AccountId = paymentAccountId, TransactionNumber = "{checkout.order.id}" }, Uri.UriSchemeHttps))?
                    .AbsoluteUri
                    .Replace("%7B", "{").Replace("%7D", "}");
                var validateUrl = new Uri(channelUri, urlHelper
                    .Action("Validate", "KlarnaPayment", new { AccountId = paymentAccountId, TransactionNumber = "{checkout.order.id}" }, Uri.UriSchemeHttps))?
                    .AbsoluteUri
                    .Replace("%7B", "{").Replace("%7D", "}");
                var pushUrl = new Uri(channelUri, urlHelper
                    .Action("PushNotification", "KlarnaPayment", new { TransactionNumber = "{checkout.order.id}" }, Uri.UriSchemeHttps))?
                    .AbsoluteUri
                    .Replace("%7B", "{").Replace("%7D", "}");

                var args = new KlarnaPaymentArgs
                {
                    CustomerPersonalNumber = string.Empty, //no ssn.
                    KlarnaCampaignCode = -1,
                    ExecuteScript = null, //not used
                    TermsUrl = termsAndConditionPageUrl,
                    ConfirmationUrl = confirmationUrl,
                    PushNotificationUrl = pushUrl,
                    CheckoutUrl = checkoutPageUrl, //if cancelled, go back to checkout page.
                    ClientLanguage = CultureInfo.CurrentUICulture.Name,
                    BackToStoreUrl = channelUrl,
                    ValidationUrl = tlsUsage ? validateUrl : null,
                    UserHostAddress = StudioKlarnaApi.GetClientIP(),
                    PaymentMode = ExecutePaymentMode.Reserve,
                    KlarnaCheckoutOptions = _paymentConfig.GetCheckoutOptions(order)
                };
                return args;
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

        [Plugin("Klarna")]
        internal class KlarnaPaymentArgsCreatorOverrides : KlarnaPaymentArgsCreator
        {
            /// <summary>
            /// Creates the payment args.
            /// </summary>
            /// <param name="checkoutFlowInfo">The checkout flow info.</param>
            /// <returns>Instance of ExecutePaymentArgs</returns>
            public override ExecutePaymentArgs CreatePaymentArgs(CheckoutFlowInfo checkoutFlowInfo)
            {
                var isCallback = checkoutFlowInfo.GetValue<bool?>("ProviderOrderIsCreated");
                return isCallback != null && isCallback.Value
                    ? new KlarnaPaymentArgs()
                    : base.CreatePaymentArgs(checkoutFlowInfo);
            }
        }
    }
}
