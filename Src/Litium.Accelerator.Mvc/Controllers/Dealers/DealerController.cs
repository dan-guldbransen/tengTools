using Litium.Accelerator.ViewModels.Dealer;
using Litium.Web.Models.Websites;
﻿using Litium.Accelerator.Services;
using System.IO;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Dealer;
using System.Collections.Generic;
using Litium.Accelerator.Routing;

namespace Litium.Accelerator.Mvc.Controllers.Dealers
{
    public class DealerController : Controller
    {
        private readonly DealerService _dealerService;
        private readonly DealerListViewModelBuilder _dealerListViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;

        private static readonly List<string> _supportedTypes = new List<string> { "xlsx" };

        public DealerController(DealerService dealerService,
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
            return View(new DealerViewModel());
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

        public ActionResult DealerList(PageModel currentPageModel)
        {
            var model = _dealerListViewModelBuilder.Build(currentPageModel);
            return View(model);
        }
    }
}
