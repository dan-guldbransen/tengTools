using Litium.Web.Models.Websites;
using Litium.Web.Routing;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Error;

namespace Litium.Accelerator.Mvc.Controllers.Error
{
    public class ErrorController : ControllerBase
    {
        private readonly ErrorViewModelBuilder _errorViewModelBuilder;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;

        public ErrorController(ErrorViewModelBuilder errorViewModelBuilder, RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor)
        {
            _errorViewModelBuilder = errorViewModelBuilder;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
        }

        [HttpGet]
        public ActionResult Error(PageModel currentPageModel)
        {
            if (!_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsInAdministration)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.Status = "500 Internal Server Error";
                Response.TrySkipIisCustomErrors = true;

                // Add random sleep due to Microsoft Security Advisory (2416728), obfuscate error time
                var delay = new byte[1];
                using (RandomNumberGenerator prng = new RNGCryptoServiceProvider())
                {
                    prng.GetBytes(delay);
                }
                Thread.Sleep(delay[0]);
            }
            var model = _errorViewModelBuilder.Build(currentPageModel);
            return View(model);
        }

        [HttpGet]
        public ActionResult NotFound(PageModel currentPageModel)
        {
            if (!_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsInAdministration)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.Status = "404 Not Found";
                Response.TrySkipIisCustomErrors = true;
            }
            var model = _errorViewModelBuilder.Build(currentPageModel);
            return View(model);
        }
    }
}