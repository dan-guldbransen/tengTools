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
            
            _updatedSubscription = eventBroker.Subscribe<BaseProductUpdated>(baseProductUpdatedEvent => SetupUpdatedBaseProduct(baseProductUpdatedEvent));
        }

        private void SetupUpdatedBaseProduct(BaseProductUpdated baseProductUpdatedEvent)
        {
            SetupCategories(baseProductUpdatedEvent.SystemId);
        }

        private void SetupCategories(Guid baseProductSystemId)
        {
            // update all variants if baseproduct category change
        }

        public void Dispose()
        {
            // unregister the event from event broker
            _updatedSubscription.Dispose();
        }
    }
}
