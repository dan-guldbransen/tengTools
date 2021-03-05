using System.Web.Mvc;
using Litium.Accelerator.Builders.Block;
using Litium.Web.Models.Blocks;

namespace Litium.Accelerator.Mvc.Controllers.Blocks
{
    public class BrandBlockController : ControllerBase
    {
        private readonly BrandsBlockViewModelBuilder _builder;

        public BrandBlockController(BrandsBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult Index(BlockModel currentBlockModel)
        {
            var model = _builder.Build(currentBlockModel);
            return PartialView("~/Views/Block/Brands.cshtml", model);
        }
    }
}