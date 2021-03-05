using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Studio.Extenssions;
using System.Linq;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Validate whether cart contains any product
    /// </summary>
    public class CartContainsProducts : IPreOrderValidationRule
    {
        /// <summary>
        ///     Validates the specified order carrier.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="checkoutFlowInfo">The checkout flow info.</param>
        public void Validate(OrderCarrier orderCarrier, CheckoutFlowInfo checkoutFlowInfo)
        {
            if (orderCarrier.OrderRows.Count(x => !x.CarrierState.IsMarkedForDeleting) == 0)
            {
                throw new PreOrderValidationException("accelerator.validation.cart.doesnotcontainsproduct".AsAngularResourceString());
            }
        }
    }
}

