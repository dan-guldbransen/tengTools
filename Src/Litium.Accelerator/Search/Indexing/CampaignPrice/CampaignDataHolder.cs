using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Litium.Common;
using Litium.Events;
using Litium.Foundation.Configuration;
using Litium.Foundation.Extenssions;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.Search.Indexing.CampaignPrice
{
    [Service(ServiceType = typeof(CampaignDataHolder), Lifetime = DependencyLifetime.Singleton)]
    public class CampaignDataHolder
    {
        private readonly RetryableLazy<CampaignHolderData> _campaignData;

        public CampaignDataHolder(
            IApplicationLifetime applicationLifetime,
            EventBroker eventBroker,
            ILogger<CampaignDataHolder> logger)
        {
            var filePath = new FileInfo(Path.Combine(GeneralConfig.Instance.SearchDirectory, "CampaignPriceFiltering.bin"));
            _campaignData = new RetryableLazy<CampaignHolderData>(() =>
            {
                var items = filePath.LoadData<CampaignHolderData>();

                if (items is null)
                {
                    logger.LogTrace("Persisted data does not exists in {path}", filePath.FullName);

                    items = new CampaignHolderData
                    {
                        Articles = new ConcurrentDictionary<Guid, List<FilterCampaignData>>(),
                        Campaigns = new ConcurrentDictionary<Guid, HashSet<Guid>>()
                    };


                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        if (items.Articles.Count > 0 || items.Campaigns.Count > 0)
                        {
                            return;
                        }

                        eventBroker.Publish(new CampaignCollector.CollectAllCampaignPrices());
                    });
                }
                else
                {
                    logger.LogTrace("Loaded persisted data from {path}", filePath.FullName);
                }

                return items;
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                if (_campaignData.IsValueCreated)
                {
                    logger.LogTrace("Persisting to {path}", filePath.FullName);
                    filePath.Persist(_campaignData.Value);
                    logger.LogTrace("Persisted");
                }
                else
                {
                    logger.LogTrace("No data exists, no need to persist");
                }
            });
        }

        public bool AddOrUpdateArticlePrice(Guid articleId, Guid campaignId, decimal price)
        {
            var result = true;
            _campaignData.Value.Articles.AddOrUpdate(articleId, new List<FilterCampaignData> { new FilterCampaignData { CampaignId = campaignId, Price = price } }, (_, c) =>
            {
                var item = c.Find(x => x.CampaignId == campaignId);
                if (item == null)
                {
                    c.Add(new FilterCampaignData { CampaignId = campaignId, Price = price });
                }
                else
                {
                    result = item.Price != price;
                    item.Price = price;
                }
                return c;
            });

            _campaignData.Value.Campaigns.GetOrAdd(campaignId, new HashSet<Guid>()).Add(articleId);
            return result;
        }

        public List<FilterCampaignData> GetArticleCampaigns(Guid articleId)
        {
            return _campaignData.Value.Articles.GetOrAdd(articleId, _ => new List<FilterCampaignData>());
        }

        public HashSet<Guid> GetCampaignArticles(Guid campaignId)
        {
            return _campaignData.Value.Campaigns.GetOrAdd(campaignId, _ => new HashSet<Guid>());
        }

        public bool RemoveArticlePrice(Guid articleId, Guid campaignId)
        {
            _campaignData.Value.Articles.AddOrUpdate(articleId, new List<FilterCampaignData>(), (_, c) =>
            {
                c.RemoveAll(x => x.CampaignId == campaignId);
                return c;
            });

            return _campaignData.Value.Campaigns.GetOrAdd(campaignId, new HashSet<Guid>()).Remove(articleId);
        }

        public void RemoveCampaign(Guid campaignId)
        {
            _campaignData.Value.Campaigns.TryRemove(campaignId, out var item);
        }

        [Serializable]
        private class CampaignHolderData
        {
            public ConcurrentDictionary<Guid, List<FilterCampaignData>> Articles { get; set; }
            public ConcurrentDictionary<Guid, HashSet<Guid>> Campaigns { get; set; }
        }
    }
}
