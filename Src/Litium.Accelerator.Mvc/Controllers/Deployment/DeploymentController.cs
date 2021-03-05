using Litium.Accelerator.Deployments;
using Litium.Accelerator.ViewModels.Deployment;
using Litium.Common;
using Litium.ComponentModel;
using Litium.Foundation.Modules.CMS;
using Litium.Foundation.Modules.ECommerce;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Studio.Extenssions;
using Litium.Websites;
using System;
using System.Linq;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Deployment;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Litium.Foundation;
using Litium.Web;

namespace Litium.Accelerator.Mvc.Controllers.Deployment
{
    public class DeploymentController : Controller
    {
        private readonly DeploymentViewModelBuilder _deploymentViewModelBuilder;
        private readonly FolderService _folderService;
        private readonly ChannelService _channelService;
        private readonly AssortmentService _assortmentService;
        private readonly WebsiteService _websiteService;
        private readonly StructureInfoService _structureInfoService;
        private readonly IPackageService _packageService;
        private readonly IPackage _package;
        private readonly SettingService _settingService;
        private readonly DomainNameService _domainNameService;
        private readonly SlugifyService _slugifyService;
        private readonly ILogger<DeploymentController> _logger;
        private readonly InventoryService _inventoryService;
        private readonly PriceListService _priceListService;
        private readonly CategoryService _categoryService;
        private readonly BaseProductService _baseProductService;
        private readonly VariantService _variantService;
        private readonly MarketService _marketService;

        public DeploymentController(
            DeploymentViewModelBuilder deploymentViewModelBuilder,
            FolderService folderService,
            ChannelService channelService,
            AssortmentService assortmentService,
            IPackageService packageService,
            IPackage package,
            WebsiteService websiteService,
            StructureInfoService structureInfoService,
            SettingService settingService,
            DomainNameService domainNameService,
            SlugifyService slugifyService,
            ILogger<DeploymentController> logger,
            InventoryService inventoryService,
            PriceListService priceListService,
            CategoryService categoryService,
            BaseProductService baseProductService,
            VariantService variantService,
            MarketService marketService)
        {
            _deploymentViewModelBuilder = deploymentViewModelBuilder;
            _folderService = folderService;
            _channelService = channelService;
            _assortmentService = assortmentService;
            _packageService = packageService;
            _package = package;
            _websiteService = websiteService;
            _structureInfoService = structureInfoService;
            _settingService = settingService;
            _domainNameService = domainNameService;
            _slugifyService = slugifyService;
            _logger = logger;
            _inventoryService = inventoryService;
            _priceListService = priceListService;
            _categoryService = categoryService;
            _baseProductService = baseProductService;
            _variantService = variantService;
            _marketService = marketService;
        }

