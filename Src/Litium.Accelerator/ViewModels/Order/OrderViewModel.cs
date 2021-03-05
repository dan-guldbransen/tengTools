using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using Litium.Web.Models.Websites;

namespace Litium.Accelerator.ViewModels.Order
{
    public class OrderViewModel : IAutoMapperConfiguration, IViewModel
    {
        public string Title { get; set; }
        public string Introduction { get; set; }
        public EditorString Text { get; set; }

        public OrderDetailsViewModel Order { get; set; } = new OrderDetailsViewModel();

        public bool IsPrintPage { get; set; }
        public bool ShowButton { get => !IsPrintPage; }
        public string OrderHistoryUrl { get; set; }

        public bool IsBusinessCustomer { get; set; }
        public bool HasApproverRole { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageModel, OrderViewModel>()
               .ForMember(x => x.Title, m => m.MapFromField(PageFieldNameConstants.Title))
               .ForMember(x => x.Introduction, m => m.MapFromField(PageFieldNameConstants.Introduction))
               .ForMember(x => x.Text, m => m.MapFrom(orderPage => orderPage.GetValue<string>(PageFieldNameConstants.Text)));
        }
    }
}
