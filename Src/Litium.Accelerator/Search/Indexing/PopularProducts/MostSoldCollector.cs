using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Data;
using Litium.Events;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Products;
using Litium.Products.Events;
using Litium.Products.Queryable;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.Search.Indexing.PopularProducts
{
    [Autostart]
    [Service(ServiceType = typeof(MostSoldCollector), Lifetime = DependencyLifetime.Singleton)]
    public class MostSoldCollector
    {
        private readonly MostSoldDataHolder _mostSoldDataHolder;
        private readonly EventBroker _eventBroker;
        private readonly SecurityToken _token;
        private readonly VariantService _variantService;
        private readonly ChannelService _channelService;
        private readonly MarketService _marketService;
        private readonly ILogger _logger;
        private readonly DataService _dataService;

        public MostSoldCollector(
            MostSoldDataHolder mostSoldDataHolder,
            EventBroker eventBroker,
            VariantService variantService,
            ChannelService channelService,
            MarketService marketService,
            ILogger<MostSoldCollector> logger,
            DataService dataService)
        {
            _mostSoldDataHolder = mostSoldDataHolder;
            _eventBroker = eventBroker;
            _variantService = variantService;
            _token = Solution.Instance.SystemToken;
            _channelService = channelService;
            _marketService = marketService;
            _logger = logger;
            _dataService = dataService;
            _eventBroker.Subscribe<CollectMostSoldProducts>(x => Collect());
        }

        public void Collect()
        {
            _logger.LogTrace("Collecting most sold products");

            var articleStatistics = new List<(Guid webSiteSystemId, string articleNumber, decimal count)>();
            foreach (var channel in _channelService.GetAll())
            {
                if (channel.WebsiteSystemId != null && channel.MarketSystemId != null)
                {
                    var market = _marketService.Get(channel.MarketSystemId.Value);
                    if (market?.AssortmentSystemId != null)
                    {
                        using var query = _dataService.CreateQuery<Category>().Filter(x => x.Assortment(market.AssortmentSystemId.Value));
                        var categorySystemIds = query.ToSystemIdList();
                        articleStatistics.AddRange(ModuleECommerce.Instance.Statistics.GetMostSoldArticles(null, channel.WebsiteSystemId.Value, channel.SystemId, categorySystemIds, int.MaxValue, _token)
                            .GroupBy(x => x.VariantArticleNumber, StringComparer.OrdinalIgnoreCase)
                            .Select(x => (channel.WebsiteSystemId.Value, x.Key, x.Sum(z => z.Count))));
                    }
                }
            }

            var existingArticleNumbers = _mostSoldDataHolder.GetCurrentArticleNumbers();
            foreach (var item in articleStatistics.GroupBy(x => x.articleNumber))
            {
                if (!existingArticleNumbers.TryGetValue(item.Key, out var existingWebsite))
                {
                    existingWebsite = new HashSet<Guid>();
                }

                var isUpdated = item.Aggregate(false, (current, iitem) =>
                {
                    existingWebsite.Remove(iitem.webSiteSystemId);
                    decimal old = -1;
                    if (_mostSoldDataHolder.TryGet(iitem.articleNumber, out var c))
                    {
                        c.TryGetValue(iitem.webSiteSystemId, out old);
                    }
                    var r = _mostSoldDataHolder.AddOrUpdate(iitem.articleNumber, iitem.webSiteSystemId, iitem.count);
                    _logger.LogTrace("WebSite: {3} Article: {0} IsUpdated: {2} IsCurrent: {5} OldCount: {4} Count: {1}", iitem.articleNumber, iitem.count, r, iitem.webSiteSystemId, old, current);
                    return r | current;
                });

                _logger.LogTrace("Article: {0} IsUpdated: {1}", item.Key, isUpdated);
                if (isUpdated)
                {
                    _logger.LogTrace("Article: {0} try get", item.Key);
                    var variant = _variantService.Get(item.Key);
                    if (variant != null)
                    {
                        _logger.LogTrace("Article: {0} fire event", item.Key);
                        _eventBroker.Publish(new VariantUpdated(variant.SystemId, variant.BaseProductSystemId, new Lazy<Variant>(() => variant)));
                    }
                    else
                    {
                        _logger.LogTrace("Article: {0} did not exists", item.Key);
                    }
                }
            }

            foreach (var item in existingArticleNumbers.Where(x => x.Value.Count > 0))
            {
                foreach (var iitem in item.Value)
                {
                    _mostSoldDataHolder.Remove(item.Key, iitem);
                }

                var variant = _variantService.Get(item.Key);
                if (variant != null)
                {
                    _logger.LogTrace("Article: {0} fire event", item.Key);
                    _eventBroker.Publish(new VariantUpdated(variant.SystemId, variant.BaseProductSystemId, new Lazy<Variant>(() => variant)));
                }
            }

            _logger.LogTrace("Collected most sold products");
        }

        public class CollectMostSoldProducts : IMessage { }
    }
}
