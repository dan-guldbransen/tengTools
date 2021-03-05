using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Litium.ComponentModel;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Foundation.Modules.ProductCatalog;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Security;
using Litium.Studio.Extenssions;
using Litium.Studio.Plugins.Suggestions;
using Litium.Web;
using Litium.Websites;

namespace Litium.Accelerator.Deployments
{
    public class AcceleratorPackage : IPackage
    {
        private readonly AssortmentService _assortmentService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly StructureInfoService _structureInfoService;
        private readonly MarketService _marketService;
        private readonly ChannelService _channelService;
        private readonly CurrencyService _currencyService;
        private readonly WebsiteService _websiteService;
        private readonly InventoryService _inventoryService;
        private readonly PriceListService _priceListService;
        private readonly DomainNameService _domainNameService;
        private readonly GroupService _groupService;
        private readonly PersonService _personService;
        private readonly FolderService _folderService;
        private readonly String _storagePath = "~/Site/Deployments";
        private readonly LanguageService _languageService;
        private readonly SlugifyService _slugifyService;

        public AcceleratorPackage(
            AssortmentService assortmentService,
            FieldTemplateService fieldTemplateService,
            StructureInfoService structureInfoService,
            MarketService marketService,
            ChannelService channelService,
            CurrencyService currencyService,
            WebsiteService websiteService,
            InventoryService inventoryService,
            PriceListService priceListService,
            DomainNameService domainNameService,
            GroupService groupService,
            FolderService folderService,
            PersonService personService,
            LanguageService languageService,
            SlugifyService slugifyService)
        {
            _assortmentService = assortmentService;
            _fieldTemplateService = fieldTemplateService;
            _structureInfoService = structureInfoService;
            _marketService = marketService;
            _channelService = channelService;
            _currencyService = currencyService;
            _websiteService = websiteService;
            _inventoryService = inventoryService;
            _priceListService = priceListService;
            _domainNameService = domainNameService;
            _groupService = groupService;
            _folderService = folderService;
            _personService = personService;
            _languageService = languageService;
            _slugifyService = slugifyService;
        }

        public string Name => "Accelerator";

        public string Path => "~/Litium/UI/settings/iframe/%2FLitium%2FDeployment";
        public string Type { get; set; }

        public PackageInfo CreatePackageInfo(StructureInfo structureInfo, string name, string domainName)
        {
            Type = name.Replace(" ", "");
            CreateBasicMaping(structureInfo);

            var assortment = CreateAssortment(structureInfo, name, domainName);

            CreateDomainName(domainName, structureInfo);
            CreateWebsite(name, structureInfo);
            if (structureInfo.Website.Market != null)
            {
                CreateMarket(structureInfo, assortment.SystemId, name);
            }
            var channel = CreateChannel(structureInfo, structureInfo.Website.Website.SystemId, ExtractUrlPath(domainName), name);
            var folderTemplateId = _fieldTemplateService.GetAll().First(c => (c is FolderFieldTemplate)).SystemId;
            var newFolder = new Folder(folderTemplateId, name);

            var visitorGroupSystemId = (_groupService.Get<Group>("Visitors") ?? _groupService.Get<Group>("Besökare"))?.SystemId ?? Guid.Empty;
            if (visitorGroupSystemId != Guid.Empty)
            {
                newFolder.AccessControlList.Add(new AccessControlEntry(Operations.Entity.Read, visitorGroupSystemId));
            }

            _folderService.Create(newFolder);
            var priceList = CreatePriceList(structureInfo.Website.Website, structureInfo);

            return new PackageInfo
            {
                Channel = channel,
                Market = structureInfo.Website.Market,
                Website = structureInfo.Website.Website,
                Assortment = assortment,
                Folder = newFolder,
                Inventory = CreateInventory(name, structureInfo),
                PriceList = priceList,
                DeliveryMethods = structureInfo.ECommerce.DeliveryMethod,
                PaymentMethods = structureInfo.ECommerce.PaymentMethod
            };
        }

