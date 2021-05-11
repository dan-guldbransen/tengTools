using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;
using System.ComponentModel.DataAnnotations;


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
                 .ForMember(x => x.Title, m => m.MapFromField(PageFieldNameConstants.Title));
        }
    }
}