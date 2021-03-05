using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Foundation.Security;
using Litium.Sales;

namespace Litium.Accelerator.StateTransitions
{
    public class OrderStateSerivceImpl : OrderStateService
    {
        public override bool IsReturnConfirmed(Order order)
        {
            return order.OrderStatus == (short)OrderState.ReturnConfirmed;
        }

        public override void SetReturnConfirmed(Order order, SecurityToken token)
        {
            order.SetOrderStatus((short)OrderState.ReturnConfirmed, token);
        }

        public override bool IsProcessing(Order order)
        {
            return order.OrderStatus == (short)OrderState.Processing;
        }
    }
}
