using AutoMapper;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.ContactUs
{
    public class ContactInformationViewModel : IViewModel, IAutoMapperConfiguration
    {
        public string ContactName { get; set; }
        public string ContactStreet { get; set; }
        public string ContactZipCode { get; set; }
        public string ContactCity { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactWebsite { get; set; }
        public string ContactLat { get; set; }
        public string ContactLong { get; set; }
        public string ContactTwitterURL { get; set; }
        public string ContactFacebookURL { get; set; }
        public string ContactInstagramURL { get; set; }
        public string ContactLinkedInURL { get; set; }
        public string ContactYoutubeURL { get; set; }

        public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ChannelModel, ContactInformationViewModel>()
                 .ForMember(x => x.ContactName, m => m.MapFromField(ChannelFieldNameConstants.ContactName))
                 .ForMember(x => x.ContactStreet, m => m.MapFromField(ChannelFieldNameConstants.ContactStreet))
                 .ForMember(x => x.ContactZipCode, m => m.MapFromField(ChannelFieldNameConstants.ContactZipCode))
                 .ForMember(x => x.ContactCity, m => m.MapFromField(ChannelFieldNameConstants.ContactCity))
                 .ForMember(x => x.ContactPhone, m => m.MapFromField(ChannelFieldNameConstants.ContactPhone))
                 .ForMember(x => x.ContactEmail, m => m.MapFromField(ChannelFieldNameConstants.ContactEmail))
                 .ForMember(x => x.ContactWebsite, m => m.MapFromField(ChannelFieldNameConstants.ContactWebsite))
                 .ForMember(x => x.ContactLat, m => m.MapFromField(ChannelFieldNameConstants.ContactLat))
                 .ForMember(x => x.ContactLong, m => m.MapFromField(ChannelFieldNameConstants.ContactLong))
                 .ForMember(x => x.ContactTwitterURL, m => m.MapFromField(ChannelFieldNameConstants.Twitter))
                 .ForMember(x => x.ContactFacebookURL, m => m.MapFromField(ChannelFieldNameConstants.Facebook))
                 .ForMember(x => x.ContactInstagramURL, m => m.MapFromField(ChannelFieldNameConstants.Instagram))
                 .ForMember(x => x.ContactLinkedInURL, m => m.MapFromField(ChannelFieldNameConstants.LinkedIn))
                 .ForMember(x => x.ContactYoutubeURL, m => m.MapFromField(ChannelFieldNameConstants.Youtube));
        }
    }
}
