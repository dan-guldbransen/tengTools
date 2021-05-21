using Litium.Accelerator.ViewModels.Dealer;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;
using System.Collections.Generic;
using System.Linq;
namespace Litium.Accelerator.Builders.Dealer
{
    public class DealerListViewModelBuilder : IViewModelBuilder<DealerListViewModel>
    {
        public DealerListViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<DealerListViewModel>();
            
            var dealers = new List<DealerItemViewModel>();

            if(dealers != null && dealers.Any())
            {
                model.Dealers = dealers;
            }

            return model;
        }

        // For backoffice list
        public DealerListViewModel Build()
        {
            return new DealerListViewModel();
        }
    }
}
