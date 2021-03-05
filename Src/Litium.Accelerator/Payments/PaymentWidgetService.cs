using System;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Litium.Accelerator.Constants;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Runtime.DependencyInjection;
using Litium.Runtime.DistributedLock;
using Litium.Web.Customers.TargetGroups;
using Litium.Web.Customers.TargetGroups.Events;

namespace Litium.Accelerator.Payments
{
    [Service(ServiceType = typeof(PaymentWidgetService))]
    public class PaymentWidgetService
    {
        private readonly CartAccessor _cartAccessor;
        private readonly ChannelService _channelService;
        private readonly LanguageService _languageService;
        private readonly DistributedLockService _distributedLockService;
        private readonly TargetGroupEngine _targetGroupEngine;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly SecurityToken _securityToken;

        public PaymentWidgetService(
            SecurityToken securityToken,
            ModuleECommerce moduleECommerce,
            TargetGroupEngine targetGroupEngine,
            CartAccessor cartAccessor,
            ChannelService channelService,
            LanguageService languageService,
            DistributedLockService distributedLockService)
        {
            _cartAccessor = cartAccessor;
            _channelService = channelService;
            _languageService = languageService;
            _distributedLockService = distributedLockService;
            _securityToken = securityToken;
            _targetGroupEngine = targetGroupEngine;
            _moduleECommerce = moduleECommerce;
        }

        public string GetPaymentAccountId(string paymentMethod)
        {
            var paymentMethodParts = paymentMethod.Split(' ');
            if (paymentMethodParts.Length > 0)
            {
                return paymentMethodParts[0];
            }

            return null;
        }