        private Assortment CreateAssortment(StructureInfo structureInfo, string name, string domainName)
        {
            var assortment = new Assortment
            {
                Id = structureInfo.ProductCatalog.Assortment.Id + _slugifyService.Slugify(CultureInfo.CurrentCulture, name),
                AccessControlList = structureInfo.ProductCatalog.Assortment.AccessControlList.MakeWritable()
            };

            foreach (var language in _languageService.GetAll())
            {
                assortment.Localizations[language.CultureInfo].Name = name;
            }

            _assortmentService.Create(assortment);
            return assortment;
        }

        private void CreateBasicMaping(StructureInfo structureInfo)
        {
            //Visitor Group
            structureInfo.Mappings.Add(structureInfo.Foundation.VisitorGroupId, (_groupService.Get<Group>("Visitors") ?? _groupService.Get<Group>("Besökare"))?.SystemId ?? CreateVisitorGroup());
            //Language Map
            foreach (var item in structureInfo.Foundation.LanguageMap)
            {
                var culture = new CultureInfo(item.Value);
                structureInfo.Mappings.Add(item.Key,
                    Solution.Instance.Languages.Exists(culture)
                        ? Solution.Instance.Languages[culture].ID
                        : Solution.Instance.Languages.CreateLanguage(culture.NativeName, culture, false,
                            ModuleProductCatalog.Instance.AdminToken).ID);
            }
            //Currency Map
            foreach (var item in structureInfo.Foundation.Currencies)
            {
                var cu = _currencyService.Get(item.Id);
                if (cu == null)
                {
                    cu = new Currency(item.Id);
                    _currencyService.Create(cu);
                }
                if (!structureInfo.Mappings.ContainsKey(item.SystemId))
                {
                    structureInfo.Mappings.Add(item.SystemId, cu.SystemId);
                }
            }

            //Relation map
            if (structureInfo.Foundation.RelationTemplates != null)
            {
                var relationTemplates = _fieldTemplateService.GetAll().ToList();
                foreach (var template in structureInfo.Foundation.RelationTemplates)
                {
                    var fieldTemplate = relationTemplates.FirstOrDefault(x => x.Id == template.Value);
                    if (fieldTemplate != null)
                    {
                        structureInfo.Mappings.Add(template.Key, fieldTemplate.SystemId);
                    }
                }
            }

            // Template
            foreach (var marketTemplate in structureInfo.Website.MarketTemplateMap)
            {
                var template = _fieldTemplateService.Get<MarketFieldTemplate>(marketTemplate.Value);
                structureInfo.Mappings.Add(marketTemplate.Key, template?.SystemId ?? Guid.Empty);
            }

            // Market mappings
            structureInfo.Mappings.Add(structureInfo.Website.Market.SystemId, Guid.NewGuid());

            //Channel
            foreach (var channelTemplate in structureInfo.Website.ChannelTemplateMap)
            {
                var template = _fieldTemplateService.Get<ChannelFieldTemplate>(channelTemplate.Value);
                structureInfo.Mappings.Add(channelTemplate.Key, template?.SystemId ?? Guid.Empty);
            }
            structureInfo.Mappings.Add(structureInfo.Website.Channel.SystemId, Guid.NewGuid());

            //Website
            structureInfo.Mappings.Add(structureInfo.Website.Website.SystemId, Guid.NewGuid());
            foreach (var websiteTemplate in structureInfo.Website.WebsiteTemplateMap)
            {
                var template = _fieldTemplateService.Get<WebsiteFieldTemplate>(websiteTemplate.Value);
                structureInfo.Mappings.Add(websiteTemplate.Key, template?.SystemId ?? Guid.Empty);
            }
        }

        private Guid CreateVisitorGroup()
        {
            var defaultGroupTemplate = _fieldTemplateService.GetAll().OfType<GroupFieldTemplate>().FirstOrDefault();
            if (defaultGroupTemplate == null)
            {
                defaultGroupTemplate = new GroupFieldTemplate("Default Group Template");
                _fieldTemplateService.Create(defaultGroupTemplate);
            }
            var visitorGroup = new StaticGroup(defaultGroupTemplate.SystemId, "Visitors")
            {
                Id = "Visitors"
            };
            var everyone = _personService.Get("_everyone")?.MakeWritableClone();
            if (everyone != null)
            {
                everyone.GroupLinks.Add(new PersonToGroupLink(everyone.SystemId));
            }

            _groupService.Create(visitorGroup);
            _personService.Update(everyone);
            return visitorGroup.SystemId;
        }

