using Litium.Events;
using Litium.Foundation.Modules.ECommerce;
using Litium.Owin.Lifecycle;

namespace Litium.Accelerator.Search.Indexing.PopularProducts
{
    internal class MostSoldEventListener : IStartupTask
    {
        private readonly EventBroker _eventBroker;

        public MostSoldEventListener(
            EventBroker eventBroker)
        {
            _eventBroker = eventBroker;
        }

        public void Start()
        {
            ModuleECommerce.Instance.EventManager.StatisticUpdated += () => _eventBroker.Publish(new MostSoldCollector.CollectMostSoldProducts());
        }
    }
}
