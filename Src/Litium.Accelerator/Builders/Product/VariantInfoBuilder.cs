using System;
using System.Linq;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Product;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Web.Models.Products;
using Litium.Websites;

namespace Litium.Accelerator.Builders.Product
{
    public class VariantInfoBuilder : IViewModelBuilder<VariantInfo>
    {
        private readonly VariantService _variantService;
        private readonly FileService _fileService;
        private readonly ProductPriceModelBuilder _priceModelBuilder;
        private readonly CurrencyService _currencyService;
        private readonly ChannelService _channelService;
        private readonly PriceListItemService _priceListItemService;

        public VariantInfoBuilder(
            VariantService variantService, 
            FileService fileService, 
            ProductPriceModelBuilder priceModelBuilder, 
            WebsiteService websiteService, 
            CurrencyService currencyService, 
            ChannelService channelService, 
            PriceListItemService priceListItemService)
        {
            _variantService = variantService;
            _fileService = fileService;
            _priceModelBuilder = priceModelBuilder;
            _currencyService = currencyService;
            _channelService = channelService;
            _priceListItemService = priceListItemService;
        }

        public VariantInfo Build(Guid variantSystemId, Guid channelSystemId, DataFilterBase dataFilter = null)
        {
            if (variantSystemId == Guid.Empty)
            {
                return null;
            }
            var entity = _variantService.Get(variantSystemId);
            if (entity == null)
            {
                return null;
            }
            var pageModel = new VariantInfo() { SystemId = variantSystemId };
            var currency = _currencyService.Get(id: ((ProductDataFilter) dataFilter)?.Currency)??_currencyService.GetBaseCurrency();
            BuildFields(pageModel, entity, dataFilter?.Culture);
            BuildPrices(pageModel, entity, currency, channelSystemId);
            return pageModel;
        }

        private void BuildFields(VariantInfo pageModel, Variant entity, string culture)
        {
            pageModel.Id = entity.Id;
            var fields = entity.Fields;
            pageModel.Name = fields.GetName(culture);
            pageModel.Description = fields.GetDescription(culture);
            pageModel.Images = fields.GetImageUrls(_fileService);
            pageModel.Size = fields.GetSize();
            pageModel.Color = fields.GetColor();
        }

        private void BuildPrices(VariantInfo pageModel, Variant entity, Currency currency, Guid channelSystemId)
        {
            var channel = _channelService.Get(channelSystemId);
            var productPriceModel = _priceModelBuilder.Build(entity, currency, channel);
            if (productPriceModel.Price == null)
            {
                return;
            }
            pageModel.ListPrice = productPriceModel.Price.Price;
            pageModel.VatPercentage = productPriceModel.Price.VatPercentage;
            pageModel.CampaignPriceWithVat = productPriceModel.CampaignPrice.PriceWithVat;
        }
    }
}
