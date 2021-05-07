using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Models;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;

namespace Litium.Accelerator.Builders.Framework
{
    public class FooterViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : FooterViewModel
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public FooterViewModelBuilder(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public FooterViewModel Build()
        {
            var website = _requestModelAccessor.RequestModel.WebsiteModel;

            var model = new FooterViewModel
            {
                Logo = website.GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.LogotypeAlt)?.MapTo<ImageModel>(),
                TopText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterTopText),
                GetOrganised = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.FooterGetOrganised)?.MapTo<LinkModel>(),
                Newsletter = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.FooterNewsletter)?.MapTo<LinkModel>(),
                SocialMediaText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterSocialMediaText),
                VisionHeader = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionHeader),
                VisionText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionText),
                Legal = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterLegal)
            };

            var channel = _requestModelAccessor.RequestModel.ChannelModel;
            
            model.SocialMediaLinks.TwitterURL = channel.GetValue<string>(ChannelFieldNameConstants.Twitter);
            model.SocialMediaLinks.FacebookURL = channel.GetValue<string>(ChannelFieldNameConstants.Facebook);
            model.SocialMediaLinks.InstagramURL = channel.GetValue<string>(ChannelFieldNameConstants.Instagram);
            model.SocialMediaLinks.YoutubeURL = channel.GetValue<string>(ChannelFieldNameConstants.Youtube);
            model.SocialMediaLinks.LinkedInURL = channel.GetValue<string>(ChannelFieldNameConstants.LinkedIn);

            var footer = website.GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Footer);
            if (footer != null)
            {
                model.SectionList = footer.Select(c => c.MapTo<SectionModel>()).ToList();
            }

            return model;
        }
    }
}