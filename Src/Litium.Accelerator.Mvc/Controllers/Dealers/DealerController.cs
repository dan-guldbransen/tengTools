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
    public class DealerController : ControllerBase
    {
        private readonly DealerListViewModelBuilder _dealerListViewModelBuilder;
       
        public DealerController(DealerListViewModelBuilder dealerListViewModelBuilder)
        {
            _dealerListViewModelBuilder = dealerListViewModelBuilder;
        }
       
        public ActionResult Index(PageModel currentPageModel)
        {
            var model = _dealerListViewModelBuilder.Build(currentPageModel);
            return View(model);
        }
    }
}
