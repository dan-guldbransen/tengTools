using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Globalization;
using Litium.Studio.Extenssions;
using System.Linq;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Validates the selected payment method.
    /// </summary>
    public class PaymentMethodValidator : IPreOrderValidationRule
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly ChannelService _channelService;

        public PaymentMethodValidator(ModuleECommerce moduleECommerce, ChannelService channelService)
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
            //get allowed payment methods, if configuration is missing, consider all payment methods as allowed.
            var allowedPaymentMethodIDs = _channelService.Get(orderCarrier.ChannelID)?.CountryLinks?.FirstOrDefault(x => x.CountrySystemId == orderCarrier.CountryID)?.PaymentMethodSystemIds ??
                                          _moduleECommerce.PaymentMethods.GetAll().Select(x => x.ID).ToList();

            foreach (var paymentInfo in orderCarrier.PaymentInfo)
            {
                var paymentMethod = _moduleECommerce.PaymentMethods.Get(paymentInfo.PaymentMethod, paymentInfo.PaymentProvider, _moduleECommerce.AdminToken);
                if (paymentMethod != null && allowedPaymentMethodIDs.Contains(paymentMethod.ID)) continue;

                if (allowedPaymentMethodIDs.Count > 0)
                {
                    var defaultPaymentmethod = _moduleECommerce.PaymentMethods.Get(allowedPaymentMethodIDs[0], _moduleECommerce.AdminToken);
                    if (defaultPaymentmethod != null)
                    {
                        _moduleECommerce.CheckoutFlow.AddPaymentInfo(orderCarrier, defaultPaymentmethod.PaymentProviderName, defaultPaymentmethod.Name, paymentInfo.BillingAddress, _moduleECommerce.AdminToken);
                    }
                }
                throw new PreOrderValidationException("accelerator.validation.paymentmethod.nolongersupported".AsAngularResourceString());
            }
        }
    }
}