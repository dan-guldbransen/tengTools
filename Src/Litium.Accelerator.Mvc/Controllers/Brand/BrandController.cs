using Litium.Web.Models.Websites;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Brand;

namespace Litium.Accelerator.Mvc.Controllers.Brand
{
    public class BrandController : ControllerBase
    {
        private readonly BrandViewModelBuilder _brandViewModelBuilder;
        private readonly BrandListViewModelBuilder _brandListViewModelBuilder;
        
        public BrandController(BrandViewModelBuilder brandViewModelBuilder, BrandListViewModelBuilder brandListViewModelBuilder)
        {
            _brandViewModelBuilder = brandViewModelBuilder;
            _brandListViewModelBuilder = brandListViewModelBuilder;
        }

        [HttpGet]
        public ActionResult Index(PageModel currentPageModel)
        {
            var model = _brandViewModelBuilder.Build(currentPageModel);
            return View(model);
        }

        [HttpGet]
        public ActionResult List(PageModel currentPageModel)
        {
            var model = _brandListViewModelBuilder.Build(currentPageModel);
            return View(model);
        }
    }
}