using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Order;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Security;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;
using System;

namespace Litium.Accelerator.Builders.Order
{
    public class OrderConfirmationViewModelBuilder : IViewModelBuilder<OrderConfirmationViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly OrderViewModelBuilder _orderViewModelBuilder;

        public OrderConfirmationViewModelBuilder(RequestModelAccessor requestModelAccessor, 
            ModuleECommerce moduleECommerce, 
            OrderViewModelBuilder orderViewModelBuilder)
        {
            _requestModelAccessor = requestModelAccessor;
            _moduleECommerce = moduleECommerce;
            _orderViewModelBuilder = orderViewModelBuilder;
        }

        public OrderConfirmationViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<OrderConfirmationViewModel>();
            var order = _moduleECommerce.Orders.GetOrder(_requestModelAccessor.RequestModel.Cart.OrderCarrier.ExternalOrderID, SecurityToken.CurrentSecurityToken);
            if (order != null)
            {
                model.Order = _orderViewModelBuilder.Build(order);
            }
            return model;
        }

        public OrderConfirmationViewModel Build(PageModel pageModel, Guid orderId)
        {
            var model = pageModel.MapTo<OrderConfirmationViewModel>();
            var order = _moduleECommerce.Orders.GetOrder(orderId, SecurityToken.CurrentSecurityToken);
            if (order != null)
            {
                model.Order = _orderViewModelBuilder.Build(order);
            }
            return model;
        }
    }
}
