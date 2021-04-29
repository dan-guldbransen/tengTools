using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;

namespace Litium.Accelerator.Builders.Framework
{
    public class FooterViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : FooterViewModel
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public FooterViewModelBuilder(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public FooterViewModel Build()
        {
            var model = new FooterViewModel();
            var website = _requestModelAccessor.RequestModel.WebsiteModel;

            var footer = website.GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Footer);
            model.TopText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterTopText) ?? string.Empty;
            model.GetOrganised = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.FooterGetOrganised)?.MapTo<LinkModel>();
            model.Newsletter = website.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.FooterNewsletter)?.MapTo<LinkModel>();
            model.SocialMediaText = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterSocialMediaText) ?? string.Empty;
            model.VisionHeader = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionHeader) ?? string.Empty;
            model.VisionTextLeft = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionTextLeft) ?? string.Empty;
            model.VisionTextRight = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterVisionTextRight) ?? string.Empty;
            model.Legal = website.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterLegal) ?? string.Empty;

            if (footer != null)
            {
                model.SectionList = footer.Select(c => c.MapTo<SectionModel>()).ToList();                
            }

            return model;
        }
    }
}