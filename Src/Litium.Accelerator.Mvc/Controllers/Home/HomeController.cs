using Litium.Security;
using Litium.Web.Models.Websites;
using System;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Home;

namespace Litium.Accelerator.Mvc.Controllers.Home
{
    public class HomeController : ControllerBase
    {
        private readonly HomeViewModelBuilder _builder;
        private readonly AuthorizationService _authorizationService;

        public HomeController(HomeViewModelBuilder builder, AuthorizationService authorizationService)
        {
            _builder = builder;
            _authorizationService = authorizationService;
        }

        public ActionResult Index(PageModel currentPageModel)
        {
            var previewBlockId = Request.QueryString["previewGlobalBlock"];
            if (!string.IsNullOrEmpty(previewBlockId) && Guid.TryParse(previewBlockId, out var blockId) && _authorizationService.HasOperation(Operations.Function.Websites.UI))
            {
                return View("Index", _builder.ForPreviewGlobalBlock(blockId));
            }
            return View(_builder.Build(currentPageModel));
        }
    }
}