        public void PlaceOrder([NotNull] IPaymentWidgetOrder paymentWidgetOrder, bool isConfirmation)
        {
            this.Log().Debug("Provider order status {providerOrderStatus}.", paymentWidgetOrder.OrderStatus);
            Order order = null;
            OrderCarrier orderCarrier = null;
            if (paymentWidgetOrder.IsCompleted)
            {
                using (_distributedLockService.AcquireLock($"{nameof(PaymentWidgetService)}:{paymentWidgetOrder.ProviderOrderId}", TimeSpan.FromMinutes(1)))
                {
                    //there is a time gap between when we last fetched order from Klarna to the time of aquiring the distributed lock.
                    //just before the distributed lock was taken by this machine (but after this machine had fetched checkout order from klarna),
                    //another machine in the server farm may have got the lock, and created the order and exited releasing the distributed lock.
                    //That would cause this machine to aquire the lock, but we cannot use the existing checkoutorder because its out-dated.
                    //therefore we have to refetch it one more time!.
                    paymentWidgetOrder.Refresh();

                    if (paymentWidgetOrder.IsCompleted)
                    {
                        //save the order only if it is not saved already. 
                        order = TryFindOrder();
                        if (order == null)
                        {
                            this.Log().Debug("Create order.");                            
                            // Replace the order carrier in the session from the one received from Klarna plugin.
                            // The order carrier contains the inforamtion of the order that will be created.
                            _cartAccessor.Cart.OrderCarrier = paymentWidgetOrder.OrderCarrier;

                            var channel = _channelService.Get(_cartAccessor.Cart.OrderCarrier.ChannelID);
                            if (channel?.WebsiteLanguageSystemId != null)
                            {
                                CultureInfo.CurrentUICulture = _languageService.Get(channel.WebsiteLanguageSystemId.Value)?.CultureInfo ?? CultureInfo.CurrentUICulture;
                            }

                            order = _cartAccessor.Cart.PlaceOrder(_securityToken, out var paymentInfos);
                            this.Log().Debug("Order created, order id {orderId}, external order id {externalOrderId}.", order.ID, order.ExternalOrderID);
                        }

                        var paymentInfo = order.PaymentInfo.FirstOrDefault(x => x.PaymentProviderName == paymentWidgetOrder.PaymentProviderName);
                        if (paymentInfo != null)
                        {
                            // If the Litium call to payment provider to "notify order created" fails, the payment will be in following states.
                            // so execute payment need to be called again, to notify klarna of Litium order id.
                            if (paymentInfo.PaymentStatus == PaymentStatus.Init
                                || paymentInfo.PaymentStatus == PaymentStatus.ExecuteReserve
                                || paymentInfo.PaymentStatus == PaymentStatus.Pending)
                            {
                                var checkoutFlowInfo = _cartAccessor.Cart.CheckoutFlowInfo;
                                checkoutFlowInfo.SetValue("ProviderOrderIsCreated", true);
                                var channel = _channelService.Get(order.ChannelID);
                                CultureInfo.CurrentUICulture = _languageService.Get(channel.WebsiteLanguageSystemId.GetValueOrDefault())?.CultureInfo ?? CultureInfo.CurrentUICulture;
                                checkoutFlowInfo.SetValue(CheckoutConstants.ClientLanguage, CultureInfo.CurrentUICulture.Name);
                                checkoutFlowInfo.SetValue(CheckoutConstants.ClientTwoLetterISOLanguageName, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

                                this.Log().Debug("Execute payment.");
                                var result = paymentInfo.ExecutePayment(checkoutFlowInfo, _securityToken);
                                checkoutFlowInfo.SetValue(CheckoutConstants.ExecutePaymentResult, result);
                                this.Log().Debug($"Payment executed{(result.Success ? string.Empty : " with error: " + result.ErrorMessage)}.");

                                // The order carrier has changed, refetch the checkoutOrder with updated data.
                                orderCarrier = order.GetAsCarrier(true, true, true, true, true, true);
                                paymentWidgetOrder.Update(orderCarrier);
                                this.Log().Debug("Provider order status changed to {providerOrderStatus}.", paymentWidgetOrder.OrderStatus);
                            }
                        }
                    }
                }
            }

            if (isConfirmation && paymentWidgetOrder.IsCreated)
            {
                var placedOrder = order ?? TryFindOrder();
                if (placedOrder != null)
                {
                    _cartAccessor.Cart.OrderCarrier = orderCarrier ?? placedOrder.GetAsCarrier(true, true, true, true, true, true);
                    if (order == null)
                    {
                        this.Log().Debug("Processing target group event for external order id {externalOrderId}.", placedOrder.ExternalOrderID);
                        _targetGroupEngine.Process(new OrderEvent { Order = placedOrder });
                    }
                    else
                    {
                        this.Log().Debug("Target group processing for external order id {externalOrderId} was executed with the order creation.", placedOrder.ExternalOrderID);
                    }
                }
            }

            Order TryFindOrder()
            {
                var checkoutOrderCarrier = paymentWidgetOrder.OrderCarrier;
                var hasExternalOrderId = !string.IsNullOrEmpty(checkoutOrderCarrier.ExternalOrderID);
                var foundOrder = hasExternalOrderId
                    ? _moduleECommerce.Orders.GetOrder(checkoutOrderCarrier.ExternalOrderID, _securityToken)
                    : _moduleECommerce.Orders.GetOrder(checkoutOrderCarrier.ID, _securityToken);

                if (foundOrder == null)
                {
                    if (hasExternalOrderId)
                    {
                        this.Log().Debug("No order exists for external order external id {externalOrderId}.", checkoutOrderCarrier.ExternalOrderID);
                    }
                    else
                    {
                        this.Log().Debug("No order exists for order id {orderId}.", checkoutOrderCarrier.ID);
                    }

                    var transactionNumber = checkoutOrderCarrier.PaymentInfo.FirstOrDefault()?.TransactionNumber;
                    if (!string.IsNullOrEmpty(transactionNumber))
                    {
                        this.Log().Debug("Try to find order based on transactionNumber {transactionNumber}.", transactionNumber);
                        foundOrder = _moduleECommerce.Orders.GetOrdersByTransactionNumber(paymentWidgetOrder.PaymentProviderName, transactionNumber, _securityToken).FirstOrDefault();
                        if (foundOrder == null)
                        {
                            this.Log().Debug("No order based on transactionNumber {transactionNumber} was found.", transactionNumber);
                        }
                    }
                    else
                    {
                        this.Log().Debug("No transaction number exists, or payment info missing. No order was found.");
                    }
                }

                if (foundOrder != null)
                {
                    this.Log().Debug("Persisted order is found, order id {orderId}, external order id {externalOrderId}.", foundOrder.ID, foundOrder.ExternalOrderID);
                }
                return foundOrder;
            }
        }
    }
}
