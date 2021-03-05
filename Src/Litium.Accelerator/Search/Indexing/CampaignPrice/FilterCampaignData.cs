using System;

namespace Litium.Accelerator.Search.Indexing.CampaignPrice
{
    [Serializable]
    public class FilterCampaignData
    {
        public Guid CampaignId { get; set; }
        public decimal Price { get; set; }
    }
}
