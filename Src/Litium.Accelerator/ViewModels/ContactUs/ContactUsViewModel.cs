using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Blocks;
using Litium.Web.Models.Websites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.ContactUs
{
    public class ContactUsViewModel : IViewModel, IAutoMapperConfiguration
    {
        public string Introduction { get; set; }
        public string Title { get; set; }

        [Required]
        public string ContacterName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string ContacterEmail { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string ContacterPhone { get; set; }

        [Required]
        public string ContacterMessage { get; set; }

        public string ThankYouMessage { get; set; }

        [UsedImplicitly]
        public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageModel, ContactUsViewModel>()
                 .ForMember(x => x.Introduction, m => m.MapFromField(PageFieldNameConstants.Introduction))
                 .ForMember(x => x.Title, m => m.MapFromField(PageFieldNameConstants.Title))
                 .ForMember(x => x.ContacterEmail, m => m.MapFromField(ContactUsFieldNameConstants.ContactUsEmail))
                 .ForMember(x => x.ContacterName, m => m.MapFromField(ContactUsFieldNameConstants.ContactUsName))
                 .ForMember(x => x.ContacterPhone, m => m.MapFromField(ContactUsFieldNameConstants.ContactUsPhone))
                 .ForMember(x => x.ContacterMessage, m => m.MapFromField(ContactUsFieldNameConstants.ContactUsMessage));
        }
    }
}


