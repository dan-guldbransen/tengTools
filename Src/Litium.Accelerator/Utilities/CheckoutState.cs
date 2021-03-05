using System;
using System.Web;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Utilities
{
    [Service(ServiceType = typeof(CheckoutState), Lifetime = DependencyLifetime.Singleton)]
    public class CheckoutState
    {
        private readonly SessionStorage _sessionStorage;

        public CheckoutState(
            SessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public AddressCarrier Address
        {
            get => _sessionStorage.GetValue<AddressCarrier>("CheckoutAddress");
            set => _sessionStorage.SetValue("CheckoutAddress", value);
        }

        public bool AlternativeAddressEnabled
        {
            get => _sessionStorage.GetValue<bool?>("CheckoutAlternativeAddressEnabled").GetValueOrDefault();
            set => _sessionStorage.SetValue("CheckoutAlternativeAddressEnabled", value);
        }

        public string Comments
        {
            get => _sessionStorage.GetValue<string>("CheckoutComments") ?? string.Empty;
            set => _sessionStorage.SetValue("CheckoutComments", value);
        }

        public string CustomerSsn
        {
            get => _sessionStorage.GetValue<string>("CustomerSsn") ?? string.Empty;
            set => _sessionStorage.SetValue("CustomerSsn", value);
        }
        public AddressCarrier DeliveryAddress
        {
            get => _sessionStorage.GetValue<AddressCarrier>("CheckoutDeliveryAddress");
            set => _sessionStorage.SetValue("CheckoutDeliveryAddress", value);
        }
        public bool NeedToRegister
        {
            get => _sessionStorage.GetValue<bool?>("CheckoutNeedToRegister").GetValueOrDefault();
            set => _sessionStorage.SetValue("CheckoutNeedToRegister", value);
        }

        public void ClearState()
        {
            Address = null;
            DeliveryAddress = null;
            AlternativeAddressEnabled = false;
            Comments = null;
            CustomerSsn = null;
        }

        public void CopyAddressValues(AddressCarrier addressSource, AddressCarrier deliveryAddressSource)
        {
            CopyAddressValues(addressSource);
            CopyDeliveryAddressValues(deliveryAddressSource);
        }

        public void CopyAddressValues(AddressCarrier source)
        {
            if (Address == null)
            {
                Address = new AddressCarrier();
            }
            CopyAddress(source, Address);
        }

        public void CopyDeliveryAddressValues(AddressCarrier source)
        {
            if (DeliveryAddress == null)
            {
                DeliveryAddress = new AddressCarrier();
            }
            CopyDeliveryAddress(source, DeliveryAddress);
        }

        private static void CopyAddress(AddressCarrier source, AddressCarrier target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Email = source.Email ?? target.Email;
            target.Fax = source.Fax ?? target.Fax;
            target.MobilePhone = source.MobilePhone ?? target.MobilePhone;
            target.OrganizationName = source.OrganizationName ?? target.OrganizationName;
            target.Phone = source.Phone ?? target.Phone;

            target.FirstName = source.FirstName ?? target.FirstName;
            target.LastName = source.LastName ?? target.LastName;

            target.CareOf = source.CareOf;
            target.Address1 = source.Address1;
            target.Address2 = source.Address2;
            target.CareOf = source.CareOf;
            target.City = source.City;
            target.Country = source.Country;
            target.State = source.State;
            target.Zip = source.Zip;
        }

        private static void CopyDeliveryAddress(AddressCarrier source, AddressCarrier target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Email = source.Email ?? target.Email;
            target.Fax = source.Fax ?? target.Fax;
            target.MobilePhone = source.MobilePhone ?? target.MobilePhone;
            target.OrganizationName = source.OrganizationName ?? target.OrganizationName;
            target.Phone = source.Phone ?? target.Phone;

            target.FirstName = source.FirstName ?? target.FirstName;
            target.LastName = source.LastName ?? target.LastName;

            target.CareOf = source.CareOf;
            target.Address1 = source.Address1;
            target.Address2 = source.Address2;
            target.City = source.City;
            target.Country = source.Country;
            target.State = source.State;
            target.Zip = source.Zip;
        }
    }
}
