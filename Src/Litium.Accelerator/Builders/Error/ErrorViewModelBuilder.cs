using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Error;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Websites;
using Litium.Websites;
using System;
using System.Linq;

namespace Litium.Accelerator.Builders.Error
{
    public class ErrorViewModelBuilder : IViewModelBuilder<ErrorViewModel>
    {
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageService _pageService;

        public ErrorViewModelBuilder(UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            PageService pageService)
        {
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _pageService = pageService;
        }

        /// <summary>
        /// Build the error model
        /// </summary>
        /// <param name="pageModel">The current error page</param>
        /// <returns>Return the error page model</returns>
        public virtual ErrorViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<ErrorViewModel>();

            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            var startPage = _pageService.GetChildPages(Guid.Empty, website.SystemId).FirstOrDefault();
            var startPageUrl = _urlService.GetUrl(startPage, new PageUrlArgs(_requestModelAccessor.RequestModel.ChannelModel.SystemId));

            model.Image = pageModel.GetValue<Guid?>(PageFieldNameConstants.Image)?.MapTo<ImageModel>();
            model.StartPageUrl = string.IsNullOrWhiteSpace(startPageUrl) ? "/" : startPageUrl;
            
            return model;
        }
    }
}
