using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.Dealer
{
    public class DealerListViewModel : IViewModel, IAutoMapperConfiguration
    {
        public string PageTitle { get; set; }

        public List<IGrouping<char, DealerItemViewModel>> Dealers { get; set; }

        [UsedImplicitly]
        public void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageModel, DealerListViewModel>()
                 .ForMember(x => x.PageTitle, m => m.MapFromField(PageFieldNameConstants.Title));
        }
    }

}
   
