using System.Web;
using System.Web.Http;
using Litium.Accelerator.Builders.Search;
using Litium.Accelerator.Mvc.Attributes;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [RoutePrefix("api/quicksearch")]
    public class QuickSearchController : ApiControllerBase
    {
        private readonly QuickSearchResultViewModelBuilder _quickSearchResultViewModelBuilder;

        public QuickSearchController(QuickSearchResultViewModelBuilder quickSearchResultViewModelBuilder)
        {
            _quickSearchResultViewModelBuilder = quickSearchResultViewModelBuilder;
        }

        /// <summary>
        /// Search all data including product, category, page.
        /// </summary>
        /// <param name="query">The query.</param>
        [HttpPost]
        [ApiValidateAntiForgeryToken]
        [Route]
        public IHttpActionResult Post([FromBody]string query)
        {
            var result = _quickSearchResultViewModelBuilder.Build(HttpUtility.UrlDecode(query));
            return Ok(result.Results);
        }
    }
}
