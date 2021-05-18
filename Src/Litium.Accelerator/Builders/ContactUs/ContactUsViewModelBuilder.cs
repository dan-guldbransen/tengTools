using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.ContactUs;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Builders.ContactUs
{
    public class ContactUsViewModelBuilder : IViewModelBuilder<ContactUsViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public ContactUsViewModelBuilder(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public virtual ContactUsViewModel Build(PageModel pageModel )
        {
            var model = pageModel.MapTo<ContactUsViewModel>();
            var channel = _requestModelAccessor.RequestModel.ChannelModel;

            var sociailMediaViewModel = new SocialMediaViewModel();
            sociailMediaViewModel.TwitterURL = channel.GetValue<string>(ChannelFieldNameConstants.Twitter);
            sociailMediaViewModel.FacebookURL = channel.GetValue<string>(ChannelFieldNameConstants.Facebook);
            sociailMediaViewModel.InstagramURL = channel.GetValue<string>(ChannelFieldNameConstants.Instagram);
            sociailMediaViewModel.YoutubeURL = channel.GetValue<string>(ChannelFieldNameConstants.Youtube);
            sociailMediaViewModel.LinkedInURL = channel.GetValue<string>(ChannelFieldNameConstants.LinkedIn);
            model.SocialMedia = sociailMediaViewModel;

            return model;
        }
    }

}
