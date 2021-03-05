using System.Web.Mvc;
using Litium.Accelerator.Builders.Block;
using Litium.Web.Models.Blocks;

namespace Litium.Accelerator.Mvc.Controllers.Blocks
{
    /// <summary>
    /// Represents the controller for slider block.
    /// </summary>
    public class SliderBlockController : ControllerBase
    {
        private readonly SliderBlockViewModelBuilder _builder;

        public SliderBlockController(SliderBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult Index(BlockModel currentBlockModel)
        {
            var model = _builder.Build(currentBlockModel);
            return PartialView("~/Views/Block/Slider.cshtml", model);
        }
    }
}