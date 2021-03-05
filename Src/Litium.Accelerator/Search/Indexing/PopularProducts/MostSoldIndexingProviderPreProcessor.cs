using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Search.Filtering;
using Litium.Events;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search.Providers;
using Litium.Framework.Search.Indexing;
using Litium.Owin.Lifecycle;
using Litium.Websites;
using Litium.Websites.Events;

namespace Litium.Accelerator.Search.Indexing.PopularProducts
{
    internal class MostSoldIndexingProviderPreProcessor : IIndexingProviderPreProcessor, IStartupTask
    {
        private readonly MostSoldDataHolder _mostSoldDataHolder;
        private readonly HashSet<Guid> _webSiteIds = new HashSet<Guid>();
        private readonly EventBroker _eventBroker;
        private readonly WebsiteService _websiteService;

        public MostSoldIndexingProviderPreProcessor(MostSoldDataHolder mostSoldDataHolder, EventBroker eventBroker, WebsiteService websiteService)
        {
            _mostSoldDataHolder = mostSoldDataHolder;
            _eventBroker = eventBroker;
            _websiteService = websiteService;
        }

        public IndexDocument PreProcessDocument(IndexDocument document, string indexName)
        {
            if (ProductCatalogSearchDomains.Products.Equals(indexName, StringComparison.OrdinalIgnoreCase))
            {
                var articleNumberTagName = TagNames.ArticleNumber;
                if (bool.TryParse(document.Tags.Where(x => TagNames.UseVariantUrl.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).FirstOrDefault(), out bool useVariantUrl) && !useVariantUrl)
                {
                    articleNumberTagName = TagNames.VariantArticleNumbers;
                }

                IDictionary<Guid, decimal> allItems = new Dictionary<Guid, decimal>();
                var articleNumbers = document.Tags.Where(x => articleNumberTagName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value);
                foreach (var articleNumber in articleNumbers)
                {
                    if (_mostSoldDataHolder.TryGet(articleNumber, out IDictionary<Guid, decimal> items))
                    {
                        foreach (var item in items)
                        {
                            if (allItems.TryGetValue(item.Key, out decimal currentItem))
                            {
                                allItems[item.Key] = currentItem + item.Value;
                            }
                            else
                            {
                                allItems[item.Key] = item.Value;
                            }
                        }
                    }
                }

                foreach (var item in allItems)
                {
                    document.Tags.Add(new DocumentTag(FilteringConstants.GetMostSoldTagName(item.Key), item.Value));
                }

                foreach (var item in _webSiteIds.Except(allItems.Keys))
                {
                    document.Tags.Add(new DocumentTag(FilteringConstants.GetMostSoldTagName(item), 0));
                }
            }

            return document;
        }

        void IStartupTask.Start()
        {
            _eventBroker.Subscribe<WebsiteCreated>(x => _webSiteIds.Add(x.SystemId));
            _eventBroker.Subscribe<WebsiteDeleted>(x => _webSiteIds.Remove(x.SystemId));
            foreach (var webSite in _websiteService.GetAll())
            {
                _webSiteIds.Add(webSite.SystemId);
            }
        }
    }
}
