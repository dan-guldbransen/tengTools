using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Builders.Framework
{
    public class CartViewModelBuilder : IViewModelBuilder<CartViewModel>
    {
        private readonly CartService _cartService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly BaseProductService _baseProductService;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly UnitOfMeasurementService _unitOfMeasurementService;
        private readonly UrlService _urlService;
        private readonly VariantService _variantService;

        public CartViewModelBuilder(CartService cartService, RequestModelAccessor requestModelAccessor, UrlService urlService,
            VariantService variantService, BaseProductService baseProductService, ModuleECommerce moduleECommerce,
            UnitOfMeasurementService unitOfMeasurementService)
        {
            _cartService = cartService;
            _requestModelAccessor = requestModelAccessor;
            _urlService = urlService;
            _variantService = variantService;
            _baseProductService = baseProductService;
            _moduleECommerce = moduleECommerce;
            _unitOfMeasurementService = unitOfMeasurementService;
        }

        public CartViewModel Build(Cart cart)
        {
            var amount = cart.IncludeVAT
                ? cart.OrderCarrier.TotalOrderRow + cart.OrderCarrier.TotalOrderRowVAT
                : cart.OrderCarrier.TotalOrderRow;
            var discount = cart.IncludeVAT ? cart.OrderCarrier.TotalDiscountWithVAT : cart.OrderCarrier.TotalDiscount;
            var deliveryCost = cart.IncludeVAT ? cart.OrderCarrier.TotalDeliveryCostWithVAT : cart.OrderCarrier.TotalDeliveryCost;
            var paymentCost = cart.IncludeVAT ? cart.OrderCarrier.TotalFeeWithVAT : cart.OrderCarrier.TotalFee;
            var totalVat = cart.OrderCarrier.TotalVAT;
            var checkoutPage = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage).MapTo<LinkModel>();

            return new CartViewModel
            {
                OrderTotal = _cartService.FormatAmount(amount),
                Quantity = _cartService.GetNumberOfProductsAsString(),
                CheckoutUrl = checkoutPage?.Href,
                OrderRows = BuildOrderRows(cart.Currency, cart.IncludeVAT),
                GrandTotal = _cartService.FormatAmount(cart.OrderCarrier.GrandTotal),
                Discount = _cartService.FormatAmount(discount),
                DeliveryCost = _cartService.FormatAmount(deliveryCost),
                PaymentCost = _cartService.FormatAmount(paymentCost),
                Vat = _cartService.FormatAmount(totalVat),
            };
        }

        private IList<OrderRowViewModel> BuildOrderRows(Currency currency, bool includeVat)
        {
            return _requestModelAccessor.RequestModel.Cart.OrderCarrier.OrderRows.Where(row => !row.CarrierState.IsMarkedForDeleting).Select(row => BuildOrderRow(row, currency, includeVat)).ToList();
        }

        private OrderRowViewModel BuildOrderRow(OrderRowCarrier orderRow, Currency currency, bool includeVat)
        {
            var variant = _variantService.Get(orderRow.ArticleNumber);
            var baseProduct = _baseProductService.Get(variant?.BaseProductSystemId ?? Guid.Empty);

            var name = variant?.Localizations.CurrentCulture.Name.NullIfWhiteSpace() ?? baseProduct?.Localizations.CurrentCulture.Name.NullIfWhiteSpace() ?? orderRow.ArticleNumber;
            var url = variant == null ? null : _urlService.GetUrl(variant);
            var image = (variant?.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images)?.FirstOrDefault()
                ?? baseProduct?.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images)?.FirstOrDefault())
                .MapTo<ImageModel>();

            var price = GetPriceModel(orderRow, currency, includeVat);
            var totalPrice = new ProductPriceModel.PriceItem(decimal.MinusOne, orderRow.TotalPrice, orderRow.VATPercentage, orderRow.TotalPriceWithVAT)
            {
                FormatPrice = b => currency.Format(includeVat ? orderRow.TotalPriceWithVAT : orderRow.TotalPrice, b, CultureInfo.CurrentUICulture)
            };

            var unitOfMeasurement = _unitOfMeasurementService.Get(orderRow.SKUCode);
            var unitOfMeasurementFormatString = $"0.{new string('0', unitOfMeasurement?.DecimalDigits ?? 0)}";

            var model = new OrderRowViewModel
            {
                ArticleNumber = orderRow.ArticleNumber,
                Name = name,
                RowSystemId = orderRow.ID,
                Quantity = orderRow.Quantity,
                QuantityString = orderRow.Quantity.ToString(unitOfMeasurementFormatString, CultureInfo.CurrentUICulture.NumberFormat).Replace(",", "."),
                Url = url,
                Image = image?.GetUrlToImage(Size.Empty, new Size(200, 120)).Url,
                TotalPrice = totalPrice.FormatPrice(true),
                Price = price.Price.FormatPrice(true),
                CampaignPrice = price.CampaignPrice?.FormatPrice(true),
                IsFreeGift = orderRow.IsAutoGenerated
            };

            if (orderRow.CampaignID == Guid.Empty)
            {
                return model;
            }
            
            var campaign = _moduleECommerce.Campaigns.GetCampaign(orderRow.CampaignID, _moduleECommerce.AdminToken);
            if (campaign == null)
            {
                return model;
            }
            var requestModel = _requestModelAccessor.RequestModel;
            var channel = campaign.Data.Channels.Find(x => x.ChannelId == requestModel.ChannelModel.SystemId);
            if (channel != null && channel.CampainPage != null && channel.CampainPage.EntitySystemId != Guid.Empty)
            {
                model.CampaignLink = channel.CampainPage.MapTo<LinkModel>()?.Href;
            }

            return model;
        }

        private ProductPriceModel GetPriceModel(OrderRowCarrier orderRow, Currency currency, bool includeVat)
        {
            ProductPriceModel.CampaignPriceItem campaignPrice = null;
            if (orderRow.CampaignID != Guid.Empty)
            {
                var campaignPriceWithVat = orderRow.UnitCampaignPrice * (1 + orderRow.VATPercentage);
                campaignPrice = new ProductPriceModel.CampaignPriceItem(0, orderRow.UnitCampaignPrice, orderRow.VATPercentage, campaignPriceWithVat, orderRow.CampaignID)
                {
                    FormatPrice = b => currency.Format(includeVat ? campaignPriceWithVat : orderRow.UnitCampaignPrice, b, CultureInfo.CurrentUICulture)
                };
            }
            var priceWithVat = orderRow.UnitListPrice * (1 + orderRow.VATPercentage);
            var price = new ProductPriceModel.PriceItem(decimal.MinusOne, orderRow.UnitListPrice, orderRow.VATPercentage, priceWithVat)
            {
                FormatPrice = b => currency.Format(includeVat ? priceWithVat : orderRow.UnitListPrice, b, CultureInfo.CurrentUICulture)
            };
            return new ProductPriceModel
            {
                CampaignPrice = campaignPrice,
                Price = price
            };
        }
    }
}
