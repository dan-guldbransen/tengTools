using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Litium.Blobs;
using Litium.Blocks;
using Litium.Data;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Websites;

namespace Litium.Accelerator.Deployments
{
    public class WebsiteStructurePackage : IStructurePackage<StructureInfo.WebsiteStructure>
    {
        private readonly PageService _pageService;
        private readonly DataService _dataService;
        private readonly ChannelService _channelService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly CountryService _countryService;
        private readonly MarketService _marketService;
        private readonly TaxClassService _taxClassService;
        private readonly BlockService _blockService;
        private readonly StructureInfoService _structureInfoService;
        private readonly BlobService _blobService;


        public WebsiteStructurePackage(PageService pageService, DataService dataService, ChannelService channelService, WebsiteService websiteService, FieldTemplateService fieldTemplateService, DomainNameService domainNameService, CountryService countryService, MarketService marketService, TaxClassService taxClassService, BlockService blockService, StructureInfoService structureInfoService, BlobService blobService)
        {
            _pageService = pageService;
            _dataService = dataService;
            _channelService = channelService;
            _fieldTemplateService = fieldTemplateService;
            _countryService = countryService;
            _marketService = marketService;
            _taxClassService = taxClassService;
            _blockService = blockService;
            _structureInfoService = structureInfoService;
            _blobService = blobService;
        }

        public StructureInfo.WebsiteStructure Export(PackageInfo packageInfo)
        {
            var childPages = GetPages(packageInfo.Website.SystemId).ToList();
            var channel = packageInfo.Channel;
            var countries = _countryService.GetAll().Where(x => channel.CountryLinks.Any(link => link.CountrySystemId == x.SystemId)).ToList();
            var market = channel.MarketSystemId != null ? _marketService.Get(channel.MarketSystemId.GetValueOrDefault()) : null;
            var taxclasses = _taxClassService.GetAll().ToList();
            var blocks = GetChildBlocks(childPages).ToList();

            return new StructureInfo.WebsiteStructure()
            {
                Channel = channel,
                Countries = countries,
                Market = market,
                Website = packageInfo.Website,
                ChildPages = childPages,
                PageThumbnailData = GetThumbnailData(childPages, Page.BlobAuthority, c => c.SystemId),
                BlockThumbnailData = GetThumbnailData(blocks, Block.BlobAuthority, c => c.SystemId),
                Blocks = blocks,
                TaxClasses = taxclasses,
                PageTemplateMap = GetPageTemplateMap(childPages),
                WebsiteTemplateMap = GetWebsiteTemplateMap(packageInfo.Website),
                ChannelTemplateMap = GetChannelTemplateMap(packageInfo.Channel),
                MarketTemplateMap = market != null ? GetMarketTemplateMap(market) : null,
                BlockTemplateMap = GetBlockTemplateMap()
            };
        }

