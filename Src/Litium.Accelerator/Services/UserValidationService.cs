using Litium.Runtime.DependencyInjection;
using Litium.Security;
using Litium.Studio.Plugins.EmailValidation;
using System.Text.RegularExpressions;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(UserValidationService))]
    public class UserValidationService
    {
        private readonly SecurityContextService _securityContextService;
        private readonly IEmailValidator _emailValidator;
        private readonly PasswordService _passwordService;

        public UserValidationService(SecurityContextService securityContextService, IEmailValidator emailValidator, PasswordService passwordService)
        {
            _securityContextService = securityContextService;
            _emailValidator = emailValidator;
            _passwordService = passwordService;
        }

        public bool IsValidUserName(string userName)
        {
            var validUsername = false;

            if (!string.IsNullOrEmpty(userName))
            {
                var user = _securityContextService.GetPersonSystemId(userName);
                validUsername = user == null || user.Equals(_securityContextService.GetIdentityUserSystemId().GetValueOrDefault());
            }

            return validUsername;
        }

        public bool IsValidEmail(string email)
        {
            var validEmail = false;

            if (!string.IsNullOrEmpty(email))
            {
                validEmail = _emailValidator.IsValidEmail(email);
            }

            return validEmail;
        }

        public bool IsValidPhone(string phone)
        {
            var validPhone = false;

            if (!string.IsNullOrEmpty(phone))
            {
                var rgx = new Regex(@"^(\+?)\(?\d{2,3}\)? *-? *\d{2,4} *-? *\d{2,4} *-? *\d{2,4}$");
                validPhone = rgx.IsMatch(phone);
            }

            return validPhone;
        }

        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && PasswordStartsAndEndsWithLegalCharacters(password);
        }

        public bool IsPasswordMatch(string password, string confirmPassword)
        {
            return password.Equals(confirmPassword);
        }

        public bool IsValidPasswordComplexity(string password)
        {
            var validPassword = false;
            
            if (!string.IsNullOrEmpty(password))
            {
                validPassword = _passwordService.ValidateComplexity(password);
            }

            return validPassword;
        }

        private bool PasswordStartsAndEndsWithLegalCharacters(string password)
        {
            return password.Trim().Length.Equals(password.Length);
        }
    }
}
