using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.ContactUs;
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
            var channel = _requestModelAccessor.RequestModel.ChannelModel;
            var model = pageModel.MapTo<ContactUsViewModel>();
            model.CurrentChannelContactInformation = new ChannelContactInformation();

            model.CurrentChannelContactInformation.ContactName =  channel.GetValue<string>(ChannelFieldNameConstants.ContactName);
            model.CurrentChannelContactInformation.ContactStreet =  channel.GetValue<string>(ChannelFieldNameConstants.ContactStreet);
            model.CurrentChannelContactInformation.ContactAddressLine2 =  channel.GetValue<string>(ChannelFieldNameConstants.ContactAddressLine2);
            model.CurrentChannelContactInformation.ContactZipCode =  channel.GetValue<string>(ChannelFieldNameConstants.ContactZipCode);
            model.CurrentChannelContactInformation.ContactCity =  channel.GetValue<string>(ChannelFieldNameConstants.ContactCity);
            model.CurrentChannelContactInformation.ContactCounty =  channel.GetValue<string>(ChannelFieldNameConstants.ContactCounty);
            model.CurrentChannelContactInformation.ContactPhone =  channel.GetValue<string>(ChannelFieldNameConstants.ContactPhone);
            model.CurrentChannelContactInformation.ContactEmails =  channel.GetValue<string>(ChannelFieldNameConstants.ContactEmail);
            model.CurrentChannelContactInformation.ContactWebsite =  channel.GetValue<string>(ChannelFieldNameConstants.ContactWebsite);
            model.CurrentChannelContactInformation.ContactLat =  channel.GetValue<string>(ChannelFieldNameConstants.ContactLat);
            model.CurrentChannelContactInformation.ContactLong =  channel.GetValue<string>(ChannelFieldNameConstants.ContactLong);
            model.CurrentChannelContactInformation.ContactTwitterURL =  channel.GetValue<string>(ChannelFieldNameConstants.Twitter);
            model.CurrentChannelContactInformation.ContactYoutubeURL =  channel.GetValue<string>(ChannelFieldNameConstants.Youtube);
            model.CurrentChannelContactInformation.ContactLinkedInURL =  channel.GetValue<string>(ChannelFieldNameConstants.LinkedIn);
            model.CurrentChannelContactInformation.ContactInstagremURL =  channel.GetValue<string>(ChannelFieldNameConstants.Instagram);
            model.CurrentChannelContactInformation.ContactFacebookURL =  channel.GetValue<string>(ChannelFieldNameConstants.Facebook);

            return model;
        }
    }

}
