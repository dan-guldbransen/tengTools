using Litium.Accelerator.Mvc.ModelStates;
using Litium.Accelerator.Services;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.Login;
using Litium.Customers;
using Litium.Foundation.Security;
using Litium.Studio.Extenssions;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Login;
using ModelState = Litium.Accelerator.Services.ModelState;
using System;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Globalization;
using System.Net;

namespace Litium.Accelerator.Mvc.Controllers.Login
{
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;
        private readonly MyPagesViewModelService _myPagesViewModelService;
        private readonly SecurityToken _securityToken;
        private readonly LoginViewModelBuilder _loginViewModelBuilder;
        private readonly ForgotPasswordViewModelBuilder _forgotPasswordViewModelBuilder;
        private readonly OrganizationService _organizationService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly AddressTypeService _addressTypeService;
        private readonly PersonStorage _personStorage;
        private readonly CountryService _countryService;
        private readonly ModelState _modelState;
        public LoginController(
            LoginService loginService,
            SecurityToken securityToken,
            LoginViewModelBuilder loginViewModelBuilder,
            ForgotPasswordViewModelBuilder forgotPasswordViewModelBuilder,
            OrganizationService organizationService,
            MyPagesViewModelService myPagesViewModelService,
            RequestModelAccessor requestModelAccessor,
            AddressTypeService addressTypeService,
            CountryService countryService,
            PersonStorage personStorage)
        {
            _loginService = loginService;
            _securityToken = securityToken;
            _loginViewModelBuilder = loginViewModelBuilder;
            _forgotPasswordViewModelBuilder = forgotPasswordViewModelBuilder;
            _organizationService = organizationService;
            _myPagesViewModelService = myPagesViewModelService;
            _modelState = new MvcModelState(ModelState);
            _requestModelAccessor = requestModelAccessor;
            _addressTypeService = addressTypeService;
            _personStorage = personStorage;
            _countryService = countryService;
        }

        [HttpGet]
        public virtual ActionResult Login(string redirectUrl, string code)
        {
            var model = _loginViewModelBuilder.Build(redirectUrl ?? string.Empty);
            if (code != null && int.TryParse(code, out int resultCode) && Enum.IsDefined(typeof(HttpStatusCode), resultCode))
            {
                model.ErrorMessage = $"error.{resultCode}".AsWebSiteString();
                if (User.Identity.IsAuthenticated && resultCode == (int)HttpStatusCode.Unauthorized)
                {
                    model.InsufficientPermissions = true;
                }
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Login(LoginViewModel loginViewModel)
        {
            var model = _loginViewModelBuilder.Build(loginViewModel);
            model.RedirectUrl = loginViewModel.RedirectUrl;

            if (!_loginService.IsValidLoginForm(_modelState, model.LoginForm))
            {
                return View(model);
            }

            try
            {
                var loginSuccessfull = _loginService.Login(model.LoginForm.UserName, model.LoginForm.Password, out var token);
                if (loginSuccessfull)
                {
                    var person = _loginService.GetUser(model.LoginForm.UserName);

                    if (!_loginService.IsBusinessCustomer(person, out var organizations))
                    {
                        var addressType = _addressTypeService.Get(AddressTypeNameConstants.Address);
                        var address = person.Addresses.FirstOrDefault(x => x.AddressTypeSystemId == addressType.SystemId);
                        //Check if user has the same country in the address as channel has.
                        if (address != null && !string.IsNullOrEmpty(address.Country) && !address.Country.Equals( _requestModelAccessor.RequestModel.CountryModel.Country.Id, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var country = _countryService.Get(address.Country);
                            //Check if country is connected to the channel
                            if (country != null && _requestModelAccessor.RequestModel.ChannelModel.Channel.CountryLinks.Any(x=>x.CountrySystemId==country.SystemId))
                            {
                                // Set user's country to the channel
                                _requestModelAccessor.RequestModel.Cart.SetChannel(_requestModelAccessor.RequestModel.ChannelModel.Channel,country, SecurityToken.CurrentSecurityToken);
                            }
                        }
                        return new RedirectResult(model.RedirectUrl);
                    }
                    if (organizations.Count <= 1)
                    {
                        return new RedirectResult(model.RedirectUrl);
                    }
                    model.Organizations = _loginViewModelBuilder.GetOrganizations(organizations);
                    return View(nameof(SelectOrganization), model);
                }
                model.ErrorMessage = "login.failed".AsWebSiteString();
            }
            catch (ChangePasswordException)
            {
                return View(nameof(ChangePassword), model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LoginViewModel loginViewModel)
        {
            if (string.IsNullOrWhiteSpace(loginViewModel.LoginForm.UserName) || string.IsNullOrWhiteSpace(loginViewModel.LoginForm.Password))
            {
                return RedirectToAction(nameof(Login));
            }

            var model = _loginViewModelBuilder.Build(loginViewModel);
            model.RedirectUrl = loginViewModel.RedirectUrl;

            if (!_myPagesViewModelService.IsValidPasswordForm(_modelState, model.ChangePasswordForm,
                model.LoginForm.Password))
            {
                return View(model);
            }
            
            var loginSuccessfull = _loginService.Login(model.LoginForm.UserName, model.LoginForm.Password, model.ChangePasswordForm.Password, out var token);
            if (!loginSuccessfull)
            {
                return View(model);
            }
            var person = _loginService.GetUser(model.LoginForm.UserName);
            if (!_loginService.IsBusinessCustomer(person, out var organizations))
            {
                return new RedirectResult(model.RedirectUrl);
            }
                    
            if (organizations.Count <= 1)
            {
                return new RedirectResult(model.RedirectUrl);
            }
            model.Organizations = _loginViewModelBuilder.GetOrganizations(organizations);
            return View(nameof(SelectOrganization), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectOrganization(LoginViewModel loginViewModel)
        {
            _personStorage.CurrentSelectedOrganization = _organizationService.Get(loginViewModel.SelectedOrganization);
            if (_personStorage.CurrentSelectedOrganization != null)
            {
                return new RedirectResult(loginViewModel.RedirectUrl);
            }

            _loginService.Logout();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public virtual ActionResult ForgotPassword()
        {
            var model = _forgotPasswordViewModelBuilder.Build();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var model = _forgotPasswordViewModelBuilder.Build(forgotPasswordViewModel.ForgotPasswordForm);

            if (_loginService.IsValidForgotPasswordForm(_modelState, model.ForgotPasswordForm))
            {
                var user = _loginService.GetUser(model.ForgotPasswordForm.Email?.ToLower());
                if (user == null)
                {
                    model.ErrorMessage = "login.usernotfound".AsWebSiteString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        if (_loginService.ChangePassword(user, false, true, model.ChannelSystemId, _securityToken))
                        {
                            model.Message = "login.passwordsent".AsWebSiteString();
                        }
                        else
                        {
                            model.ErrorMessage = "login.passwordcouldnotbesent".AsWebSiteString();
                        }
                    }
                    else
                    {
                        model.ErrorMessage = "login.passwordcouldnotbesent".AsWebSiteString();
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public RedirectResult Logout(string redirectUrl)
        {
            _loginService.Logout();

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = "~/";
            }
            return new RedirectResult(redirectUrl);
        }
    }
}