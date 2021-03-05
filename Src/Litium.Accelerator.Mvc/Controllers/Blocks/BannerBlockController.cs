using System.Web.Mvc;
using Litium.Accelerator.Builders.Block;
using Litium.Web.Models.Blocks;

namespace Litium.Accelerator.Mvc.Controllers.Blocks
{
    public class BannerBlockController : ControllerBase
    {
        private readonly BannersBlockViewModelBuilder _builder;

        public BannerBlockController(BannersBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult Index(BlockModel currentBlockModel)
        {
            var model = _builder.Build(currentBlockModel);
            return PartialView("~/Views/Block/Banners.cshtml", model);
        }
    }
}