using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litium.Accelerator.Routing;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime;
using Litium.Web;
using Litium.Websites;

namespace Litium.Accelerator.Events
{
    [Autostart]
    public class CreateCategoryEventBroker
    {
        private readonly UrlService _urlService;
        private readonly Litium.Events.ISubscription<Products.Events.BaseProductCreated> _subscription;
        private readonly CategoryService _categoryService;
        private readonly BaseProductService _baseProductService;

        public CreateCategoryEventBroker(Litium.Events.EventBroker eventBroker, UrlService urlService, CategoryService categoryService, BaseProductService baseProductService)
        {
            _baseProductService = baseProductService;
            _urlService = urlService;
            _categoryService = categoryService;
            _subscription = eventBroker.Subscribe<Products.Events.BaseProductCreated>(baseProductCreated =>
            {
                CreateUrl(baseProductCreated.Item);
            });
        }

        private void CreateUrl(BaseProduct product)
        {
            BaseProduct product2 = GetProduct();

            int level = 2;
            string url = "";
            string baseUrl = "";
            string level1Url = "";
            string level2Url = "";
            string level3Url = "";

                switch (level)
                {
                    case 1:
                        url = $"{baseUrl}/{level1Url}/";
                        break;

                    case 2:
                        url = $"{baseUrl}/{level1Url}/{level2Url}/";
                        break;

                    default:
                        url = $"{baseUrl}/{level1Url}/{level2Url}/{level3Url}/";
                        break;
                }
        }


        private void GetCategory(Guid categoryId)
        {
            var category = _categoryService.GetChildCategories(categoryId);
           
        }

        private BaseProduct GetProduct()
        {
            BaseProduct product = _baseProductService.Get("45D1EE77-675F-417B-A3F0-7687D1505688");
            return product;
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