        private void CreateMarket(StructureInfo structureInfo, Guid assortmentSystemId, string marketName)
        {
            var market = structureInfo.Website.Market.MakeWritableClone();
            market.SystemId = structureInfo.Id(market.SystemId);
            market.FieldTemplateSystemId = structureInfo.Id(market.FieldTemplateSystemId);
            _structureInfoService.AddProperties<WebsiteArea>(structureInfo, structureInfo.Website.Market.Fields, market.Fields, false);
            market.AssortmentSystemId = assortmentSystemId;
            foreach (var language in _languageService.GetAll())
            {
                market.Localizations[language.CultureInfo].Name = marketName;
            }
            market.Id = _slugifyService.Slugify(CultureInfo.CurrentCulture, marketName).NullIfEmpty();
            _marketService.Create(market);
            structureInfo.Website.Market = market;
        }

        private Channel CreateChannel(StructureInfo structureInfo, Guid websiteId, string urlPrefix, string channelName)
        {
            var channel = structureInfo.Website.Channel;
            var newChannel = new Channel(structureInfo.Id(channel.FieldTemplateSystemId))
            {
                GoogleTagManagerContainerId = channel.GoogleTagManagerContainerId,
                SystemId = structureInfo.Id(channel.SystemId),
                DomainNameLinks = new List<ChannelToDomainNameLink>
                {
                    new ChannelToDomainNameLink(structureInfo.Id(structureInfo.Website.DomainName.SystemId))
                    {
                        UrlPrefix = urlPrefix
                    }
                },
                GoogleAnalyticsAccountId = channel.GoogleAnalyticsAccountId
            };
            foreach (var language in _languageService.GetAll())
            {
                newChannel.Localizations[language.CultureInfo].Name = channelName;
            }

            if (channel.MarketSystemId != null)
            {
                newChannel.MarketSystemId = structureInfo.Id(structureInfo.Website.Market.SystemId);
            }
            if (channel.ProductLanguageSystemId.HasValue)
            {
                newChannel.ProductLanguageSystemId = structureInfo.Id(channel.ProductLanguageSystemId.Value);
            }
            if (channel.WebsiteLanguageSystemId.HasValue)
            {
                newChannel.WebsiteLanguageSystemId = structureInfo.Id(channel.WebsiteLanguageSystemId.Value);
            }
            newChannel.CountryLinks.Clear();

            newChannel.WebsiteSystemId = structureInfo.Id(websiteId);
            newChannel.Id = _slugifyService.Slugify(CultureInfo.CurrentCulture, channelName);

            _channelService.Create(newChannel);

            return newChannel;
        }

        private void CreateWebsite(string name, StructureInfo structureInfo)
        {
            var website = structureInfo.Website.Website.MakeWritableClone();
            website.Id = _slugifyService.Slugify(CultureInfo.CurrentCulture, name).NullIfEmpty();
            foreach (var language in _languageService.GetAll())
            {
                website.Localizations[language.CultureInfo].Name = name;
            }
            website.SystemId = structureInfo.Id(website.SystemId);
            website.FieldTemplateSystemId = structureInfo.Id(website.FieldTemplateSystemId);
            _websiteService.Create(website);
            structureInfo.Website.Website = website;
        }

        public StructureInfo GetStructureInfo()
        {
            var fileInfo = new FileInfo(GetStructureInfoPath());
            var info = fileInfo.LoadDataCompressedJson<StructureInfo>(true);

            fileInfo = new FileInfo(GetStructureInfoPath(".storage"));
            info.MediaArchive.FileData = fileInfo.LoadDataCompressedJson<Dictionary<Guid, byte[]>>();

            fileInfo = new FileInfo(GetStructureInfoPath(".pageData.storage"));
            info.Website.PageThumbnailData = fileInfo.LoadDataCompressedJson<Dictionary<Guid, byte[]>>();

            fileInfo = new FileInfo(GetStructureInfoPath(".blockData.storage"));
            info.Website.BlockThumbnailData = fileInfo.LoadDataCompressedJson<Dictionary<Guid, byte[]>>();

            return info;
        }

