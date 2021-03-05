using System;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.MyPages;
using Litium.Customers;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Studio.Extenssions;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Foundation.Security;
using Litium.Globalization;

namespace Litium.Accelerator.Services
{
    public class MyPagesViewModelService : ViewModelService<MyPagesViewModel>
    {
        private readonly SecurityContextService _securityContextService;
        private readonly PersonService _personService;
        private readonly AuthenticationService _authenticationService;
        private readonly UserValidationService _userValidationService;
        private readonly AddressTypeService _addressTypeService;
        private readonly CheckoutState _checkoutState;
        private readonly CountryService _countryService;
        private readonly RequestModelAccessor _requestModelAccessor;

        public MyPagesViewModelService(
            SecurityContextService securityContextService,
            PersonService personService,
            AuthenticationService authenticationService,
            UserValidationService userValidationService,
            AddressTypeService addressTypeService, 
            CountryService countryService, 
            RequestModelAccessor requestModelAccessor,
            CheckoutState checkoutState)
        {
            _securityContextService = securityContextService;
            _personService = personService;
            _authenticationService = authenticationService;
            _userValidationService = userValidationService;
            _addressTypeService = addressTypeService;
            _checkoutState = checkoutState;
            _countryService = countryService;
            _requestModelAccessor = requestModelAccessor;
        }

        public virtual void SaveMyDetails(IViewModel viewModel)
        {
            var person = _personService.Get(_securityContextService.GetIdentityUserSystemId().GetValueOrDefault())?.MakeWritableClone();
            if (person == null) return;

            person.MapFrom(viewModel);

            using (_securityContextService.ActAsSystem())
            {
                _personService.Update(person);
            }
            var addressType = _addressTypeService.Get(AddressTypeNameConstants.Address);
            var address = person.Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId);
            //Check if user has the same country in the address as channel has.
            if (address != null && !address.Country.Equals(_requestModelAccessor.RequestModel.CountryModel.Country.Id, StringComparison.CurrentCultureIgnoreCase))
            {
                var country = _countryService.Get(address.Country);
                // Set user's country to the channel
                _requestModelAccessor.RequestModel.Cart.SetChannel(_requestModelAccessor.RequestModel.ChannelModel.Channel, country, SecurityToken.CurrentSecurityToken);
            }
            _checkoutState.ClearState();
        }

        public void SaveUserName(string userName)
        {
            var person = _personService.Get(_securityContextService.GetIdentityUserSystemId().GetValueOrDefault())?.MakeWritableClone();
            if (person == null) return;

            person.LoginCredential.Username = userName;

            using (_securityContextService.ActAsSystem())
            {
                _personService.Update(person);
            }

            _authenticationService.RefreshSignIn();
            _checkoutState.ClearState();
        }

        public void SavePassword(string password)
        {
            var person = _personService.Get(_securityContextService.GetIdentityUserSystemId().GetValueOrDefault()).MakeWritableClone();
            if (person == null) return;

            person.LoginCredential.NewPassword = password;

            using (_securityContextService.ActAsSystem())
            {
                _personService.Update(person);
            }
        }

