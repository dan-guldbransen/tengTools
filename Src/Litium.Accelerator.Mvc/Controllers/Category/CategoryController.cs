using System.Collections.Generic;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Product;
using Litium.Web.Models.Products;
using Litium.Web.Rendering;

namespace Litium.Accelerator.Mvc.Controllers.Category
{
    public class CategoryController : ControllerBase
    {
        private readonly CategoryPageViewModelBuilder _categoryPageViewModelBuilder;
        private readonly ChildCategoryNavigationBuilder _childCategoryNavigationBuilder;
        private readonly ICollection<IRenderingValidator<Products.Category>> _renderingValidators;

        public CategoryController(
            CategoryPageViewModelBuilder categoryPageViewModelBuilder,
            ChildCategoryNavigationBuilder childCategoryNavigationBuilder,
            ICollection<IRenderingValidator<Products.Category>> renderingValidators)
        {
            _childCategoryNavigationBuilder = childCategoryNavigationBuilder;
            _categoryPageViewModelBuilder = categoryPageViewModelBuilder;
            _renderingValidators = renderingValidators;
        }

        public ActionResult Index(CategoryModel currentCategoryModel)
        {
            if (!_renderingValidators.Validate(currentCategoryModel.Category))
            {
                return HttpNotFound();
            }

            var model = _categoryPageViewModelBuilder.Build(currentCategoryModel.SystemId);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ChildCategoryNavigation(CategoryModel currentCategory)
        {
            var model = _childCategoryNavigationBuilder.Build(currentCategory);
            if (model != null)
            {
                return PartialView(model);
            }

            return null;
        }
    }
}
