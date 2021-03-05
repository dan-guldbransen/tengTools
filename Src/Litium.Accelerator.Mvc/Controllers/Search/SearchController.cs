using System.Web.Mvc;
using Litium.Accelerator.Builders.Search;

namespace Litium.Accelerator.Mvc.Controllers.Search
{
	public class SearchController : ControllerBase
	{
        private readonly SearchResultViewModelBuilder _searchResultViewModelBuilder;

        public SearchController(SearchResultViewModelBuilder searchResultViewModelBuilder)
        {
            _searchResultViewModelBuilder = searchResultViewModelBuilder;
        }

        public ActionResult Index()
		{
			return View(_searchResultViewModelBuilder.Build());
		}
	}
}