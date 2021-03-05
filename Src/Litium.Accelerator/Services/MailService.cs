using Litium.Accelerator.Mailing;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(MailService), Lifetime = DependencyLifetime.Singleton)]
    public abstract class MailService
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="mailSender">The mail sender</param>
        /// <param name="toEmail">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="htmlFormat">if set to <c>true</c> [HTML format].</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        public abstract void SendEmail(string mailSender, string toEmail, string subject, string body, bool htmlFormat, bool throwException);

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        public abstract void SendEmail(MailDefinition mail, bool throwException);
    }
}
