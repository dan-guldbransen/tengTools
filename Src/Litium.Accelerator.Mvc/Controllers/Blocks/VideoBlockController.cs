using System.Web.Mvc;
using Litium.Accelerator.Builders.Block;
using Litium.Web.Models.Blocks;

namespace Litium.Accelerator.Mvc.Controllers.Blocks
{
    /// <summary>
    /// Represents the controller for video block.
    /// </summary>
    public class VideoBlockController : ControllerBase
    {
        private readonly VideoBlockViewModelBuilder _builder;

        public VideoBlockController(VideoBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult Index(BlockModel currentBlockModel)
        {
            var model = _builder.Build(currentBlockModel);
            return PartialView("~/Views/Block/Video.cshtml", model);
        }
    }
}