        public bool IsValidMyDetailsForm(ModelState modelState, MyDetailsViewModel myDetailsForm)
        {
            var firstNameField = nameof(myDetailsForm.FirstName);
            var lastNameField = nameof(myDetailsForm.LastName);

            var validationRules = new List<ValidationRuleItem<MyDetailsViewModel>>()
            {
                new ValidationRuleItem<MyDetailsViewModel>{Field = firstNameField, Rule = model => !string.IsNullOrEmpty(model.FirstName), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<MyDetailsViewModel>{Field = lastNameField, Rule = model => !string.IsNullOrEmpty(model.LastName), ErrorMessage = () => "validation.required".AsWebSiteString()},
            };

            return myDetailsForm.IsValid(validationRules, modelState);
        }

        public bool IsValidBusinessCustomerDetailsForm(ModelState modelState, BusinessCustomerDetailsViewModel businessCustomerDetailsForm)
        {
            var firstNameField = nameof(businessCustomerDetailsForm.FirstName);
            var lastNameField = nameof(businessCustomerDetailsForm.LastName);
            var phoneField = nameof(businessCustomerDetailsForm.Phone);

            var validationRules = new List<ValidationRuleItem<BusinessCustomerDetailsViewModel>>()
            {
                new ValidationRuleItem<BusinessCustomerDetailsViewModel>{Field = firstNameField, Rule = model => !string.IsNullOrEmpty(model.FirstName), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<BusinessCustomerDetailsViewModel>{Field = lastNameField, Rule = model => !string.IsNullOrEmpty(model.LastName), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<BusinessCustomerDetailsViewModel>{Field = phoneField, Rule = model => !string.IsNullOrEmpty(model.Phone), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<BusinessCustomerDetailsViewModel>{Field = phoneField, Rule = model => _userValidationService.IsValidPhone(model.Phone), ErrorMessage = () => "validation.phone".AsWebSiteString()},
            };

            return businessCustomerDetailsForm.IsValid(validationRules, modelState);
        }

        public bool IsValidUserNameForm(ModelState modelState, ChangeUserNameFormViewModel userNameForm)
        {
            var prefix = nameof(userNameForm);
            var userNameField = $"{prefix}.{nameof(userNameForm.UserName)}";

            var validationRules = new List<ValidationRuleItem<ChangeUserNameFormViewModel>>()
            {
                new ValidationRuleItem<ChangeUserNameFormViewModel>{Field = userNameField, Rule = model => !string.IsNullOrEmpty(model.UserName), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<ChangeUserNameFormViewModel>{Field = userNameField, Rule = model => _userValidationService.IsValidEmail(model.UserName), ErrorMessage = () => "validation.email".AsWebSiteString()},
                new ValidationRuleItem<ChangeUserNameFormViewModel>{Field = userNameField, Rule = model => _userValidationService.IsValidUserName(model.UserName), ErrorMessage = () => "validation.invalidusername".AsWebSiteString()}
            };

            return userNameForm.IsValid(validationRules, modelState);
        }

        public bool IsValidPasswordForm(ModelState modelState, ChangePasswordFormViewModel passwordForm, string oldPassword = null)
        {
            var prefix = nameof(passwordForm);
            var passwordField = $"{prefix}.{nameof(passwordForm.Password)}";

            var validationRules = new List<ValidationRuleItem<ChangePasswordFormViewModel>>()
            {
                new ValidationRuleItem<ChangePasswordFormViewModel>{Field = passwordField, Rule = model => !string.IsNullOrEmpty(model.Password), ErrorMessage = () => "validation.required".AsWebSiteString()},
                new ValidationRuleItem<ChangePasswordFormViewModel>{Field = passwordField, Rule = model => oldPassword == null || !model.Password.Equals(oldPassword), ErrorMessage = () => "changepassword.newpasswordequalstheoldpassword".AsWebSiteString()},
                new ValidationRuleItem<ChangePasswordFormViewModel>{Field = passwordField, Rule = model => _userValidationService.IsValidPassword(model.Password), ErrorMessage = () => "changepassword.invalidpasswordformat".AsWebSiteString()},
                new ValidationRuleItem<ChangePasswordFormViewModel>{Field = passwordField, Rule = model => _userValidationService.IsPasswordMatch(model.Password, model.ConfirmPassword), ErrorMessage = () => "changepassword.passwordconfirmationdoesnotmatch".AsWebSiteString()},
                new ValidationRuleItem<ChangePasswordFormViewModel>{Field = passwordField, Rule = model => _userValidationService.IsValidPasswordComplexity(model.Password), ErrorMessage = () => "changepassword.weakpassword".AsWebSiteString()},
            };

            return passwordForm.IsValid(validationRules, modelState);
        }
    }
}
