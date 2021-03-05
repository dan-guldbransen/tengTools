using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Foundation.Security;
using Litium.Studio.Extenssions;
using System;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Check whether order grand total has not changed since the last cart re-calculation.
    /// </summary>
    public class OrderGrandTotalHasNotChanged : IPreOrderValidationRule
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly SecurityToken _securityToken;

        public OrderGrandTotalHasNotChanged(ModuleECommerce moduleECommerce, SecurityToken securityToken)
        {
            _moduleECommerce = moduleECommerce;
            _securityToken = securityToken;
        }

        /// <summary>
        ///     Validates the specified order carrier.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="checkoutFlowInfo">The checkout flow info.</param>
        public void Validate(OrderCarrier orderCarrier, CheckoutFlowInfo checkoutFlowInfo)
        {
            if (orderCarrier.OrderRows.Count != 0)
            {
                var clone = orderCarrier.Clone(true, true, true, true, true, true);
                _moduleECommerce.Orders.CalculateOrderTotals(clone, _securityToken);
                if (Math.Round(clone.GrandTotal, 2) != Math.Round(orderCarrier.GrandTotal, 2))
                {
                    throw new PreOrderValidationException("accelerator.validation.ordertotal.haschanged".AsAngularResourceString());
                }
            }
        }
    }
}

