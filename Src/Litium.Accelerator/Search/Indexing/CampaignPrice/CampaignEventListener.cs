using Litium.Events;
using Litium.Foundation.Modules.ECommerce;
using Litium.Owin.Lifecycle;

namespace Litium.Accelerator.Search.Indexing.CampaignPrice
{
    internal class CampaignEventListener : IStartupTask
    {
        private readonly EventBroker _eventBroker;

        public CampaignEventListener(
            EventBroker eventBroker)
        {
            _eventBroker = eventBroker;
        }

        void IStartupTask.Start()
        {
            ModuleECommerce.Instance.EventManager.CampaignCreated += x => _eventBroker.Publish(new CampaignCollector.CollectCampaignPrices(x));
            ModuleECommerce.Instance.EventManager.CampaignDeleted += x => _eventBroker.Publish(new CampaignCollector.RemoveCollectedCampaignPrices(x));
            ModuleECommerce.Instance.EventManager.CampaignActionInfoUpdated += (campaignId, _) => _eventBroker.Publish(new CampaignCollector.CollectCampaignPrices(campaignId));
        }
    }
}
