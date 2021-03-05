using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Litium.Accelerator.Constants;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Studio.Extenssions;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Routing;
using Litium.Websites;

namespace Litium.Accelerator.Builders.Framework
{
    /// <summary>
    /// This Model builder is creating the open graph.
    /// See http://ogp.me/ for more info about Open Graph.
    /// </summary>
    public class OpenGraphViewModelBuilder : IViewModelBuilder<OpenGraphViewModel>
    {
        private readonly MetaService _metaService;
        private string _baseUrl;
        private readonly UrlService _urlService;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;


        public OpenGraphViewModelBuilder(MetaService metaService, UrlService urlService, RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor)
        {
            _metaService = metaService;
            _urlService = urlService;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
        }

        private string PageImageProperty { get; } = "Image1";

        public OpenGraphViewModel Build(Website website, Page page, Category category, BaseProduct baseProduct, Variant variant)
        {
            var model = new OpenGraphViewModel
            {
                Locale = GetLanguageCode(),
                MetaDescription = _metaService.GetDescription(page, category, baseProduct, variant)
            };

            if (_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsSecureConnection)
            {
                model.ImageSuffix = ":secure_url";
            }
            SetPageData(model, page, category, baseProduct, variant);
            SetLogotypeImage(model, website, page);

            return model;
        }

        private string GetBaseUrl()
        {
            if (_baseUrl != null)
            {
                return _baseUrl;
            }
            
            var hostName = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.DomainName.Id;

            if (hostName.Contains("/"))
            {
                hostName = hostName.Substring(0, hostName.IndexOf("/", StringComparison.Ordinal));
            }

            int? port = null;
            if (hostName.Contains(":"))
            {
                var s = hostName.Split(':');
                hostName = s.First();
                port = int.Parse(s.Last());
            }

            var uriBuilder = new UriBuilder(_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsSecureConnection ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, hostName, port ?? (_routeRequestLookupInfoAccessor.RouteRequestLookupInfo.IsSecureConnection ? 443 : 80));

            _baseUrl = new Uri(uriBuilder.Uri.ToString()).AbsoluteUri.TrimEnd('/');
            return _baseUrl;
        }

        private string GetImageUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                return url.Contains("//") ? url : GetBaseUrl() + VirtualPathUtility.ToAbsolute(url);
            }

            return string.Empty;
        }

        private string GetLanguageCode()
        {
            return CultureInfo.CurrentUICulture.IetfLanguageTag.Replace("-", "_");
        }

        private string GetProductCatalogImageUrl(IList<Guid> displayImage, Page page)
        {
            if (displayImage == null || !displayImage.Any())
            {
                return null;
            }

            return GetImageUrl(FileExtensions.GetUrl(displayImage.First()));
        }

        private void SetLogotypeImage(OpenGraphViewModel model, Website website, Page page)
        {
            var imageUrl = website.Fields.GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.LogotypeMain)?.MapTo<ImageModel>().GetUrlToImage(System.Drawing.Size.Empty, System.Drawing.Size.Empty).Url;
            model.LogotypeImageUrl = HttpUtility.HtmlEncode(GetImageUrl(imageUrl));
            model.LogotypeVisible = !string.IsNullOrEmpty(model.LogotypeImageUrl);
        }

        private void SetPageData(OpenGraphViewModel model, Page page, Category category, BaseProduct baseProduct, Variant variant)
        {
            if (baseProduct != null)
            {
                // Product page
                model.Url = _urlService.GetUrl(baseProduct, options: options => options.AbsoluteUrl = true);
                model.ImageUrl = HttpUtility.HtmlEncode(GetProductCatalogImageUrl(variant?.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images) ?? baseProduct.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images), page));
            }
            else if (category != null)
            {
                // category page
                model.Url = _urlService.GetUrl(category, options: options => options.AbsoluteUrl = true);
                model.ImageUrl = HttpUtility.HtmlEncode(GetProductCatalogImageUrl(category.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images), page));
            }
            else
            {
                model.Url = _urlService.GetUrl(page, options: options => options.AbsoluteUrl = true);

                if (PageImageProperty != null)
                {
                    var imageUrl = page.Fields.GetValue<Guid?>(PageImageProperty)?.MapTo<ImageModel>().GetUrlToImage(new System.Drawing.Size(-1, -1), new System.Drawing.Size(-1, -1)).Url;

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        model.ImageUrl = imageUrl.Contains("//") ? HttpUtility.HtmlEncode(imageUrl) : HttpUtility.HtmlEncode(GetBaseUrl() + imageUrl);
                    }
                    else
                    {
                        model.ImageVisible = false;
                    }
                }
            }

            model.Title = _metaService.GetTitle(page, category, baseProduct, variant);
            model.WebSiteTitle = "site.title".AsWebSiteString();
            model.ImageVisible = !string.IsNullOrEmpty(model.ImageUrl);
        }
    }
}
