using System;
using Litium.Accelerator.StateTransitions;
using Litium.Accelerator.ViewModels.Order;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Security;

namespace Litium.Accelerator.Services
{
    public class OrderHistoryViewModelService : ViewModelService<OrderHistoryViewModel>
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly SecurityToken _securityToken;

        public OrderHistoryViewModelService(ModuleECommerce moduleECommerce, SecurityToken securityToken)
        {
            _moduleECommerce = moduleECommerce;
            _securityToken = securityToken;
        }

        public void SaveOrder(OrderState orderState, Guid id)
        {
            var order = _moduleECommerce.Orders.GetOrder(id, _securityToken);

            if (orderState == OrderState.Confirmed)
            {
                order.SetOrderStatus((short)OrderState.Confirmed, _securityToken);
            }
            else if (orderState == OrderState.Cancelled)
            {
                //cancel all deliveries, this will cancel all payments and then cancel the order automatically.
                foreach (var delivery in order.Deliveries)
                {
                    delivery.SetDeliveryStatus((short)DeliveryState.Cancelled, _moduleECommerce.AdminToken);
                }
                //it can be the case that deliveries were infact cancelled from backoffice. 
                //in which case, the automatic triggering of the cancelleation will not pass through. So, attempt to cancel the order as well.
                order.SetOrderStatus((short)OrderState.Cancelled, _securityToken);
            }
        }
    }
}
