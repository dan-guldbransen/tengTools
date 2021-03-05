using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Deliveries;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Globalization;
using Litium.Studio.Extenssions;
using System.Linq;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Validates the selected delivery method.
    /// </summary>
    public class DeliveryMethodValidator : IPreOrderValidationRule
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly ChannelService _channelService;

        public DeliveryMethodValidator(ModuleECommerce moduleECommerce, ChannelService channelService)
        {
            _moduleECommerce = moduleECommerce;
            _channelService = channelService;
        }

        /// <summary>
        ///     Validates the specified order carrier.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="checkoutFlowInfo">The checkout flow info.</param>
        public void Validate(OrderCarrier orderCarrier, CheckoutFlowInfo checkoutFlowInfo)
        {
            //get allowed delivery methods, if configuration is missing, consider all delivery methods as allowed.
            var allowedDeliveryMethodIDs = _channelService.Get(orderCarrier.ChannelID)?.CountryLinks?.FirstOrDefault(x => x.CountrySystemId == orderCarrier.CountryID)?.DeliveryMethodSystemIds ?? 
                                           _moduleECommerce.DeliveryMethods.GetAll().Select(x => x.ID).ToList();

            foreach (var delivery in orderCarrier.Deliveries)
            {
                var deliveryMethod = _moduleECommerce.DeliveryMethods.Get(delivery.DeliveryMethodID, _moduleECommerce.AdminToken);
                if (deliveryMethod != null && allowedDeliveryMethodIDs.Contains(deliveryMethod.ID)) continue;

                if (allowedDeliveryMethodIDs.Count > 0)
                {
                    var payload = new DeliveryPayloadInfo
                    {
                        DeliveryAddress = delivery.Address,
                        DeliveryMethodId = allowedDeliveryMethodIDs[0]
                    };
                    _moduleECommerce.CheckoutFlow.EditDelivery(orderCarrier, delivery.ID, payload, _moduleECommerce.AdminToken);
                }
                throw new PreOrderValidationException("accelerator.validation.deliverymethod.nolongersupported".AsAngularResourceString());
            }
        }
    }
}

