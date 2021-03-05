using System.Web.Mvc;
using Litium.Accelerator.Builders.Block;
using Litium.Web.Models.Blocks;

namespace Litium.Accelerator.Mvc.Controllers.Blocks
{
    public class ProductsAndBannerBlockController : ControllerBase
    {
        private readonly ProductsAndBannerBlockViewModelBuilder _builder;

        public ProductsAndBannerBlockController(ProductsAndBannerBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult Index(BlockModel currentBlockModel)
        {
            var model = _builder.Build(currentBlockModel);
            return PartialView("~/Views/Block/ProductsAndBanner.cshtml", model);
        }
    }
}