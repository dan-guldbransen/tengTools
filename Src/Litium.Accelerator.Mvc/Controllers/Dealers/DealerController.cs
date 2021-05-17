using Litium.Accelerator.ViewModels.Dealer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Litium.Accelerator.Mvc.Controllers.Dealers
{
    public class DealerController : Controller
    {
        [Route("litium/dealers", Name = "DealerView")]
        public ActionResult Index()
        {
            return View(new DealerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportFile(DealerViewModel model)
        {
            // TODO validate its an excelfile

            if (!ModelState.IsValid)
                return View(nameof(Index), model);

            model.ImportMessage = "Done";
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportFile()
        {
            return View(nameof(Index));
        }
    }
}
