using Litium.Events;
using Litium.Products;
using Litium.Products.Events;
using Litium.Runtime;
using Litium.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Events.Product
{
    [Autostart]
    public class BaseProductEventBroker : IDisposable
    {
        private readonly ISubscription<BaseProductCreated> _createdSubscription;
        private readonly ISubscription<BaseProductUpdated> _updatedSubscription;

        private readonly CategoryService _categoryService;
        private readonly BaseProductService _baseProductService;
        private readonly SecurityContextService _securityContextService;

        public BaseProductEventBroker(EventBroker eventBroker,
             BaseProductService baseProductService,
             CategoryService categoryService,
             SecurityContextService securityContextService)
        {
            _baseProductService = baseProductService;
            _securityContextService = securityContextService;
            _categoryService = categoryService;

            _createdSubscription = eventBroker.Subscribe<BaseProductCreated>(baseProductCreatedEvent => SetupNewBaseProduct(baseProductCreatedEvent));
            _updatedSubscription = eventBroker.Subscribe<BaseProductUpdated>(baseProductUpdatedEvent => SetupUpdatedBaseProduct(baseProductUpdatedEvent));
        }

        private void SetupNewBaseProduct(BaseProductCreated baseProductCreatedEvent)
        {
            SetupCategories(baseProductCreatedEvent.SystemId);
        }

        private void SetupUpdatedBaseProduct(BaseProductUpdated baseProductUpdatedEvent)
        {
            SetupCategories(baseProductUpdatedEvent.SystemId);
        }

        private void SetupCategories(Guid baseProductSystemId)
        {
            // Set category link (on baseproduct)
            var baseProduct = _baseProductService.Get(baseProductSystemId);
            var productCategoryNumber = baseProduct.Fields.GetValue<string>("ProductCategoryNumber");
            var productGroupNumber = baseProduct.Fields.GetValue<string>("ProductGroupNumber");

            Products.Category mainCategory, subCategory;
            if (!string.IsNullOrEmpty(productCategoryNumber))
            {
                mainCategory = _categoryService.Get(productCategoryNumber);
                if (mainCategory != null)
                {
                    var mainClone = mainCategory.MakeWritableClone();
                    var link = new CategoryToProductLink(baseProduct.SystemId);

                    if (!mainClone.ProductLinks.Contains(link))
                    {
                        mainClone.ProductLinks.Add(new CategoryToProductLink(baseProduct.SystemId));

                        using (_securityContextService.ActAsSystem())
                            _categoryService.Update(mainClone);
                    }
                }
            }

            if (!string.IsNullOrEmpty(productGroupNumber))
            {
                subCategory = _categoryService.Get(productGroupNumber);
                if (subCategory != null)
                {
                    var subClone = subCategory.MakeWritableClone();
                    var link = new CategoryToProductLink(baseProduct.SystemId);

                    if (!subClone.ProductLinks.Contains(link))
                    {
                        subClone.ProductLinks.Add(link);

                        using (_securityContextService.ActAsSystem())
                            _categoryService.Update(subClone);
                    }
                }
            }
        }

        public void Dispose()
        {
            // unregister the event from event broker
            _createdSubscription.Dispose();
            _updatedSubscription.Dispose();
        }
    }
}