        [Route("litium/deployment", Name = "DeploymentView")]
        public ActionResult Index()
        {
            var model = _deploymentViewModelBuilder.Build(ShowExport());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportPackage(DeploymentViewModel.ImportViewModel importForm)
        {
            var model = _deploymentViewModelBuilder.Build(ShowExport());
            StructureInfo structureInfo = null;
            PackageInfo packageInfo = null;
            if (!ValidateImportForm(importForm))
            {
                return View(nameof(Index), model);
            }
            _package.Type = importForm.PackageName;

            try
            {
                structureInfo = _package.GetStructureInfo();
                structureInfo.CreateExampleProducts = importForm.CreateExampleProducts;

                packageInfo = _package.CreatePackageInfo(structureInfo, importForm.Name, importForm.DomainName);
                model.ShowImport = true;
                model.CanImport = true;

                Solution.Instance.SearchService.IndexJobConsumer.Stop();

                _packageService.Import(structureInfo, packageInfo);
                _structureInfoService.UpdatePropertyReferences(structureInfo, packageInfo);
                model.ImportMessage = "accelerator.deployment.import.success".AsAngularResourceString();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Package installation error.");
                model.ImportMessage = "accelerator.deployment.error".AsAngularResourceString();
                try
                {
                    foreach (var variant in structureInfo.ProductCatalog.Variants)
                    {
                        var newVariant = _variantService.Get(structureInfo.Id(variant.SystemId));
                        if (newVariant != null)
                        {
                            _variantService.Delete(newVariant);
                        }
                    }

                    foreach (var product in structureInfo.ProductCatalog.BaseProducts)
                    {
                        var newProduct = _baseProductService.Get(structureInfo.Id(product.SystemId));
                        if (newProduct != null)
                        {
                            _baseProductService.Delete(newProduct);
                        }
                    }

                    foreach (var category in structureInfo.ProductCatalog.Categories)
                    {
                        var newCategory = _categoryService.Get(structureInfo.Id(category.SystemId));
                        if (newCategory != null)
                        {
                            _categoryService.Delete(newCategory);
                        }
                    }

                    _channelService.Delete(packageInfo.Channel);
                    _marketService.Delete(packageInfo.Market);
                    _websiteService.Delete(packageInfo.Website);
                    _assortmentService.Delete(packageInfo.Assortment);
                    _folderService.Delete(packageInfo.Folder);
                    _inventoryService.Delete(packageInfo.Inventory);
                    _priceListService.Delete(packageInfo.PriceList);
                }
                catch (Exception ee)
                {
                    _logger.LogError(ee, "Package installation cleanup error.");
                }
            }
            finally
            {
                Solution.Instance.SearchService.IndexJobConsumer.Start();
            }

            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportPackage(DeploymentViewModel.ExportViewModel exportForm)
        {
            var model = _deploymentViewModelBuilder.Build(ShowExport());

            if (!Guid.TryParse(exportForm.ChannelSystemId, out var channelSystemId) ||
                !Guid.TryParse(exportForm.FolderSystemId, out var folderSystemId)) 
            {
                return View(nameof(Index), model);
            }

            var channel = _channelService.Get(channelSystemId);
            _package.Type = channel.Localizations.CurrentCulture.Name.Replace(" ", "");
            var website = _websiteService.Get(channel.WebsiteSystemId.GetValueOrDefault());

            var assortment = GetAssortment(channel);

            if (assortment == null)
            {
                return View(nameof(Index), model);
            }

            var deliveryMethodCarriers = ModuleECommerce.Instance.DeliveryMethods.GetAll().Select(x => x.GetAsCarrier()).ToList();
            var paymentMethodCarriers = ModuleECommerce.Instance.PaymentMethods.GetAll().Select(paymentMethod => paymentMethod.GetAsCarrier()).ToList();

            var structureInfo = _packageService.Export(new PackageInfo
            {
                Assortment = assortment,
                Folder = _folderService.Get(folderSystemId),
                Channel = channel,
                Website = website,
                DeliveryMethods = deliveryMethodCarriers,
                PaymentMethods = paymentMethodCarriers,
            });
            _package.PersistStructureInfo(structureInfo);

            model.ExportMessage = "accelerator.deployment.export.success".AsAngularResourceString();

            return View(nameof(Index), model);
        }

        private Assortment GetAssortment(Channel channel)
        {
            var marketModel = channel.MarketSystemId?.MapTo<Market>();
            if (marketModel?.AssortmentSystemId == null)
            {
                return null;
            }
            var assortment = _assortmentService.Get(marketModel.AssortmentSystemId.GetValueOrDefault());
            return assortment;
        }

        private bool ShowExport()
        {
            return ModuleCMS.Instance.ModuleLicense.CustomerName.IndexOf("Litium Product-Team licenses", StringComparison.OrdinalIgnoreCase) != -1
                && Request.Url.Host.StartsWith("content-");
        }

        private bool ValidateImportForm(DeploymentViewModel.ImportViewModel importForm)
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(importForm.PackageName))
            {
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(importForm.Name))
            {
                ModelState.AddModelError($"ImportForm.{nameof(importForm.Name)}", "accelerator.deployment.validation.required".AsAngularResourceString());
                isValid = false;
            }
            else
            {
                var name = _slugifyService.Slugify(CultureInfo.CurrentCulture, importForm.Name);

                if (_channelService.Get(name) != null)
                {
                    ModelState.AddModelError($"ImportForm.{nameof(importForm.Name)}", "accelerator.deployment.validation.channelexists".AsAngularResourceString());
                    isValid = false;
                }
            }

            var tempDomainName = !string.IsNullOrEmpty(importForm.DomainName) && importForm.DomainName.Contains("://") ? importForm.DomainName : "http://" + importForm.DomainName;   
            if (string.IsNullOrWhiteSpace(importForm.DomainName))
            {
                ModelState.AddModelError($"ImportForm.{nameof(importForm.DomainName)}", "accelerator.deployment.validation.required".AsAngularResourceString());
                isValid = false;
            }
            else if (importForm.DomainName.Contains(" "))
            {
                ModelState.AddModelError($"ImportForm.{nameof(importForm.DomainName)}", "accelerator.deployment.validation.space".AsAngularResourceString());
                isValid = false;
            }
            else if (!Uri.TryCreate(tempDomainName, UriKind.RelativeOrAbsolute, out var uri))
            {
                ModelState.AddModelError($"ImportForm.{nameof(importForm.DomainName)}", "general.hostnameisnotvalid".AsAngularResourceString());
                isValid = false;
            }
            else
            {
                var domain = AcceleratorPackage.ExtractDomainName(importForm.DomainName);
                var urlPath = AcceleratorPackage.ExtractUrlPath(importForm.DomainName).NullIfEmpty();

                if (urlPath?.Contains("/") == true)
                {
                    ModelState.AddModelError($"ImportForm.{nameof(importForm.DomainName)}", "accelerator.deployment.validation.urlpath".AsAngularResourceString());
                    isValid = false;
                }
                else
                {
                    var domainName = _domainNameService.Get(domain);
                    if (domainName != null && _channelService.GetAll().Any(x => x
                        .DomainNameLinks
                        .Any(z => z.DomainNameSystemId == domainName.SystemId && string.Equals(z.UrlPrefix, urlPath, StringComparison.OrdinalIgnoreCase))))
                    {
                        ModelState.AddModelError($"ImportForm.{nameof(importForm.DomainName)}", "accelerator.deployment.validation.domainnameexists".AsAngularResourceString());
                        isValid = false;
                    }
                }
            }

            return isValid;
        }
    }
}