using Litium.Accelerator.Mvc.Attributes;
using Litium.Accelerator.Services;
using Litium.Accelerator.Payments;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.AddOns.Klarna.Abstractions;
using Litium.AddOns.Klarna.ExtensionMethods;
using Litium.AddOns.Klarna.Kco;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using System;
using System.Linq;
using System.Web.Mvc;
using KlarnaV3 = Litium.AddOns.Klarna;
using KlarnaV2 = Litium.Studio.AddOns.Klarna;
using Litium.Accelerator.Mvc.App_Start;
using System.Collections.Generic;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Klarna.Rest.Core.Model;

namespace Litium.Accelerator.Mvc.Controllers.Checkout
{
    public class KlarnaPaymentController : Controller
    {
        private static readonly ISet<string> _klarnaProviders = new HashSet<string>(new[]
        {
            KlarnaV2.KlarnaProvider.ProviderName,
            KlarnaV3.KlarnaProvider.ProviderName
        }, StringComparer.OrdinalIgnoreCase);

        private readonly KlarnaPaymentConfigV2 _paymentConfigv2;
        private readonly KlarnaPaymentConfigV3 _paymentConfigv3;
        private readonly PaymentWidgetService _paymentWidgetService;
        private readonly CheckoutService _checkoutService;
        private readonly CartAccessor _cartAccessor;
        private readonly PaymentService _paymentService;

        public KlarnaPaymentController(
            KlarnaPaymentConfigV2 paymentConfigv2,
            KlarnaPaymentConfigV3 paymentConfigv3,
            CartAccessor cartAccessor,
            PaymentWidgetService paymentWidgetService,
            CheckoutService checkoutService,
            PaymentService paymentService)
        {
            _paymentConfigv2 = paymentConfigv2;
            _paymentConfigv3 = paymentConfigv3;
            _paymentWidgetService = paymentWidgetService;
            _checkoutService = checkoutService;
            _cartAccessor = cartAccessor;
            _paymentService = paymentService;
        }

        [HttpGet]
        public ActionResult Confirmation(string accountId, string transactionNumber)
        {
            this.Log().Debug("Confirmation started accountId {accountId} transactionNumber {transactionNumber}.", accountId, transactionNumber);
            try
            {
                var klarnaCheckout = CreateKlarnaCheckoutApi(accountId);
                var widget = _paymentService.GetPaymentWidget(klarnaCheckout.PaymentProviderName);
                var checkoutOrder = klarnaCheckout.FetchKcoOrder(transactionNumber.Split('/').Last());
                if (checkoutOrder != null)
                {
                    _paymentWidgetService.PlaceOrder(new PaymentWidgetOrder(klarnaCheckout, checkoutOrder), true);
                    this.Log().Debug("Confirmation completed.");
                }
            }
            catch (Exception ex)
            {
                var logger = this.WebLog();
                logger.Append("account id", accountId);
                logger.Append("transaction number", transactionNumber);
                logger.Error(ex);
            }

            var redirectUrl = _checkoutService.GetOrderConfirmationPageUrl(_cartAccessor.Cart.OrderCarrier);
            return Redirect(redirectUrl);
        }

        [HttpPost]
        public ActionResult Validate(string accountId, string transactionNumber)
        {
            this.Log().Debug("Validate order, accountId {accountId} transactionNumber {transactionNumber}.", accountId, transactionNumber);
            var klarnaCheckout = CreateKlarnaCheckoutApi(accountId);

            switch (klarnaCheckout.PaymentProviderName)
            {
                case KlarnaV2.KlarnaProvider.ProviderName:
                    klarnaCheckout.ValidateKcoOrder(order => _paymentConfigv2.ValidateCheckoutOrder(order), Request, Response);
                    break;
                case KlarnaV3.KlarnaProvider.ProviderName:
                    klarnaCheckout.ValidateKcoOrder(order => _paymentConfigv3.ValidateCheckoutOrder(order), Request, Response);
                    break;
                default:
                    throw new Exception("Method does not implement validations for payment provider: " + klarnaCheckout.PaymentProviderName);
            }

            this.Log().Debug("Completed.");
            return Content("OK");
        }

        [HttpPost]
        public ActionResult AddressUpdate(string accountId, string transactionNumber, [FromJsonBody] CheckoutOrder checkoutOrderData)
        {
            this.Log().Debug("Address update started accountId {accountId} transactionNumber {transactionNumber}.", accountId, transactionNumber);
            _paymentConfigv3.AddressUpdate(checkoutOrderData);

            this.Log().Debug("Completed.");
            //if the correct Json result is not written to Klarna, the checkout snippet will show and error!.
            return new JsonResult { Data = checkoutOrderData };
        }

