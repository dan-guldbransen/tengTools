using System.Web.Mvc;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Builders.Search;

namespace Litium.Accelerator.Mvc.Controllers.Navigation
{
    public class NavigationController : ControllerBase
    {
        private readonly SubNavigationViewModelBuilder _subNavigationViewModelBuilder;
        private readonly CategoryFilteringViewModelBuilder _categoryFilteringViewModelBuilder;

        public NavigationController(SubNavigationViewModelBuilder subNavigationViewModelBuilder, CategoryFilteringViewModelBuilder categoryFilteringViewModelBuilder)
        {
            _subNavigationViewModelBuilder = subNavigationViewModelBuilder;
            _categoryFilteringViewModelBuilder = categoryFilteringViewModelBuilder;
        }

        [ChildActionOnly]
        public ActionResult SubNavigationCategory()
        {
            return PartialView(_subNavigationViewModelBuilder.Build());
        }

        public ActionResult CategoryFiltering(int totalHits = 0)
        {
            return PartialView(_categoryFilteringViewModelBuilder.Build(totalHits));
        }
    }
}
