using System;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.Accelerator.ViewModels.Persons;
using Litium.Customers;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Web.Models;
using Litium.Web.Models.Websites;
using Litium.Web.Routing;

namespace Litium.Accelerator.Builders.Checkout
{
    public class CheckoutViewModelBuilder : IViewModelBuilder<CheckoutViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly SecurityContextService _securityContextService;
        private readonly DeliveryMethodViewModelBuilder _deliveryMethodViewModelBuilder;
        private readonly PaymentMethodViewModelBuilder _paymentMethodViewModelBuilder;
        private readonly SecurityToken _securityToken;
        private readonly PersonService _personService;
        private readonly AddressTypeService _addressTypeService;
        private readonly PersonStorage _personStorage;
        private readonly CheckoutState _checkoutState;
        private readonly ISignInUrlResolver _signInUrlResolver;
        private readonly CountryService _countryService;

        public CheckoutViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            SecurityContextService securityContextService,
            DeliveryMethodViewModelBuilder deliveryMethodViewModelBuilder,
            PaymentMethodViewModelBuilder paymentMethodViewModelBuilder,
            SecurityToken securityToken,
            PersonService personService,
            ISignInUrlResolver signInUrlResolver,
            AddressTypeService addressTypeService,
            CountryService countryService,
            PersonStorage personStorage,
            CheckoutState checkoutState)
        {
            _requestModelAccessor = requestModelAccessor;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _securityContextService = securityContextService;
            _deliveryMethodViewModelBuilder = deliveryMethodViewModelBuilder;
            _paymentMethodViewModelBuilder = paymentMethodViewModelBuilder;
            _securityToken = securityToken;
            _personService = personService;
            _addressTypeService = addressTypeService;
            _countryService = countryService;
            _personStorage = personStorage;
            _checkoutState = checkoutState;
            _signInUrlResolver = signInUrlResolver;
        }

        public virtual CheckoutViewModel Build()
        {
            var requestModel = _requestModelAccessor.RequestModel;
            var orderCarrier = requestModel.Cart.OrderCarrier;
            var deliveryMethods = _deliveryMethodViewModelBuilder.Build();
            var paymentMethods = _paymentMethodViewModelBuilder.Build();
            var checkoutPage = requestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
            var checkoutPageEntity = checkoutPage.EntitySystemId.MapTo<PageModel>();
            _signInUrlResolver.TryGet(_routeRequestLookupInfoAccessor.RouteRequestLookupInfo, out var loginPageUrl);
            var termsAndConditionPageUrl = checkoutPageEntity.GetValue<PointerPageItem>(CheckoutPageFieldNameConstants.TermsAndConditionsPage)?.MapTo<LinkModel>()?.Href;
            var checkoutModeInt = requestModel.WebsiteModel.GetValue<int>(AcceleratorWebsiteFieldNameConstants.CheckoutMode);
            var checkoutMode = checkoutModeInt == 0 ? CheckoutMode.Both : (CheckoutMode)checkoutModeInt;
            var model = new CheckoutViewModel()
            {
                Authenticated = _securityContextService.GetIdentityUserSystemId().HasValue,
                CheckoutMode = (int)checkoutMode,
                DeliveryMethods = deliveryMethods,
                PaymentMethods = paymentMethods,
                IsBusinessCustomer = _personStorage.CurrentSelectedOrganization != null,
                CompanyName = _personStorage.CurrentSelectedOrganization?.Name,
                SelectedDeliveryMethod = orderCarrier?.Deliveries?.FirstOrDefault()?.DeliveryMethodID ?? deliveryMethods?.FirstOrDefault()?.Id,
                SelectedPaymentMethod = (orderCarrier?.PaymentInfo?.FirstOrDefault() != null)
                                        ? string.Concat(orderCarrier?.PaymentInfo?.FirstOrDefault().PaymentProvider, ":", orderCarrier?.PaymentInfo?.FirstOrDefault().PaymentMethod)
                                        : paymentMethods?.FirstOrDefault()?.Id,
                CheckoutUrl = checkoutPage?.MapTo<LinkModel>()?.Href,
                TermsUrl = termsAndConditionPageUrl,
                LoginUrl = loginPageUrl,
                SignUp = _checkoutState.NeedToRegister,
            };

            model = GetCustomerDetails(model);

            //Connected country to the channel must be selected
            if (model.IsBusinessCustomer)
            {
                var countries = _countryService.GetAll().Where(x => _requestModelAccessor.RequestModel.ChannelModel.Channel.CountryLinks.Any(y => y.CountrySystemId == x.SystemId));
                var companyAddresses = _personStorage.CurrentSelectedOrganization?.Addresses?.Where(x => countries.Any(y => y.Id == x.Country))
                    .Select(address => address.MapTo<AddressViewModel>())?.Where(address => address != null)?.ToList();
                model.CompanyAddresses = companyAddresses;
                model.SelectedCompanyAddressId = companyAddresses?.FirstOrDefault(x=>x.Country == requestModel.CountryModel.Country.Id)?.SystemId;
            }
            else
            {
                model.CustomerDetails.Country = requestModel.CountryModel.Country.Id;
            }
            
            return model;
        }

        private CheckoutViewModel GetCustomerDetails(CheckoutViewModel model)
        {
            model.CustomerDetails = new CustomerDetailsViewModel();
            if (_checkoutState.Address != null)
            {
                CopyAddressValues(_checkoutState.Address, model.CustomerDetails);
                if (_checkoutState.DeliveryAddress != null)
                {
                    model.AlternativeAddress = new CustomerDetailsViewModel();
                    CopyAddressValues(_checkoutState.DeliveryAddress, model.AlternativeAddress);
                }

                return model;
            }

            if (_securityToken.IsAnonymousUser)
            {
                return model;
            }

            var person = _personService.Get(_securityToken.UserID);
            if (person == null)
            {
                return model;
            }

            if (model.IsBusinessCustomer)
            {
                //Use person phone for company customer
                model.CustomerDetails.PhoneNumber = person.Phone;
            }
            else
            {
                var addressType = _addressTypeService.Get(AddressTypeNameConstants.Address);
                var address = person.Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId);
                if (address != null)
                {
                    model.CustomerDetails.MapFrom(address);
                }

                addressType = _addressTypeService.Get(AddressTypeNameConstants.AlternativeAddress);
                address = person.Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId);
                if (address != null)
                {
                    model.AlternativeAddress = new CustomerDetailsViewModel();
                    model.AlternativeAddress.MapFrom(address);
                }
            }

            model.CustomerDetails.FirstName = person.FirstName;
            model.CustomerDetails.LastName = person.LastName;
            model.CustomerDetails.Email = person.Email;

            return model;
        }

        private void CopyAddressValues(AddressCarrier source, CustomerDetailsViewModel target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));
            target.Email = source.Email ?? target.Email;
            target.PhoneNumber = source.MobilePhone ?? target.PhoneNumber;
            target.FirstName = source.FirstName ?? target.FirstName;
            target.LastName = source.LastName ?? target.LastName;
            target.Email = source.Email ?? target.Email;
            target.Address = source.Address1;
            target.CareOf = source.CareOf;
            target.City = source.City;
            target.Country = source.Country;
            target.ZipCode = source.Zip;
        }
    }
}
