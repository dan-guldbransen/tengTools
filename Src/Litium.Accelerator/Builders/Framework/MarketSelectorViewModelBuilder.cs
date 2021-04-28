using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models;
using Litium.Websites;
using System;
using System.Collections.Generic;
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
                var lang = channel.WebsiteLanguageSystemId.HasValue ? _languagesService.Get(channel.WebsiteLanguageSystemId.Value) : null;

                var linkModel = new ContentLinkModel
                {
                    IsSelected = _requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId == channel.SystemId,
                    Url = _urlService.GetUrl(channel, new ChannelUrlArgs { UsePrimaryDomainName = true }),
                    Name = channel.Fields.GetFieldContainer(culture.Key).GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name),
                    ExtraInfo = lang != null ? lang.CultureInfo.TwoLetterISOLanguageName.ToUpper() : "",
                    Image = channel.Fields.GetValue<Guid?>(ChannelFieldNameConstants.FlagIcon)?.MapTo<ImageModel>()
                };

                viewModel.ChannelLinkList.Add(linkModel);
            }

            return viewModel;
        }
    }
}
