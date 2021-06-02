using Litium.Accelerator.Helpers;
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

            if (dealers != null && dealers.Any())
            {
                model.Dealers = dealers.Where(c => !string.IsNullOrEmpty(c.CompanyName)).GroupBy(c => c.CompanyName.ToCharArray().First()).OrderBy(g => g.Key).ToList();
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
            var filePath = _dealerService.GetFilePath();

            if (System.IO.File.Exists(filePath))
            {
                var dataTable = ExcelHelpers.GetDataTable(filePath, true);

                if(dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    foreach(System.Data.DataRow row in dataTable.Rows)
                    {
                        var dealerItem = new DealerItemViewModel
                        {
                            DealerGroup = row.ItemArray[0].ToString(),
                            DealerNumber = row.ItemArray[1].ToString(),
                            CompanyName = row.ItemArray[2].ToString(),
                            StreetAdress = row.ItemArray[3].ToString(),
                            City = row.ItemArray[4].ToString(),
                            ZipCode = row.ItemArray[5].ToString(),
                            Email = row.ItemArray[6].ToString(),
                            Phone = row.ItemArray[7].ToString(),
                            Website = row.ItemArray[8].ToString(),
                            Ecom = row.ItemArray[9].ToString(),
                        };

                        retval.Add(dealerItem);
                    }
                }
            }

            return retval;
        }
    }
}
