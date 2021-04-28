using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using System;
using System.Globalization;

namespace Litium.Accelerator.Builders.Framework
{
    public class CookieNotificationViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : CookieNotificationViewModel
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public CookieNotificationViewModelBuilder(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public CookieNotificationViewModel Buid()
        {
            var model = new CookieNotificationViewModel();
            var website = _requestModelAccessor.RequestModel.WebsiteModel;

            model.Text = website.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.CookieNotificationText, CultureInfo.CurrentUICulture);
            model.Title = website.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.CookieNotificationHeader, CultureInfo.CurrentUICulture);
            model.PolicyPage = website.Fields.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CookieNotificationPolicyPage).MapTo<LinkModel>();
            model.Logo = website.GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.LogotypeMain)?.MapTo<ImageModel>();

            return model;
        }
    }
}