        [HttpGet]
        public ActionResult ChangePaymentMethod(string paymentProvider, string paymentMethod, string redirectUrl)
        {
            var cart = _cartAccessor.Cart;
            var paymentInfoCarrier = cart.OrderCarrier.PaymentInfo.Find(x => x.PaymentProvider.StartsWith("Klarna"));
            if (!string.IsNullOrEmpty(paymentInfoCarrier?.PaymentMethod))
            {
                var paymentAccountId = _paymentWidgetService.GetPaymentAccountId(paymentInfoCarrier.PaymentMethod);
                var klarnaCheckout = CreateKlarnaCheckoutApi(paymentAccountId);
                var checkoutOrder = klarnaCheckout.FetchKcoOrder(cart.OrderCarrier);
                if (checkoutOrder?.OrderCarrier != null)
                {
                    paymentInfoCarrier = checkoutOrder.OrderCarrier.PaymentInfo.Find(x => x.PaymentProvider.StartsWith("Klarna"));
                    cart.OrderCarrier = checkoutOrder.OrderCarrier;
                }
            }

            if (paymentInfoCarrier != null)
            {
                paymentInfoCarrier.PaymentProvider = paymentProvider;
                paymentInfoCarrier.PaymentMethod = paymentMethod;
            }
            return Redirect(redirectUrl);
        }

        [HttpPost]
        public ActionResult KlarnaOnChange(KlarnaOnChangeViewModel args)
        {
            var paymentInfoCarrier = _cartAccessor.Cart.OrderCarrier.PaymentInfo.First(x => _klarnaProviders.Contains(x.PaymentProvider));
            if (!string.IsNullOrEmpty(paymentInfoCarrier?.PaymentMethod))
            {
                var paymentAccountId = _paymentWidgetService.GetPaymentAccountId(paymentInfoCarrier.PaymentMethod);
                var klarnaCheckout = CreateKlarnaCheckoutApi(paymentAccountId);
                SetAddress(args, paymentInfoCarrier.BillingAddress);
                klarnaCheckout.CustomerDetailsChanged(paymentInfoCarrier.GetKlarnaOrderId(), new CustomerDetails
                {
                    Country = args.Country,
                    Email = args.Email,
                    FirstName = args.GivenName,
                    LastName = args.FamilyName,
                    Zip = args.PostalCode
                });
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult PushNotification(string accountId, string transactionNumber)
        {
            this.Log().Debug("Push notification started accountId {accountId} transactionNumber {transactionNumber}.", accountId, transactionNumber);
            var klarnaCheckout = CreateKlarnaCheckoutApi(accountId);
            var checkoutOrder = klarnaCheckout.FetchKcoOrder(transactionNumber);

            //to place the order, we need both the order carrier and the klarna checkout order.
            if (checkoutOrder?.OrderCarrier != null)
            {
                _paymentWidgetService.PlaceOrder(new PaymentWidgetOrder(klarnaCheckout, checkoutOrder), false);
                this.Log().Debug("Push notification completed.");
                return Content("OK");
            }
            else
            {
                this.Log().Debug("No order was found for push notification.");
                return Content("Error");
            }
        }

        private ILitiumKcoApi CreateKlarnaCheckoutApi(string merchantId)
        {
            return KlarnaV2.StudioKlarnaCheckoutApi.CreateFrom(merchantId, _paymentConfigv2.UpdateDataSentToKlarna)
                ?? LitiumKcoApi.CreateFrom(
                    merchantId, 
                    (order, payment, kcoOrder) => _paymentConfigv3.UpdateDataSentToKlarna(Url, order, payment, kcoOrder));
        }

        private void SetAddress(KlarnaOnChangeViewModel args, AddressCarrier addressCarrier)
        {
            if (addressCarrier != null)
            {
                addressCarrier.FirstName = args.GivenName;
                addressCarrier.LastName = args.FamilyName;
                addressCarrier.Email = args.Email;
                addressCarrier.Country = args.Country;
                addressCarrier.Zip = args.PostalCode;
            }
        }

        private class PaymentWidgetOrder : IPaymentWidgetOrder
        {
            private ILitiumKcoApi _klarnaCheckout;
            private ILitiumKcoOrder _checkoutOrder;

            public PaymentWidgetOrder(ILitiumKcoApi klarnaCheckout, ILitiumKcoOrder checkoutOrder)
            {
                _klarnaCheckout = klarnaCheckout;
                _checkoutOrder = checkoutOrder;
            }

            public string PaymentProviderName => _klarnaCheckout.PaymentProviderName;
            public string ProviderOrderId => _checkoutOrder.KlarnaId;
            public OrderCarrier OrderCarrier => _checkoutOrder.OrderCarrier;
            public bool IsCompleted => _checkoutOrder.KlarnaOrderStatus == KlarnaOrderStatus.Complete;
            public bool IsCreated => _checkoutOrder.KlarnaOrderStatus == KlarnaOrderStatus.Created;
            public string OrderStatus => _checkoutOrder.KlarnaOrderStatus.ToString();

            public void Refresh()
            {
                _checkoutOrder = _klarnaCheckout.FetchKcoOrder(_checkoutOrder.OrderCarrier);
            }

            public void Update(OrderCarrier orderCarrier)
            {
                _checkoutOrder = _klarnaCheckout.FetchKcoOrder(orderCarrier);
            }
        }
    }
}
