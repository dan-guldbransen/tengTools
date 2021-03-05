using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Constants;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using Litium.Web.Models.Websites;
using Litium.Accelerator.Builders;
using Litium.FieldFramework.FieldTypes;

namespace Litium.Accelerator.ViewModels.MyPages
{
    public class OrderHistoryPanelViewModel : IAutoMapperConfiguration, IViewModel
    {
        public LinkModel OrderHistoryLink { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageModel, OrderHistoryPanelViewModel>()
               .ForMember(x => x.OrderHistoryLink, m => m.MapFrom(myPagesPage => myPagesPage.GetValue<PointerPageItem>(PageFieldNameConstants.OrderHistoryLink).MapTo<LinkModel>()));
        }
    }
}


