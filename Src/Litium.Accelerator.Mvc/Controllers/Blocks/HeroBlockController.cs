using Litium.Accelerator.Builders.Block;
using Litium.Web.Models.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Litium.Accelerator.Mvc.Controllers.Blocks
{
    public class HeroBlockController : ControllerBase
    {
        private readonly HeroBlockViewModelBuilder _builder;

        public HeroBlockController(HeroBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        [HttpGet]
        public ActionResult Index(BlockModel currentBlockModel)
        {
            var model = _builder.Build(currentBlockModel);
            return PartialView("~/Views/Block/Hero.cshtml", model);
        }
    }
}
