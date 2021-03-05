using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Payments;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Security;

namespace Litium.Accelerator.Builders.Checkout
{
    public class PaymentMethodViewModelBuilder : IViewModelBuilder<PaymentMethodViewModel>
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly PaymentService _paymentService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly SecurityToken _securityToken;
        private readonly CartService _cartService;
        private readonly CartAccessor _cartAccessor;

        public PaymentMethodViewModelBuilder(
            ModuleECommerce moduleECommerce,
            PaymentService paymentService,
            SecurityToken securityToken,
            CartService cartService,
            RequestModelAccessor requestModelAccessor,
            CartAccessor cartAccessor
            )
        {
            _moduleECommerce = moduleECommerce;
            _paymentService = paymentService;
            _requestModelAccessor = requestModelAccessor;
            _securityToken = securityToken;
            _cartService = cartService;
            _cartAccessor = cartAccessor;
        }

        public PaymentWidgetResult BuildWidget(string paymentMethodId)
        {
            var currentOrderCarrier = _requestModelAccessor.RequestModel.Cart.OrderCarrier;
            var paymentMethodParts = paymentMethodId.Split(':');
            var paymentMethod = _moduleECommerce.PaymentMethods.Get(paymentMethodParts[1], paymentMethodParts[0], _securityToken);
            var paymentInfo = currentOrderCarrier.PaymentInfo.Single(x => x.PaymentProvider.Equals(paymentMethod.PaymentProviderName) && !x.CarrierState.IsMarkedForDeleting);
            var paymentWidget = _paymentService.GetPaymentWidget(paymentInfo);
            var widget = paymentWidget?.GetWidget(currentOrderCarrier, paymentInfo);
            if (widget != null)
            {
                widget.Hash = DateTime.Now.Ticks;
            }
            return widget;
        }

        public virtual List<PaymentMethodViewModel> Build()
        {
            var ids = _requestModelAccessor.RequestModel.ChannelModel?.Channel?.CountryLinks?.FirstOrDefault(x => x.CountrySystemId == _cartAccessor.Cart.OrderCarrier.CountryID)?.PaymentMethodSystemIds ?? Enumerable.Empty<Guid>();
            var currency = _requestModelAccessor.RequestModel.Cart.Currency;
            var allPaymentMethods = _moduleECommerce.PaymentMethods.GetAll();
            var paymentMethods = ids.Select(x => allPaymentMethods.FirstOrDefault(y => y.ID == x)).Where(x => x is object).Select(x =>
            {
                var cost = x.GetCost(currency.SystemId)?.Cost ?? 0;
                return new PaymentMethodViewModel
                {
                    Id = string.Concat(x.PaymentProviderName, ":", x.Name),
                    Name = x.GetDisplayName(_requestModelAccessor.RequestModel.ChannelModel?.Channel?.WebsiteLanguageSystemId ?? Guid.Empty),
                    Price = cost,
                    FormattedPrice = _cartService.FormatAmount(cost)
                };
            }).ToList();
            return paymentMethods;
        }
    }
}
