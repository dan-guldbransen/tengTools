using Litium.Accelerator.ViewModels.Dealer;
using Litium.Web.Models.Websites;
﻿using Litium.Accelerator.Services;
using System.IO;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Dealer;
using System.Collections.Generic;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Helpers;
using System.Linq;

namespace Litium.Accelerator.Mvc.Controllers.Dealers
{
    public class DealerAdminController : Controller
    {
        private readonly DealerService _dealerService;
        private readonly DealerListViewModelBuilder _dealerListViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;

        private static readonly List<string> _supportedTypes = new List<string> { "xlsx" };

        public DealerAdminController(DealerService dealerService,
            DealerListViewModelBuilder dealerListViewModelBuilder,
            RequestModelAccessor requestModelAccessor)
        {
            _dealerService = dealerService;
            _dealerListViewModelBuilder = dealerListViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
        }

        [Route("litium/dealers", Name = "DealerView")]
        public ActionResult Index()
        {
            var model = new DealerViewModel();
            model.Dealers = GetDealers();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportFile(DealerViewModel model)
        {
            
            if(!ModelState.IsValid)
                return View(nameof(Index), model);

            var fileExt = Path.GetExtension(model.ImportForm.DealerFile.FileName).Substring(1);
            if (!_supportedTypes.Contains(fileExt))
            {
                ModelState.AddModelError(nameof(model.ImportForm.DealerFile), "Invalid file extension");
            }

            model.Dealers = GetDealers();

            if (!ModelState.IsValid)           
                return View(nameof(Index), model);
            

            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            model.ImportMessage = _dealerService.SaveFile(model.ImportForm.DealerFile, website);

            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportFile()
        {
            var (bytes, fileName) = _dealerService.GetDealerFileBytes();

            if(bytes == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return File(bytes, "application/octet-stream", fileName);
        }

        private List<DealerItemViewModel> GetDealers()
        {
            var retval = new List<DealerItemViewModel>();
            var filePath = _dealerService.GetFilePath();

            if (System.IO.File.Exists(filePath))
            {
                var dataTable = ExcelHelpers.GetDataTable(filePath, true);

                if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in dataTable.Rows)
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

            return retval.OrderBy(c => c.CompanyName).ToList();
        }
    }
}
