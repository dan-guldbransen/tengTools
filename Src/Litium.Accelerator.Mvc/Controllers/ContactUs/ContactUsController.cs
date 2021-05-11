using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Litium.Accelerator.Builders.ContactUs;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.ContactUs;
using Litium.Globalization;
using Litium.Web.Models.Websites;
using Litium.Websites;

namespace Litium.Accelerator.Mvc.Controllers.ContactUs
{
    public class ContactUsController : ControllerBase
    {
        private readonly ContactUsViewModelBuilder _contactUsViewModelBuilder;
        private readonly MailService _mailService;
        private readonly RequestModelAccessor _requestModelAccessor;

        public ContactUsController(ContactUsViewModelBuilder contactUsViewModelBuilder, MailService mailService, RequestModelAccessor requestModelAccessor)
        {
            _contactUsViewModelBuilder = contactUsViewModelBuilder;
            _mailService = mailService;
            _requestModelAccessor = requestModelAccessor;
        }
        

        [HttpGet]
        public ActionResult Index(PageModel currentPageModel)
        {
            var model = _contactUsViewModelBuilder.Build(currentPageModel);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ContactUsViewModel model)
        {
            var channel = _requestModelAccessor.RequestModel.ChannelModel;
            var website = _requestModelAccessor.RequestModel.WebsiteModel;
           
            if (ModelState.IsValid)
            {
                var subject = website.Texts.GetValue("contactus.subject") ?? "contactus.subject";
                var emailAddress = channel.GetValue<string>(ChannelFieldNameConstants.ContactEmail);

                _mailService.SendEmail(model.ContacterEmail, emailAddress, subject, model.ContacterMessage, false, false);
               model.ThankYouMessage = website.Texts.GetValue("contactus.thankyoumessage") ?? "contactus.thankyoumessage";
            }
            else
            {
                var customErrors = new Dictionary<string, string>();
                foreach(var key in ModelState.Keys)
                {
                    if(key == nameof(ContactUsViewModel.ContacterName))
                    {
                        customErrors.Add(nameof(ContactUsViewModel.ContacterName), website.Texts.GetValue("contactus.name.required") ?? "contactus.name.required");
                    }
                    if (key == nameof(ContactUsViewModel.ContacterEmail))
                    {
                        customErrors.Add(nameof(ContactUsViewModel.ContacterEmail), website.Texts.GetValue("contactus.email.required") ?? "contactus.email.required");
                    }
                    if (key == nameof(ContactUsViewModel.ContacterMessage))
                    {
                        customErrors.Add(nameof(ContactUsViewModel.ContacterMessage), website.Texts.GetValue("contactus.message.required") ?? "contactus.message.required");
                    }
                }

                if (customErrors.Any())
                {
                    foreach(var key in customErrors.Keys)
                    {
                        ModelState.Remove(key);
                        ModelState.AddModelError(key, customErrors[key]);
                    }
                }
            }

            return View("Index", model);
        }
    }
}