        private Dictionary<Guid, byte[]> GetThumbnailData<T>(IEnumerable<T> entities, string entityAuthority, Func<T, Guid> getSystemId)
            where T : class
        {
            var thumbnailData = new Dictionary<Guid, byte[]>();
            foreach (var item in entities)
            {
                var systemId = getSystemId(item);
                var container = _blobService.Create(entityAuthority, systemId);
                var blob = container.GetDefault();
                if (!blob.Exists)
                {
                    continue;
                }
                thumbnailData[systemId] = GetPhysicalFileContent(blob);
            }
            return thumbnailData;
        }
        private byte[] GetPhysicalFileContent(Blob blob)
        {
            var result = new byte[blob.Length];
            using (var fileStream = blob.OpenRead())
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    reader.Read(result, 0, result.Length);
                }
            }
            return result;
        }

        private Dictionary<Guid, string> GetBlockTemplateMap()
        {
            return _fieldTemplateService.GetAll().OfType<BlockFieldTemplate>().ToDictionary(x => x.SystemId, y => y.Id);
        }

        private Dictionary<Guid, string> GetMarketTemplateMap(Market market)
        {
            return Enumerable.Repeat(_fieldTemplateService.Get<MarketFieldTemplate>(market.FieldTemplateSystemId), 1)
                .ToDictionary(x => x.SystemId, y => y.Id);
        }

        private Dictionary<Guid, string> GetChannelTemplateMap(Channel channel)
        {
            return Enumerable.Repeat(_fieldTemplateService.Get<ChannelFieldTemplate>(channel.FieldTemplateSystemId), 1)
                .ToDictionary(x => x.SystemId, y => y.Id);
        }

        private IEnumerable<Page> GetPages(Guid websiteSystemId)
        {
            var result = new List<Page>();

            GetChildPages(result, Guid.Empty, websiteSystemId);

            return result;
        }

        private void GetChildPages(List<Page> resultList, Guid parentSystemId, Guid? websiteSystemId = null)
        {
            var pages = _pageService.GetChildPages(parentSystemId, websiteSystemId).ToList();

            resultList.AddRange(pages);

            foreach (var page in pages)
            {
                GetChildPages(resultList, page.SystemId);
            }
        }

        private IEnumerable<Block> GetChildBlocks(IEnumerable<Page> pages)
        {
            var usedBlockIds = GetChildBlockSystemIds(pages);

            return usedBlockIds.Any() ? _blockService.Get(usedBlockIds).ToList() : Enumerable.Empty<Block>();
        }

        private IEnumerable<Guid> GetChildBlockSystemIds(IEnumerable<Page> pages)
        {
            var childBlockIds = new List<Guid>();
            foreach (var page in pages)
            {
                childBlockIds.AddRange(page.Blocks.Items.SelectMany(container => container.Items.Cast<BlockItemLink>().Select(x => x.BlockSystemId)).ToList());
            }

            return childBlockIds.Distinct();
        }

        private Dictionary<Guid, string> GetWebsiteTemplateMap(Website website)
        {
            return Enumerable.Repeat(_fieldTemplateService.Get<WebsiteFieldTemplate>(website.FieldTemplateSystemId), 1)
                .ToDictionary(x => x.SystemId, y => y.Id);
        }

        private Dictionary<Guid, string> GetPageTemplateMap(IEnumerable<Page> childPages)
        {
            var pageFieldTemplates =
                childPages.Select(x => _fieldTemplateService.Get<PageFieldTemplate>(x.FieldTemplateSystemId));

            var result = new Dictionary<Guid, string>();
            foreach (var pageFieldTemplate in pageFieldTemplates)
            {
                if (!result.ContainsKey(pageFieldTemplate.SystemId))
                {
                    result.Add(pageFieldTemplate.SystemId, pageFieldTemplate.Id);
                }
            }

            return result;
        }


        public void Import(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            CreateBlocks(structureInfo);
            CreatePages(structureInfo, packageInfo);

            if (structureInfo.Website.BlockThumbnailData != null)
            {
                ImportThumbnail(structureInfo.Website.BlockThumbnailData, structureInfo, Block.BlobAuthority);
            }

            if (structureInfo.Website.PageThumbnailData != null)
            {
                ImportThumbnail(structureInfo.Website.PageThumbnailData, structureInfo, Page.BlobAuthority);
            }
        }

        private void ImportThumbnail(Dictionary<Guid, byte[]> thumbnailData, StructureInfo structureInfo, string blobAuthority)
        {
            foreach (var thumbnail in thumbnailData)
            {
                using (var inputStream = new MemoryStream(thumbnail.Value))
                {
                    var container = _blobService.Create(blobAuthority, structureInfo.Id(thumbnail.Key));
                    if (container.GetDefault().Exists)
                    {
                        // since we are using the same Blob Uri over and over, we need to delete the container
                        // add write new file in order to delete generated thumbnails.
                        _blobService.Delete(container);
                    }
                    using (var stream = container.GetDefault().OpenWrite())
                    {
                        inputStream.CopyTo(stream);
                    }
                }
            }
        }

        private void CreatePages(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            using (var db = _dataService.CreateBatch())
            {
                foreach (var page in structureInfo.Website.ChildPages)
                {
                    var newPage = page.MakeWritableClone();
                    newPage.SystemId = structureInfo.Id(page.SystemId);
                    newPage.FieldTemplateSystemId = structureInfo.Id(page.FieldTemplateSystemId);
                    newPage.ParentPageSystemId = structureInfo.Id(page.ParentPageSystemId);
                    newPage.WebsiteSystemId = structureInfo.Id(page.WebsiteSystemId);
                    newPage.AccessControlList = _structureInfoService.GetAccessControlList(page.AccessControlList);
                    newPage.ChannelLinks = new List<PageToChannelLink> { new PageToChannelLink(packageInfo.Channel.SystemId) };
                    UpdateBlockSystemId(structureInfo, page, newPage);
                    _structureInfoService.AddProperties<WebsiteArea>(structureInfo, page.Fields, newPage.Fields, false);
                    db.Create(newPage);
                }
                db.Commit();
            }
        }

        private void UpdateBlockSystemId(StructureInfo structureInfo, Page oldPage, Page newPage)
        {
            var newBlockContainer = new BlockContainer();
            foreach (var blockContainer in oldPage.Blocks)
            {
                var newBlockItemContainer = new BlockItemContainer(blockContainer.Id);
                foreach (var oldItemLink in blockContainer.Items.OfType<BlockItemLink>())
                {
                    newBlockItemContainer.Items.Add(new BlockItemLink(structureInfo.Id(oldItemLink.BlockSystemId)));
                }
                newBlockContainer.Add(newBlockItemContainer);
            }
            newPage.Blocks.Clear();
            foreach (var item in newBlockContainer)
            {
                newPage.Blocks.Add(item);
            }
        }

        private void CreateBlocks(StructureInfo structureInfo)
        {
            using (var db = _dataService.CreateBatch())
            {
                foreach (var block in structureInfo.Website.Blocks)
                {
                    var newBlock = block.MakeWritableClone();
                    newBlock.SystemId = structureInfo.Id(block.SystemId);
                    newBlock.FieldTemplateSystemId = structureInfo.Id(block.FieldTemplateSystemId);
                    newBlock.AccessControlList = _structureInfoService.GetAccessControlList(block.AccessControlList);
                    _structureInfoService.AddProperties<BlockArea>(structureInfo, block.Fields, newBlock.Fields, false);
                    newBlock.ChannelLinks = UpdateBlockChannelLink(structureInfo, block.ChannelLinks);
                    db.Create(newBlock);
                }
                db.Commit();
            }
        }

        private ICollection<BlockToChannelLink> UpdateBlockChannelLink(StructureInfo structureInfo, ICollection<BlockToChannelLink> blockChannelLinks)
        {
            foreach (var channelLink in blockChannelLinks)
            {
                channelLink.ChannelSystemId = structureInfo.Id(channelLink.ChannelSystemId);
            }
            return blockChannelLinks;
        }

        public void PrepareImport(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            foreach (var page in structureInfo.Website.ChildPages)
            {
                structureInfo.Mappings.Add(page.SystemId, Guid.NewGuid());
            }
            foreach (var block in structureInfo.Website.Blocks ?? Enumerable.Empty<Block>())
            {
                structureInfo.Mappings.Add(block.SystemId, Guid.NewGuid());
            }

            PrepareTaxclassImport(structureInfo);
            PrepareCountryImport(structureInfo);

            foreach (var pageTemplate in structureInfo.Website.PageTemplateMap)
            {
                if (!structureInfo.Mappings.ContainsKey(pageTemplate.Key))
                {
                    var template = _fieldTemplateService.Get<PageFieldTemplate>(pageTemplate.Value)?.MakeWritableClone() as PageFieldTemplate;
                    structureInfo.Mappings.Add(pageTemplate.Key, template?.SystemId ?? Guid.Empty);
                }
            }
            foreach (var blockTemplate in structureInfo.Website.BlockTemplateMap ?? new Dictionary<Guid, string>())
            {
                var template = _fieldTemplateService.Get<BlockFieldTemplate>(blockTemplate.Value)?.MakeWritableClone() as BlockFieldTemplate;
                structureInfo.Mappings.Add(blockTemplate.Key, template?.SystemId ?? Guid.Empty);
            }
        }
        private void PrepareTaxclassImport(StructureInfo structureInfo)
        {
            var taxclasses = _taxClassService.GetAll().ToList();
            foreach (var taxClass in structureInfo.Website.TaxClasses)
            {
                var existTaxclass = string.IsNullOrEmpty(taxClass.Id) ? taxclasses.FirstOrDefault(x => x.Localizations.CurrentCulture.Name == taxClass.Localizations.CurrentCulture.Name ): taxclasses.FirstOrDefault(x => x.Id == taxClass.Id);
                if (existTaxclass != null)
                {
                    structureInfo.Mappings.Add(taxClass.SystemId, existTaxclass.SystemId);
                }
                else
                {
                    var taxclassId = Guid.NewGuid();
                    structureInfo.Mappings.Add(taxClass.SystemId, taxclassId);

                    var newTaxclass = taxClass.MakeWritableClone();
                    newTaxclass.SystemId = taxclassId;
                    _taxClassService.Create(newTaxclass);
                }
            }
        }
        private void PrepareCountryImport(StructureInfo structureInfo)
        {
            foreach (var country in structureInfo.Website.Countries)
            {
                var existCountry = _countryService.Get(country.Id)?.MakeWritableClone();
                if (existCountry != null)
                {
                    structureInfo.Mappings.Add(country.SystemId, existCountry.SystemId);
                    foreach (var countryToTaxClassLink in country.TaxClassLinks)
                    {
                        var taxclass = existCountry.TaxClassLinks.FirstOrDefault(x => x.TaxClassSystemId == structureInfo.Id(countryToTaxClassLink.TaxClassSystemId));
                        if (taxclass != null)
                        {
                            taxclass.VatRate = countryToTaxClassLink.VatRate;
                        }
                        else
                        {
                            existCountry.TaxClassLinks.Add(new CountryToTaxClassLink(structureInfo.Id(countryToTaxClassLink.TaxClassSystemId)) { VatRate = countryToTaxClassLink.VatRate });
                        }
                    }
                    _countryService.Update(existCountry);
                }
                else
                {
                    var countryId = Guid.NewGuid();
                    structureInfo.Mappings.Add(country.SystemId, countryId);

                    var newCountry = country.MakeWritableClone();
                    newCountry.SystemId = countryId;
                    newCountry.TaxClassLinks = country.TaxClassLinks.Select(x => new CountryToTaxClassLink(structureInfo.Id(x.TaxClassSystemId)) { VatRate = x.VatRate }).ToList();
                    newCountry.CurrencySystemId = structureInfo.Id(country.CurrencySystemId);
                    _countryService.Create(newCountry);
                }
            }
        }
    }
}
