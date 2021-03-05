using Litium.Accelerator.Utilities;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Security;

namespace Litium.Accelerator.Services
{
    public class CustomerInfoFactory : Foundation.Modules.ECommerce.Plugins.CustomerInfo.CustomerInfoFactory
    {
        private readonly PersonStorage _personStorage;

        public CustomerInfoFactory(PersonStorage personStorage)
        {
            _personStorage = personStorage;
        }

        public override CustomerInfoCarrier Create(SecurityToken token)
        {
            var carrier = base.Create(token);

            var organization = _personStorage.CurrentSelectedOrganization;
            if (organization is object)
            {
                carrier.OrganizationID = organization.SystemId;
                if (carrier.Address != null)
                {
                    carrier.Address.OrganizationName = organization.Name;
                }
            }

            return carrier;
        }
    }
}
