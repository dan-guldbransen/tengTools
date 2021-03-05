using Litium.Accelerator.Extensions;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Foundation.Security;
using Litium.Studio.Extenssions;
using System.Linq;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Check whether campaign code still valid.
    /// </summary>
    internal class CampaignCodeValidator : IPreOrderValidationRule
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly SecurityToken _securityToken;

        public CampaignCodeValidator(ModuleECommerce moduleECommerce, SecurityToken securityToken)
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
            if (orderCarrier.OrderRows.Count != 0 && !string.IsNullOrEmpty(orderCarrier.CampaignInfo))
            {
                var withVoucherCode = orderCarrier.Clone(true, true, true, true, true, true);
                var noVoucherCode = withVoucherCode.Clone(true, true, true, true, true, true);
                noVoucherCode.CampaignInfo = string.Empty;

                _moduleECommerce.Orders.CalculateOrderTotals(noVoucherCode, _securityToken);
                _moduleECommerce.Orders.CalculateOrderTotals(withVoucherCode, _securityToken);

                //if the order total becomes modified, it surely is due to the voucherCode since the carriers are just clones of each other and only difference is voucher code.
                if (noVoucherCode.GrandTotal == withVoucherCode.GrandTotal)
                {
                    //the order grand total is not modified, but still the voucher code may have resulted in a different campaign.
                    //obtain all campaign ids in the order carrier and see whether withVoucherCode resulted in a new campaign id.
                    var noVoucherCodeCampaignIds = noVoucherCode.GetCampaignIds();
                    var withVoucherCodeCampaignIds = withVoucherCode.GetCampaignIds();

                    //check whether withVoucherCodeCampaignIds has any campaign ids, that are not there in noVoucherCodeCampaignIds.
                    if (!withVoucherCodeCampaignIds.Except(noVoucherCodeCampaignIds).Any())
                    {
                        throw new PreOrderValidationException("accelerator.validation.campaigncode.notvalid".AsAngularResourceString());
                    }
                }
            }
        }
    }
}

