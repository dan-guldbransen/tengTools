using Litium.Accelerator.ViewModels.ContactUs;
using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(ContactUsService))]
    public class ContactUsService
    {
        public bool ValidateEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void BeakOutMessage(ContactUsViewModel model)
        {
            var output = new MessageInfo() { SenderName = model.ContacterName, SenderEmail = model.ContacterEmail, SenderMessage = model.ContacterMessage, SenderPhone = model.ContacterPhone ?? "" };

        }
    }
}
public class MessageInfo
{
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPhone { get; set; }
    public string SenderMessage { get; set; }
    public string ReceiverEmail { get; set; }
}
