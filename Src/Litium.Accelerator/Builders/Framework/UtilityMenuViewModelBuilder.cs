using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Globalization;
using Litium.Web;
using System.Linq;

namespace Litium.Accelerator.Builders.Framework
{
    public class UtilityMenuViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : UtilityMenuViewModel
    {
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ChannelService _channelService;
        

        public UtilityMenuViewModelBuilder(
            UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            ChannelService channelService)
        {
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _channelService = channelService;
        }

        public UtilityMenuViewModel Build()
        {
            var viewModel = new UtilityMenuViewModel();

            // Get current culture
            var currentUICulture = _requestModelAccessor.RequestModel.ChannelModel.Channel.Localizations.CurrentUICulture.Name;
            var culture = _requestModelAccessor.RequestModel.ChannelModel.Channel.Localizations.First(x => x.Value.Name == currentUICulture);

            // get current website
            var channels = _channelService.GetAll().ToList();
           
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
