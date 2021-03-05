using Litium.Web.Models.Websites;
using System.Web.Mvc;
using Litium.Accelerator.Builders.LandingPage;

namespace Litium.Accelerator.Mvc.Controllers.LandingPage
{
    public class LandingPageController : ControllerBase
    {
        private readonly LandingPageViewModelBuilder _landingPageViewModelBuilder;

        public LandingPageController(LandingPageViewModelBuilder landingPageViewModelBuilder)
        {
            _landingPageViewModelBuilder = landingPageViewModelBuilder;
        }

        [HttpGet]
        public ActionResult Category(PageModel currentPageModel)
        {
            var model = _landingPageViewModelBuilder.Build(currentPageModel);
            return View("CategoryLandingPage", model);
        }
    }
}