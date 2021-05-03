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
        private readonly VariantService _variantService;
        private readonly SecurityContextService _securityContextService;

        public BaseProductEventBroker(EventBroker eventBroker,
             BaseProductService baseProductService,
             VariantService variantService,
             CategoryService categoryService,
             SecurityContextService securityContextService)
        {
            _baseProductService = baseProductService;
            _variantService = variantService;
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
            
        }

        public void Dispose()
        {
            // unregister the event from event broker
            _createdSubscription.Dispose();
            _updatedSubscription.Dispose();
        }
    }
}
