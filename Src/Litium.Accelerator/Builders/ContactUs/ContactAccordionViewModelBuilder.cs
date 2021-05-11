using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.ContactUs;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models.Globalization;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Builders.ContactUs
{
    public class ContactAccordionViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel> where TViewModel : ContactAccordionViewModel
    {
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ChannelService _channelService;
        private readonly PageService _pageService;

        public ContactAccordionViewModelBuilder(UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            ChannelService channelService,
            PageService pageService)
        {
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _channelService = channelService;
            _pageService = pageService;
        }

        public ContactAccordionViewModel Build(bool isPartners = false)
        {
            var model = new ContactAccordionViewModel
            {
                IsPartnerList = isPartners
            };

            var website = _requestModelAccessor.RequestModel.WebsiteModel;

            if (isPartners)
            {
                model.PageTitle = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerTitle);
                var partners = website.GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Partners);

                foreach(var partner in partners)
                {
                    var sociailMediaViewModel = new SocialMediaViewModel();

                    sociailMediaViewModel.TwitterURL = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerTwitter);
                    sociailMediaViewModel.FacebookURL = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerFacebook);
                    sociailMediaViewModel.InstagramURL = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerInstagram);
                    sociailMediaViewModel.YoutubeURL = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerYoutube);
                    sociailMediaViewModel.LinkedInURL = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerLinkedIn);

                    model.Content.Add(new ContentViewModel
                    {
                        Country = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerCountry),
                        StreetAddress = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerStreetAddress),
                        City = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerCity),
                        Email = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerEmail),
                        Name = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerName),
                        ZipCode = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerZipcode),
                        PhoneNumber = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerPhone),
                        Website = partner.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.PartnerWebsite),
                        SocialMedia = sociailMediaViewModel
                    });
                }
            }
            else
            {
                var startPage = _pageService.GetChildPages(Guid.Empty, website.SystemId).FirstOrDefault();
                var channelsActive = new List<Guid>();

                model.PageTitle = website.Texts.GetValue("contactus.channelsaccordion.header");

                // also check dates if set
                foreach (var cl in startPage.ChannelLinks)
                {
                    if (cl.StartDateTimeUtc.HasValue && cl.StartDateTimeUtc.Value.CompareTo(DateTime.UtcNow) > 0)
                        continue;

                    if (cl.EndDateTimeUtc.HasValue && cl.EndDateTimeUtc.Value.CompareTo(DateTime.UtcNow) <= 0)
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

                    var sociailMediaViewModel = new SocialMediaViewModel();

                    sociailMediaViewModel.TwitterURL = channelModel.GetValue<string>(ChannelFieldNameConstants.Twitter);
                    sociailMediaViewModel.FacebookURL = channelModel.GetValue<string>(ChannelFieldNameConstants.Facebook);
                    sociailMediaViewModel.InstagramURL = channelModel.GetValue<string>(ChannelFieldNameConstants.Instagram);
                    sociailMediaViewModel.YoutubeURL = channelModel.GetValue<string>(ChannelFieldNameConstants.Youtube);
                    sociailMediaViewModel.LinkedInURL = channelModel.GetValue<string>(ChannelFieldNameConstants.LinkedIn);

                    model.Content.Add(new ContentViewModel
                    {
                        Country = channelModel.GetValue<string>(FieldFramework.SystemFieldDefinitionConstants.Name),
                        StreetAddress = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactStreet),
                        City = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactCity),
                        Email = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactEmail),
                        Name = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactName),
                        ZipCode = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactZipCode),
                        PhoneNumber = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactPhone),
                        Website = channelModel.GetValue<string>(ChannelFieldNameConstants.ContactWebsite),
                        SocialMedia = sociailMediaViewModel
                    });
                }
            }

            // order by name
            model.Content = model.Content.OrderBy(x => x.Country).ToList();

            return model;
        }
    }
}
