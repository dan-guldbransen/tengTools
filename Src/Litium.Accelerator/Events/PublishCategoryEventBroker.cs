using Litium.Accelerator.Routing;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Events
{
    [Autostart]
    class PublishCategoryEventBroker
    {
        private readonly Litium.Events.ISubscription<Products.Events.CategoryCreated> _subscription;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageService _pageService;
        private readonly ChannelService _channelService;
        public PublishCategoryEventBroker(Litium.Events.EventBroker eventBroker, RequestModelAccessor requestModelAccessor, PageService pageService, ChannelService channelService)
        {
            _pageService = pageService;
            _requestModelAccessor = requestModelAccessor;
            _channelService = channelService;
            _subscription = eventBroker.Subscribe<Products.Events.CategoryCreated>(categoryCreated =>
            {
                var t = 5;
                //Publish(categoryCreated.Item);
            });
        }

        private void Publish(Category category)
        {
            var channels = GetChannels();
            foreach (var channel in channels)
            {

            }
        }

        private List<Channel> GetChannels()
        {
            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            var startPage = _pageService.GetChildPages(Guid.Empty, website.SystemId).FirstOrDefault();
            var channelsTemp = new List<Guid>();

            foreach (var cl in startPage.ChannelLinks)
            {
                if (cl.StartDateTimeUtc.HasValue && cl.StartDateTimeUtc.Value.CompareTo(DateTime.UtcNow) > 0)
                    continue;

                if (cl.EndDateTimeUtc.HasValue && cl.EndDateTimeUtc.Value.CompareTo(DateTime.UtcNow) <= 0)
                    continue;

                channelsTemp.Add(cl.ChannelSystemId);
            }
            var channels = _channelService.GetAll()
               .Where(c => channelsTemp.Contains(c.SystemId))
               .ToList();
            return channels;
        }

        public void Dispose()
        {
            // unregister the event from event broker
            _subscription.Dispose();
        }
    }
}
