using System.Web.Http;
using Litium.Accelerator.Builders.Framework;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [RoutePrefix("api/navigation")]
    public class NavigationController : ApiControllerBase
    {
        private readonly NavigationViewModelBuilder _navigationViewModelBuilder;

        public NavigationController(NavigationViewModelBuilder navigationViewModelBuilder)
        {
            _navigationViewModelBuilder = navigationViewModelBuilder;
        }

        /// <summary>
        /// Get navigation menu.
        /// </summary>
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(_navigationViewModelBuilder.Build());
        }
    }
}
