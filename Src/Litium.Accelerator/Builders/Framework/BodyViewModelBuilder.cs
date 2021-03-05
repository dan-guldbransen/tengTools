using System;
using System.Linq;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Models.Products;
using Litium.Web.Models.Websites;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace Litium.Accelerator.Builders.Framework
{
    [Service(ServiceType = typeof(BodyViewModelBuilder), Lifetime = DependencyLifetime.Scoped)]
    public class BodyViewModelBuilder
    {
        private static readonly object _lock = new object();
        private readonly TrackingScriptService _trackingScriptService;
        private readonly IMemoryCache _memoryCache;
        private readonly IFileProvider _fileProvider;

        public BodyViewModelBuilder(TrackingScriptService trackingScriptService, IMemoryCache memoryCache, IFileProvider fileProvider)
        {
            _trackingScriptService = trackingScriptService;
            _memoryCache = memoryCache;
            _fileProvider = fileProvider;
        }

        public BodyEndViewModel BuildBodyEnd(WebsiteModel websiteModel, PageModel pageModel, CategoryModel categoryModel, ProductModel productModel)
        {
            string fileName = _memoryCache.Get("LitiumAppJs") as string;
            if (fileName == null)
            {
                lock (_lock)
                {
                    fileName = _memoryCache.Get("LitiumAppJs") as string;
                    if (fileName == null)
                    {
                        var file = _fileProvider.GetDirectoryContents("ui")
                            .OfType<IFileInfo>()
                            .FirstOrDefault(x =>
                            !x.IsDirectory
                            && x.Name.StartsWith("app.", StringComparison.OrdinalIgnoreCase)
                            && x.Name.EndsWith(".js", StringComparison.OrdinalIgnoreCase));

                        fileName = "/ui/" + file.Name;

                        var changeToken = _fileProvider.Watch(fileName);
                        _memoryCache.Set("LitiumAppJs", fileName, changeToken);
                    }
                }
            }

            var viewModel = new BodyEndViewModel
            {
                FileName = fileName,
                TrackingScripts = _trackingScriptService.GetBodyEndScript(pageModel.Page)
            };
            return viewModel;
        }

        public BodyStartViewModel BuildBodyStart(WebsiteModel websiteModel, PageModel pageModel, CategoryModel categoryModel, ProductModel productModel)
        {
            var viewModel = new BodyStartViewModel
            {
                TrackingScripts = _trackingScriptService.GetBodyStartScripts(pageModel.Page)
            };
            return viewModel;
        }
    }
}