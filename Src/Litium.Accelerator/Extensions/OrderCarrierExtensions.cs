using Litium.Foundation.Modules.ECommerce.Carriers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Litium.Accelerator.Extensions
{
    public static class OrderCarrierExtensions
    {
        /// <summary>
        ///     Gets the campaign ids.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <returns></returns>
        public static List<Guid> GetCampaignIds(this OrderCarrier orderCarrier)
        {
            var result = new List<Guid>();
            if (orderCarrier != null)
            {
                //order row campaigns.
                if (orderCarrier.OrderRows != null)
                {
                    result.AddRange(orderCarrier.OrderRows.Where(x => x.CampaignID != Guid.Empty).Select(x => x.CampaignID));
                }
                //delivery campaigns.
                if (orderCarrier.Deliveries != null)
                {
                    result.AddRange(orderCarrier.Deliveries.Where(x => x.CampaignID != Guid.Empty).Select(x => x.CampaignID));
                }
                //fee campaigns
                if (orderCarrier.Fees != null)
                {
                    result.AddRange(orderCarrier.Fees.Where(x => x.CampaignID != Guid.Empty).Select(x => x.CampaignID));
                }
                //order campaigns.
                if (orderCarrier.OrderDiscounts != null)
                {
                    result.AddRange(orderCarrier.OrderDiscounts.Where(x => x.CampaignID != Guid.Empty).Select(x => x.CampaignID));
                }
            }
            return result;
        }
    }
}
