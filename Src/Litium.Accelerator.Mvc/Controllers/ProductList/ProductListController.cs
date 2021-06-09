using System.Web.Mvc;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.ViewModels.Product;

namespace Litium.Accelerator.Mvc.Controllers.ProductList
{
    public class ProductListController : ControllerBase
    {
        private readonly ProductListViewModelBuilder _productListViewModelBuilder;

        public ProductListController(ProductListViewModelBuilder productListViewModelBuilder)
        {
            _productListViewModelBuilder = productListViewModelBuilder;
        }

        public ActionResult Index()
        {
            var model = _productListViewModelBuilder.Build();
            return View(model);
        }
    }
}