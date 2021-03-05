using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Litium.Events;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Actions;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Conditions;
using Litium.Foundation.Security;
using Litium.Products;
using Litium.Products.Events;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.Search.Indexing.CampaignPrice
{
    [Autostart]
    [Service(ServiceType = typeof(CampaignCollector), Lifetime = DependencyLifetime.Singleton)]
    public class CampaignCollector
    {
        private readonly Action<Guid> _articleUpdated;
        private readonly CampaignDataHolder _campaignDataHolder;
        private readonly ILogger<CampaignCollector> _logger;
        private readonly ConcurrentDictionary<Guid, object> _syncLocks = new ConcurrentDictionary<Guid, object>();
        private readonly SecurityToken _token;

        public CampaignCollector(
            CampaignDataHolder campaignDataHolder,
            EventBroker eventBroker,
            VariantService variantService,
            ILogger<CampaignCollector> logger)
        {
            _campaignDataHolder = campaignDataHolder;
            _logger = logger;
            _token = Solution.Instance.SystemToken;

            _articleUpdated = x =>
            {
                var variant = variantService.Get(x);
                if (variant != null)
                {
                    eventBroker.Publish(new VariantUpdated(variant.SystemId, variant.BaseProductSystemId, new Lazy<Variant>(() => variant)));
                }
            };

            eventBroker.Subscribe<CollectAllCampaignPrices>(x =>
            {
                foreach (var item in ModuleECommerce.Instance.Campaigns.GetAll(true))
                {
                    var systemId = item.ID;
                    Task.Run(() => Updated(systemId));
                }
            });
            eventBroker.Subscribe<CollectCampaignPrices>(x => Updated(x.CampaignSystemId));
            eventBroker.Subscribe<RemoveCollectedCampaignPrices>(x => RemoveCampaign(x.CampaignSystemId, notify: false));
        }

        private void Updated(Guid campaignId)
        {
            lock (_syncLocks.GetOrAdd(campaignId, _ => new object()))
            {
                _logger.LogTrace("Collecting data for campaign {campaignSystemId}", campaignId);

                var campaign = ModuleECommerce.Instance.Campaigns.GetCampaign(campaignId, _token);
                if (campaign.ConditionInfos.All(x => x.TypeName == typeof(UserBelongsToGroupCondition).FullName))
                {
                    var campaignActionInfo = campaign.ActionInfo;
                    if (campaignActionInfo != null)
                    {
                        if (campaignActionInfo.TypeName == typeof(ArticleCampaignPriceAction).FullName)
                        {
                            var data = campaignActionInfo.GetData<ArticleCampaignPriceAction.Data>();
                            foreach (var key in _campaignDataHolder.GetCampaignArticles(campaignId).Except(data.CampaignPriceList.Keys).ToList())
                            {
                                if (_campaignDataHolder.RemoveArticlePrice(key, campaignId))
                                {
                                    _articleUpdated(key);
                                }
                            }

                            foreach (var item in data.CampaignPriceList)
                            {
                                if (_campaignDataHolder.AddOrUpdateArticlePrice(item.Key, campaignId, item.Value))
                                {
                                    _articleUpdated(item.Key);
                                }
                            }
                        }
                        else
                        {
                            RemoveCampaign(campaignId);
                        }
                    }
                }
                else
                {
                    RemoveCampaign(campaignId);
                }

                _logger.LogTrace("Collected data for campaign {campaignSystemId}", campaignId);
            }
        }

        private void RemoveCampaign(Guid campaignId, bool notify = true)
        {
            foreach (var key in _campaignDataHolder.GetCampaignArticles(campaignId).ToList())
            {
                if (_campaignDataHolder.RemoveArticlePrice(key, campaignId) && notify)
                {
                    _articleUpdated(key);
                }
            }

            _campaignDataHolder.RemoveCampaign(campaignId);
        }

        public class CollectAllCampaignPrices : IMessage { }

        public class CollectCampaignPrices : IMessage
        {
            public CollectCampaignPrices(Guid campaignSystemId)
            {
                CampaignSystemId = campaignSystemId;
            }

            public Guid CampaignSystemId { get; }
        }

        public class RemoveCollectedCampaignPrices : IMessage
        {
            public RemoveCollectedCampaignPrices(Guid campaignSystemId)
            {
                CampaignSystemId = campaignSystemId;
            }

            public Guid CampaignSystemId { get; }
        }
    }
}
