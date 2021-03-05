using System;
using System.Collections.Generic;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(CartService))]
    [RequireServiceImplementation]
    public abstract class CartService
    {
        /// <summary>
        /// Generate the display string for total amount of items in cart
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="displayCurrencySymbol"></param>
        /// <returns></returns>
        public abstract string FormatAmount(decimal amount, bool displayCurrencySymbol = true);
        /// <summary>
        ///     Gets the number of products.
        /// </summary>
        /// <returns>System.Decimal.</returns>
        public abstract decimal GetNumberOfProducts();

        /// <summary>
        /// Gets the number of products as string.
        /// </summary>
        /// <returns>System.String.</returns>
        public abstract string GetNumberOfProductsAsString();

        /// <summary>
        /// Trigger PlaceOrder action of the cart entity
        /// </summary>
        /// <param name="paymentInfos">PaymentInfo of the order</param>
        /// <returns></returns>
        public abstract Order PlaceOrder(out PaymentInfo[] paymentInfos);
        /// <summary>
        /// Trigger PreOrderValidate for cart
        /// </summary>
        public abstract void PreOrderValidate();

        /// <summary>
        /// Calculate product prices in cart.
        /// </summary>
        public abstract void CalculatePrices();
        /// <summary>
        /// Update card info for a given row
        /// </summary>
        /// <param name="orderRowId"></param>
        /// <param name="quantity"></param>
        public abstract void UpdateRowQuantity(Guid orderRowId, int quantity);

        /// <summary>
        /// Gets all delivery carriers.
        /// </summary>
        /// <returns>List&lt;DeliveryCarrier&gt;.</returns>
        public abstract List<DeliveryCarrier> GetAllDeliveryCarriers();

        /// <summary>
        /// Clears the cart.
        /// </summary>
        public abstract void Clear();
    }
}
