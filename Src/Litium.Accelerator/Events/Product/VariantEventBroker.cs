using Litium.Events;
using Litium.Globalization;
using Litium.Products;
using Litium.Products.Events;
using Litium.Security;
using Litium.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Events.Product
{
    public class VariantEventBroker
    {
        private readonly Services.ProductService _productService;
        private readonly ChannelService _channelService;
        private readonly SlugifyService _slugService;
        private readonly SecurityContextService _securityContextService;

        public VariantEventBroker(EventBroker eventBroker,
            Services.ProductService productService,
            ChannelService channelService,
            SlugifyService slugService,
            SecurityContextService securityContextService)
        {

            _productService = productService;
            _channelService = channelService;
            _slugService = slugService;
            _securityContextService = securityContextService;

            eventBroker.Subscribe<VariantCreated>(variant => SetupNewVariant(variant));
            eventBroker.Subscribe<VariantUpdated>(variant => SetupUpdatedVariant(variant));
            
        }

        private void SetupNewVariant(VariantCreated variant)
        {
            throw new NotImplementedException();
        }

        private void SetupUpdatedVariant(VariantUpdated variant)
        {
            throw new NotImplementedException();
        }
    }
}
