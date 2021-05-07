﻿using System.Collections.Generic;
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
            var model = new FooterViewModel();
            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            var channel = _requestModelAccessor.RequestModel.ChannelModel;

            var footer = website.GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Footer);
            model.TopText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterTopText) ?? string.Empty;
            model.GetOrganised = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.FooterGetOrganised)?.MapTo<LinkModel>();
            model.Newsletter = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.FooterNewsletter)?.MapTo<LinkModel>();
            model.SocialMediaText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterSocialMediaText) ?? string.Empty;
            model.VisionHeader = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionHeader) ?? string.Empty;
            model.VisionText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionText) ?? string.Empty;
            model.Legal = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterLegal) ?? string.Empty;

            model.SocialMediaLinks.TwitterURL = channel.GetValue<string>(ChannelFieldNameConstants.Twitter) ?? string.Empty;
            model.SocialMediaLinks.FacebookURL = channel.GetValue<string>(ChannelFieldNameConstants.Facebook) ?? string.Empty;
            model.SocialMediaLinks.InstagramURL = channel.GetValue<string>(ChannelFieldNameConstants.Instagram) ?? string.Empty;
            model.SocialMediaLinks.YoutubeURL = channel.GetValue<string>(ChannelFieldNameConstants.Youtube) ?? string.Empty;
            model.SocialMediaLinks.LinkedInURL = channel.GetValue<string>(ChannelFieldNameConstants.LinkedIn) ?? string.Empty;

            if (footer != null)
            {
                model.SectionList = footer.Select(c => c.MapTo<SectionModel>()).ToList();                
            }

            return model;
        }
    }
}