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

            for (int i = 1; i < 6; i++)
            {
                //var p = new ProductItemViewModel()
                //{
                //    Name = $"Spanner no.5_{i}",
                //    Url = "Testnamn_url",
                //    Id = $"55_{i}",
                //    Description = "aj aj aj ",
                //    Price = new Web.Models.Products.ProductPriceModel() { Price = new Web.Models.Products.ProductPriceModel.PriceItem(1, 894 + i, 0, 894 + i), }
                //};

                var p = new ProductItemViewModel();
                p.Name = $"Spanner no.5_{i}";
                p.Url = "Testnamn_url";
                p.Id = $"55_{i}";
                p.Description = "aj aj aj ";
                p.Price = new Web.Models.Products.ProductPriceModel();
                p.Price.Price = new Web.Models.Products.ProductPriceModel.PriceItem(1, 894 + i, 0, 894 + i);
                //p.Id = "A94603DD-EC4E-4791-B91E-F3228AE301BC";
                model.Products.Add(p);
            }

            model.Title = "Spanners";
            model.Pagination = new ViewModels.PaginationViewModel(1, 1);


            return View(model);
        }
    }
}