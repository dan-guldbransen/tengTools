using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Dealer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Litium.Accelerator.Mvc.Controllers.Dealers
{
    public class DealerController : Controller
    {
        private readonly DealerService _dealerService;

        public DealerController(DealerService dealerService)
        {
            _dealerService = dealerService;
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
            bool isExcel = false;
            var ext = Path.GetExtension(model.ImportForm.DealerFile.FileName);
            switch (ext)   
            {
                case ".xls":
                    isExcel =true;
                    break;
                case ".xlsx":
                    isExcel = true;
                    break;
            }

            if (!ModelState.IsValid || !isExcel)
            {
                return View(nameof(Index), model);
            }
            else
            {
                string filePath = "";
                filePath = Server.MapPath("~/Src/Files/");
                var result = _dealerService.SaveFile(model, filePath);
            }
               
            model.ImportMessage = "Done";
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportFile()
        {
            var fileName = "Dealers.xlsx";
            string filePath = "";
            filePath = Server.MapPath("~/Src/Files/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
