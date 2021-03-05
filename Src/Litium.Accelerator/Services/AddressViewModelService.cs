using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.Persons;
using Litium.Customers;
using Litium.Foundation;
using Litium.Foundation.Log;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Studio.Extenssions;

namespace Litium.Accelerator.Services
{
    public class AddressViewModelService : ViewModelService<AddressViewModel>
    {
        private readonly SecurityContextService _securityContextService;
        private readonly OrganizationService _organizationService;
        private readonly UserValidationService _userValidationService;
        private readonly AddressTypeService _addressTypeService;
        private readonly PersonStorage _personStorage;

        private Organization CurrentOrganization
        {
            get
            {
                var currentOrganization = _personStorage.CurrentSelectedOrganization;
                if (currentOrganization == null)
                {
                    throw new ArgumentNullException(nameof(CurrentOrganization));
                }

                return currentOrganization;
            }
        }

        public AddressViewModelService(
            SecurityContextService securityContextService,
            OrganizationService organizationService,
            UserValidationService userValidationService,
            AddressTypeService addressTypeService,
            PersonStorage personStorage)
        {
            _securityContextService = securityContextService;
            _organizationService = organizationService;
            _userValidationService = userValidationService;
            _addressTypeService = addressTypeService;
            _personStorage = personStorage;
        }

        public IEnumerable<Address> GetAddresses()
        {
            return CurrentOrganization.Addresses;
        }

        public Address GetAddress(Guid systemId)
        {
            return GetAddresses().FirstOrDefault(address => address.SystemId == systemId);
        }

        public bool Create(AddressViewModel viewModel, ModelState modelState)
        {
            SetDefaultAddressType(viewModel);

            try
            {
                var existingAddress = GetExistingAddress(viewModel);
                //don't save to avoid of having duplicate addresses.
                if (existingAddress == null)
                {
                    var organization = CurrentOrganization.MakeWritableClone();
                    var address = viewModel.MapTo<Address>();
                    organization.Addresses.Add(address);
                    using (_securityContextService.ActAsSystem())
                    {
                        _organizationService.Update(organization);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Solution.Instance.Log.CreateLogEntry(ex.Message, ex, LogLevels.ERROR);
                modelState.AddModelError("general", "mypage.address.unabletocreate".AsWebSiteString());
                return false;
            }
        }

        public bool Update(AddressViewModel viewModel, ModelState modelState)
        {
            SetDefaultAddressType(viewModel);

            try
            {
                var existingAddress = GetExistingAddress(viewModel);
                //don't save to avoid of having duplicate addresses.
                if (existingAddress == null)
                {
                    var organization = CurrentOrganization.MakeWritableClone();
                    var address = organization.Addresses.FirstOrDefault(x => x.SystemId == viewModel.SystemId);
                    if (address == null)
                    {
                        modelState.AddModelError("general", "mypage.address.notfound".AsWebSiteString());
                        return false;
                    }

                    address.MapFrom(viewModel);

                    using (_securityContextService.ActAsSystem())
                    {
                        _organizationService.Update(organization);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Solution.Instance.Log.CreateLogEntry(ex.Message, ex, LogLevels.ERROR);
                modelState.AddModelError("general", "mypage.address.unabletoupdate".AsWebSiteString());
                return false;
            }
        }

        public void Delete(Guid id)
        {
            var organization = CurrentOrganization.MakeWritableClone();
            var address = organization.Addresses.FirstOrDefault(item => item.SystemId == id);
            if (address != null)
            {
                organization.Addresses.Remove(address);
                using (_securityContextService.ActAsSystem())
                {
                    _organizationService.Update(organization);
                }
            }
        }

        public bool Validate(AddressViewModel viewModel, ModelState modelState)
        {
            var validationRules = new List<ValidationRuleItem<AddressViewModel>>
            {
                new ValidationRuleItem<AddressViewModel>{Field = nameof(AddressViewModel.Address), Rule = model => !string.IsNullOrEmpty(model.Address), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<AddressViewModel>{Field = nameof(AddressViewModel.ZipCode), Rule = model => !string.IsNullOrEmpty(model.ZipCode), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<AddressViewModel>{Field = nameof(AddressViewModel.City), Rule = model => !string.IsNullOrEmpty(model.City), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<AddressViewModel>{Field = nameof(AddressViewModel.Country), Rule = model => !string.IsNullOrEmpty(model.Country), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<AddressViewModel>{Field = nameof(AddressViewModel.PhoneNumber), Rule = model => !string.IsNullOrEmpty(model.PhoneNumber), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<AddressViewModel>{Field = nameof(AddressViewModel.PhoneNumber), Rule = model => _userValidationService.IsValidPhone(model.PhoneNumber), ErrorMessage = () => "validation.phone".AsWebSiteString()}
            };

            return viewModel.IsValid(validationRules, modelState);
        }

        private Address GetExistingAddress(AddressViewModel model)
        {
            var addressType = _addressTypeService.Get(model.AddressType);
            var address = _personStorage.CurrentSelectedOrganization.Addresses
                .FirstOrDefault(x => string.Equals(x.Address1, model.Address) &&
                                     string.Equals(x.Address2, model.Address2) &&
                                     string.Equals(x.ZipCode, model.ZipCode) &&
                                     string.Equals(x.City, model.City) &&
                                     string.Equals(x.Country, model.Country) &&
                                     string.Equals(x.PhoneNumber, model.PhoneNumber) &&
                                     x.AddressTypeSystemId == addressType.SystemId);
            return address;
        }

        private void SetDefaultAddressType(AddressViewModel model)
        {
            model.AddressType = AddressTypeNameConstants.Address;
        }
    }
}
