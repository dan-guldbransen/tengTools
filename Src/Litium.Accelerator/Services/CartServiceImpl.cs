using Litium.Accelerator.Routing;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Security;
using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(CartService), FallbackService = true)]
    public class CartServiceImpl : CartService
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public CartServiceImpl(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        private Cart Cart => _requestModelAccessor.RequestModel.Cart;

        public override string FormatAmount(decimal amount, bool displayCurrencySymbol = true)
        {
            return Cart.Currency.Format(amount, displayCurrencySymbol, CultureInfo.CurrentCulture);
        }

        public override decimal GetNumberOfProducts()
        {
            var quantity = decimal.Zero;
            var cart = Cart;

            var sum = cart.OrderCarrier?.OrderRows?.FindAll(x => !x.CarrierState.IsMarkedForDeleting).Sum(y => y.Quantity);
            if (sum.HasValue)
            {
                quantity = sum.Value;
            }

            return decimal.Round(quantity, 0);
        }

        public override string GetNumberOfProductsAsString()
        {
            var quantity = GetNumberOfProducts();
            return quantity.ToString(CultureInfo.CurrentCulture);
        }

        public override Order PlaceOrder(out PaymentInfo[] paymentInfos)
        {
            return Cart.PlaceOrder(SecurityToken.CurrentSecurityToken, out paymentInfos);
        }

        public override void PreOrderValidate()
        {
            Cart.PreOrderValidate();            
        }

        public override void CalculatePrices()
        {
            Cart.CalculatePrices();
        }

        public override void UpdateRowQuantity(Guid orderRowId, int quantity)
        {
            Cart.UpdateRowQuantity(orderRowId, quantity);
            Cart.UpdateChangedRows();
        }

        public override List<DeliveryCarrier> GetAllDeliveryCarriers()
        {
            var orderCarrier = Cart.OrderCarrier;
            var deliveryCarriers = orderCarrier?.Deliveries.FindAll(x => !x.CarrierState.IsMarkedForDeleting);
            return deliveryCarriers;
        }

        public override void Clear()
        {
            Cart.Clear();
        }
    }
}
