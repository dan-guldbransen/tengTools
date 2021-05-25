using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Dealer;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Websites;
using System.Collections.Generic;
using System.Linq;
namespace Litium.Accelerator.Builders.Dealer
{
    public class DealerListViewModelBuilder : IViewModelBuilder<DealerListViewModel>
    {
        private readonly DealerService _dealerService;

        public DealerListViewModelBuilder(DealerService dealerService)
        { 
            _dealerService = dealerService;
        }

        public DealerListViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<DealerListViewModel>();
            
            var dealers = GetDealers();

            if(dealers != null && dealers.Any())
            {
                model.Dealers = dealers.GroupBy(c => c.CompanyName.ToCharArray().First()).ToList();
            }

            return model;
        }

        // For backoffice list
        public List<DealerItemViewModel> Build()
        {
            return GetDealers().OrderBy(c => c.CompanyName).ToList();
        }

        private List<DealerItemViewModel> GetDealers()
        {
            var retval = new List<DealerItemViewModel>();
            var file = _dealerService.GetDealerFileBytes();
            


            return retval;
        }
    }
}
