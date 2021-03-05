using System;
using System.Linq;
using Litium.Accelerator.Search.Filtering;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search.Providers;
using Litium.Framework.Search.Indexing;

namespace Litium.Accelerator.Search.Indexing.CampaignPrice
{
    internal class CampaignIndexingProviderPreProcessor : IIndexingProviderPreProcessor
    {
        private readonly CampaignDataHolder _campaignDataHolder;

        public CampaignIndexingProviderPreProcessor(CampaignDataHolder campaignDataHolder)
        {
            _campaignDataHolder = campaignDataHolder;
        }

        public IndexDocument PreProcessDocument(IndexDocument document, string indexName)
        {
            if (ProductCatalogSearchDomains.Products.Equals(indexName, StringComparison.OrdinalIgnoreCase))
            {
                var articleIds = document.Tags.Where(x => x.Name.Equals(TagNames.VariantSystemId, StringComparison.OrdinalIgnoreCase)).Select(x => (Guid)x.OriginalValue).ToList();
                foreach (var item in articleIds
                    .SelectMany(_campaignDataHolder.GetArticleCampaigns)
                    .GroupBy(x => x.CampaignId)
                    .Select(x => new { x.Key, Price = x.Min(z => z.Price) }))
                {
                    document.Tags.Add(new ReadableDocumentTag(FilteringConstants.GetCampaignTagName(item.Key), item.Price));
                }
            }

            return document;
        }
    }
}
