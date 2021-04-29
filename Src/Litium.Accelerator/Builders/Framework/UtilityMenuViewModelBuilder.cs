using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Globalization;
using Litium.Web;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Litium.Accelerator.Builders.Framework
{
    public class UtilityMenuViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : UtilityMenuViewModel
    {
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ChannelService _channelService;
        private readonly PageService _pageService;

        public UtilityMenuViewModelBuilder(
            UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            ChannelService channelService,
            PageService pageService)
        {
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _channelService = channelService;
            _pageService = pageService; 
        }

        public UtilityMenuViewModel Build()
        {
            var viewModel = new UtilityMenuViewModel();

            // Get current culture
            var currentUICulture = _requestModelAccessor.RequestModel.ChannelModel.Channel.Localizations.CurrentUICulture.Name;
            var culture = _requestModelAccessor.RequestModel.ChannelModel.Channel.Localizations.First(x => x.Value.Name == currentUICulture);

            // Only active channels so get channellinks to startpage
            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            var startPage = _pageService.GetChildPages(Guid.Empty, website.SystemId).FirstOrDefault();
            var channelsActive = new List<Guid>();

            // also check dates if set
            foreach(var cl in startPage.ChannelLinks)
            {
                if(cl.StartDateTimeUtc.HasValue && cl.StartDateTimeUtc.Value.CompareTo(DateTime.UtcNow) > 0)
                    continue;
                
                if(cl.EndDateTimeUtc.HasValue && cl.EndDateTimeUtc.Value.CompareTo(DateTime.UtcNow) <= 0)
                    continue;
                
                channelsActive.Add(cl.ChannelSystemId);
            }

            // get channels to be shown on marketselector
            var channels = _channelService.GetAll()
                .Where(c => channelsActive.Contains(c.SystemId))
                .ToList();

            foreach (var channel in channels)
            {
                viewModel.ChannelLinkList.Add(new ContentLinkModel
                {
                    IsSelected = _requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId == channel.SystemId,
                    Url = _urlService.GetUrl(channel, new ChannelUrlArgs { AbsoluteUrl = true, UsePrimaryDomainName = true }),
                    Name = channel.Fields.GetFieldContainer(culture.Key).GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name)
                    // TODO: SET IMAGE
                });
            }

            return viewModel;
        }
    }
}
