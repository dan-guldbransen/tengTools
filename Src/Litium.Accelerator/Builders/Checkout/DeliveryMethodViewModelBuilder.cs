using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;

namespace Litium.Accelerator.Builders.Checkout
{
    public class DeliveryMethodViewModelBuilder : IViewModelBuilder<DeliveryMethodViewModel>
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly CartService _cartService;
        private readonly CartAccessor _cartAccessor;

        public DeliveryMethodViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            CartService cartService,
            ModuleECommerce moduleECommerce,
            CartAccessor cartAccessor)
        {
            _moduleECommerce = moduleECommerce;
            _requestModelAccessor = requestModelAccessor;
            _cartService = cartService;
            _cartAccessor = cartAccessor;
        }

        public virtual List<DeliveryMethodViewModel> Build()
        {
            var ids = _requestModelAccessor.RequestModel.ChannelModel?.Channel?.CountryLinks?.FirstOrDefault(x => x.CountrySystemId == _cartAccessor.Cart.OrderCarrier.CountryID)?.DeliveryMethodSystemIds ?? Enumerable.Empty<Guid>();
            var currency = _requestModelAccessor.RequestModel.Cart.Currency;
            var allDeliveryMethods = _moduleECommerce.DeliveryMethods.GetAll();
            var deliveryMethods = ids.Select(x => allDeliveryMethods.First(y => y.ID == x)).Select(x =>
            {
                var cost = x.GetCost(currency.SystemId)?.Cost ?? 0;
                return new DeliveryMethodViewModel
                {
                    Id = x.ID,
                    Name = x.GetDisplayName(_requestModelAccessor.RequestModel.ChannelModel?.Channel?.WebsiteLanguageSystemId ?? Guid.Empty) ?? x.Name,
                    Price = cost,
                    FormattedPrice = _cartService.FormatAmount(cost)
                };
            }).ToList();
            return deliveryMethods;
        }
    }
}
