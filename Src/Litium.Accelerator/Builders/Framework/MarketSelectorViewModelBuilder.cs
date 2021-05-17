using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Globalization;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Litium.Accelerator.Builders.Framework
{
    public class MarketSelectorViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : MarketSelectorViewModel
    {
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ChannelService _channelService;
        private readonly PageService _pageService;
        private readonly LanguageService _languagesService;

        public MarketSelectorViewModelBuilder(
            UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            ChannelService channelService,
            PageService pageService,
            LanguageService languageService)
        {
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _channelService = channelService;
            _pageService = pageService;
            _languagesService = languageService;
        }

        public MarketSelectorViewModel Build()
        {
            var viewModel = new MarketSelectorViewModel();

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
                var channelModel = channel.MapTo<ChannelModel>();

                var linkModel = new ContentLinkModel
                {
                    IsSelected = _requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId == channel.SystemId,
                    Url = _urlService.GetUrl(channel, new ChannelUrlArgs { UsePrimaryDomainName = true }),
                    Name = channelModel.GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name),
                    Image = channelModel.GetValue<Guid?>(ChannelFieldNameConstants.FlagIcon)?.MapTo<ImageModel>(),
                    ExtraInfo = channelModel.GetValue<bool>(ChannelFieldNameConstants.IsInternational).ToString()
                };

                viewModel.ChannelLinkList.Add(linkModel);
            }
            
            // order by name
            viewModel.ChannelLinkList = viewModel.ChannelLinkList.OrderBy(x => x.Name).ToList();

            // move international first
            var international = viewModel.ChannelLinkList.FirstOrDefault(x => x.ExtraInfo.Equals("true", StringComparison.OrdinalIgnoreCase));
            if(international != null)
            {
                viewModel.ChannelLinkList.Remove(international);
                viewModel.ChannelLinkList.Insert(0, international);
            }

            // partners
            var culture = CultureInfo.CurrentUICulture;
            var partners = website.GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Partners);

            if (partners != null)
            {
                foreach (var partner in partners)
                {
                    var linkModel = new ContentLinkModel
                    {
                        Url = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerWebsite),
                        Name = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerCountry, CultureInfo.CurrentUICulture),
                        Image = partner.Fields.GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.PartnerFlagIcon)?.MapTo<ImageModel>()
                    };

                    viewModel.PartnerLinkList.Add(linkModel);
                }
            }

            return viewModel;
        }
    }
}
