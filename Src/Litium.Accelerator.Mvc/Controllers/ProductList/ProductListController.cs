using System.Web.Mvc;
using Litium.Accelerator.Builders.Product;

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
            return View(_productListViewModelBuilder.Build());
        }
    }
}