        public void PersistStructureInfo(StructureInfo structureInfo)
        {
            var maFileData = structureInfo.MediaArchive.FileData;
            var fileInfo = new FileInfo(GetStructureInfoPath(".storage"));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            fileInfo.PersistCompressedJson(maFileData);

            structureInfo.MediaArchive.FileData = null;

            var pageThumbnailData = structureInfo.Website.PageThumbnailData;
            fileInfo = new FileInfo(GetStructureInfoPath(".pageData.storage"));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            fileInfo.PersistCompressedJson(pageThumbnailData);

            structureInfo.Website.PageThumbnailData = null;

            var blockThumbnailData = structureInfo.Website.BlockThumbnailData;
            fileInfo = new FileInfo(GetStructureInfoPath(".blockData.storage"));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            fileInfo.PersistCompressedJson(blockThumbnailData);

            structureInfo.Website.BlockThumbnailData = null;


            fileInfo = new FileInfo(GetStructureInfoPath() + ".json");
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            fileInfo.PersistJson(structureInfo);

            fileInfo = new FileInfo(GetStructureInfoPath());
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            fileInfo.PersistCompressedJson(structureInfo);
        }

        public string GetStructureInfoPath(string extra = null)
        {
            return HostingEnvironment.MapPath($"{_storagePath}/Accelerator_{Type}{extra}.bin");
        }

        public string GetPackageStoragedPath()
        {
            return HostingEnvironment.MapPath(_storagePath);
        }

        private Inventory CreateInventory(string name, StructureInfo structureInfo)
        {
            var inventory = _inventoryService.Get(name.Replace(" ", ""));
            if (inventory != null)
            {
                return inventory;
            }

            inventory = new Inventory { Id = name.Replace(" ", "") };
            foreach (var language in Solution.Instance.Languages)
            {
                inventory.Localizations[language.Culture].Name = name;
            }

            _inventoryService.Create(inventory);
            structureInfo.Mappings.Add(structureInfo.ProductCatalog.Inventory.SystemId, inventory.SystemId);
            return inventory;
        }

        private PriceList CreatePriceList(Website webSite, StructureInfo structureInfo)
        {
            var priceList = _priceListService.Get(webSite.Id.Replace(" ", ""));
            if (priceList != null)
            {
                return priceList;
            }

            var currency = _currencyService.Get(structureInfo.ProductCatalog.CurrencyId) ??
                           _currencyService.GetBaseCurrency();

            priceList = new PriceList(structureInfo.Id(currency.SystemId))
            {
                Id = webSite.Id.Replace(" ", ""),
                Active = structureInfo.ProductCatalog.PriceList?.Active ?? true,
                AccessControlList = structureInfo.ProductCatalog.PriceList.AccessControlList.MakeWritable()
            };

            if (structureInfo.ProductCatalog.PriceList?.WebSiteLinks.Any(x => structureInfo.Id(x.WebSiteSystemId) == structureInfo.Website.Website.SystemId) ?? false)
            {
                priceList.WebSiteLinks.Add(new PriceListToWebSiteLink(webSite.SystemId));
            }

            foreach (var language in Solution.Instance.Languages)
            {
                priceList.Localizations[language.Culture].Name = webSite.Localizations[language.Culture].Name;
            }

            _priceListService.Create(priceList);
            if (structureInfo.ProductCatalog.PriceList != null)
            {
                structureInfo.Mappings.Add(structureInfo.ProductCatalog.PriceList.SystemId, priceList.SystemId);
            }

            return priceList;
        }

        private void CreateDomainName(string domain, StructureInfo structureInfo)
        {
            var name = ExtractDomainName(domain);

            var domainName = _domainNameService.Get(name);
            if (domainName == null)
            {
                domainName = new DomainName(name);
                _domainNameService.Create(domainName);
            }

            structureInfo.Website.DomainName = domainName;
            structureInfo.Mappings.Add(structureInfo.Website.DomainName.SystemId, domainName.SystemId);
        }

        public static string ExtractDomainName(string domain)
        {
            var uri = CreateUri(domain);
            return uri.Host;
        }

        public static string ExtractUrlPath(string domain)
        {
            var uri = CreateUri(domain);
            var path = uri.LocalPath.Substring(1);
            return string.IsNullOrWhiteSpace(path) ? null : path.Trim();
        }

        private static Uri CreateUri(string domain)
        {
            if (!domain.Contains("://"))
                domain = "http://" + domain;
            return new Uri(domain);
        }
    }
}

