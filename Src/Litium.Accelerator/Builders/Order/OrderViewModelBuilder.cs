using System;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels.Order;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Deliveries;
using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Foundation.Modules.ECommerce.Translations;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Products;
using Litium.Web.Models.Websites;
using Litium.Websites;

namespace Litium.Accelerator.Builders.Order
{
    public class OrderViewModelBuilder : IViewModelBuilder<OrderViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly LanguageService _languageService;
        private readonly PaymentService _paymentService;
        private readonly PageService _pageServcie;
        private readonly UrlService _urlService;
        private readonly OrganizationService _organizationService;
        private readonly PersonStorage _personStorage;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly SecurityToken _securityToken;
        private readonly ProductModelBuilder _productModelBuilder;
        private readonly VariantService _variantService;
        private readonly UnitOfMeasurementService _unitOfMeasurementService;

        public OrderViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            FieldDefinitionService fieldDefinitionService,
            LanguageService languageService,
            PaymentService paymentService,
            PageService pageServcie,
            UrlService urlService,
            ModuleECommerce moduleECommerce,
            SecurityToken securityToken,
            ProductModelBuilder productModelBuilder,
            VariantService variantService,
            UnitOfMeasurementService unitOfMeasurementService,
            OrganizationService organizationService,
            PersonStorage personStorage)
        {
            _requestModelAccessor = requestModelAccessor;
            _fieldDefinitionService = fieldDefinitionService;
            _languageService = languageService;
            _paymentService = paymentService;
            _pageServcie = pageServcie;
            _urlService = urlService;

            _moduleECommerce = moduleECommerce;
            _securityToken = securityToken;
            _productModelBuilder = productModelBuilder;
            _variantService = variantService;
            _unitOfMeasurementService = unitOfMeasurementService;
            _organizationService = organizationService;
            _personStorage = personStorage;
        }

        public virtual OrderViewModel Build(Guid id, bool print)
            => Build(_requestModelAccessor.RequestModel.CurrentPageModel, id, print);

        public OrderViewModel Build(PageModel pageModel, Guid id, bool print)
        {
            var model = pageModel.MapTo<OrderViewModel>();

            var order = _moduleECommerce.Orders.GetOrder(id, _securityToken);
            if (order != null)
            {
                model.Order = Build(order);
            }
            
            model.IsPrintPage = print;
            model.OrderHistoryUrl = GetOrderHistoryUrl(pageModel.Page.ParentPageSystemId);
            model.IsBusinessCustomer = _personStorage.CurrentSelectedOrganization != null;

            if (model.IsBusinessCustomer)
            {
                model.HasApproverRole = _personStorage.HasApproverRole;
            }

            return model;
        }

        public OrderDetailsViewModel Build(Foundation.Modules.ECommerce.Orders.Order order)
        {
            var includeVat = _requestModelAccessor.RequestModel.Cart.IncludeVAT;
            var currency =  order.Currency.ProxyFor;
            var languageSystemId = _languageService.Get(CultureInfo.CurrentUICulture).SystemId;

            var paymentMethodDisplayName = string.Empty;
            var paymentInfo = order.PaymentInfo?.FirstOrDefault();
            if (paymentInfo != null)
            {
                var paymentMethod = _paymentService.GetPaymentMethod(paymentInfo.PaymentMethod, paymentInfo.PaymentProviderName, Solution.Instance.SystemToken);
                if (paymentMethod != null)
                {
                    paymentMethodDisplayName = paymentMethod.GetDisplayName(languageSystemId);
                }
            }

            var deliveryMethodDisplayName = string.Empty;
            var delivery = order.Deliveries.FirstOrDefault();
            if (delivery != null)
            {
                deliveryMethodDisplayName = delivery.DeliveryMethod.GetDisplayName(languageSystemId);
            }

            var statusTranslation = _moduleECommerce.StatusTranslations.Get(PluginStringTranslationType.OrderStatus, order.OrderStatus, languageSystemId, _moduleECommerce.AdminToken)?.TranslatedText.NullIfWhiteSpace()
                                    ?? _moduleECommerce.Orders.GetAllOrderStates(_moduleECommerce.AdminToken).FirstOrDefault(x => x.Key == order.OrderStatus).Value?.ToUpper();

            var deliveryDate = order.Deliveries.FirstOrDefault()?.ActualDeliveryDate;

            var orderTotalFee = includeVat ? order.TotalFeeWithVAT : order.TotalFee;
            var orderTotalDiscountAmount = includeVat ? order.TotalDiscountWithVAT : order.TotalDiscount;
            var orderTotalDeliveryCost = includeVat ? order.TotalDeliveryCostWithVAT : order.TotalDeliveryCost;
            var totalVat = order.TotalVAT;
            var grandTotal = order.GrandTotal;
            string organizationName = string.Empty;
            if (order.CustomerInfo.OrganizationID != Guid.Empty)
            {
                var organization = _organizationService.Get(order.CustomerInfo.OrganizationID);
                organizationName = organization != null ? organization.Name : order.CustomerInfo.Address.OrganizationName;
            }

            var orderDetails = new OrderDetailsViewModel
            {
                OrderId = order.ID,
                ExternalOrderID = order.ExternalOrderID,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                Status = statusTranslation,
                OrderTotalFee = currency.Format(orderTotalFee, true, CultureInfo.CurrentUICulture),
                OrderTotalDiscountAmount = orderTotalDiscountAmount > 0 ? currency.Format(orderTotalDiscountAmount, true, CultureInfo.CurrentUICulture) : string.Empty,
                OrderTotalDeliveryCost = currency.Format(orderTotalDeliveryCost, true, CultureInfo.CurrentUICulture),
                OrderTotalVat = currency.Format(totalVat, true, CultureInfo.CurrentUICulture),
                OrderGrandTotal = currency.Format(grandTotal, true, CultureInfo.CurrentUICulture),
                OrderRows = order.OrderRows.Select(x => BuildOrderRow(x, currency)).ToList(),
                PaymentMethod = paymentMethodDisplayName,
                DeliveryMethod = deliveryMethodDisplayName,
                ActualDeliveryDate = deliveryDate,
                Deliveries = order.Deliveries.Select(x => BuildDeliveryRow(x, includeVat, currency, languageSystemId)).ToList(),
                CustomerInfo = new OrderDetailsViewModel.CustomerInfoModel
                {
                    CustomerNumber = order.CustomerInfo?.CustomerNumber,
                    FirstName = order.CustomerInfo?.Address?.FirstName,
                    LastName = order.CustomerInfo?.Address?.LastName,
                    Address1 = order.CustomerInfo?.Address?.Address1,
                    Zip = order.CustomerInfo?.Address?.Zip,
                    City = order.CustomerInfo?.Address?.City,
                    Country = string.IsNullOrEmpty(order.CustomerInfo?.Address?.Country) 
                                ? string.Empty 
                                : new RegionInfo(order.CustomerInfo?.Address?.Country).DisplayName
                },
                MerchantOrganizationNumber = _personStorage.CurrentSelectedOrganization?.Id,
                CompanyName = organizationName
            };

            return orderDetails;
        }

        private OrderDetailsViewModel.OrderRowItem BuildOrderRow(OrderRow orderRow, Currency currency)
        {
            var productModel = _productModelBuilder.BuildFromVariant(_variantService.Get(orderRow.ArticleNumber));
            var shoppingCartIncludeVat = _requestModelAccessor.RequestModel.Cart.IncludeVAT;

            var unitOfMeasurement = _unitOfMeasurementService.Get(orderRow.SKUCode);
            var unitOfMeasurementFormatString = $"0.{new string('0', unitOfMeasurement?.DecimalDigits ?? 0)}";

            var totalPrice = shoppingCartIncludeVat ? orderRow.TotalPriceWithVat : orderRow.TotalPrice;

            var campaign = orderRow.Campaign;
            var model = new OrderDetailsViewModel.OrderRowItem
            {
                DeliveryId = orderRow.DeliveryID,
                Brand = productModel == null ? null : _fieldDefinitionService.Get<ProductArea>("Brand")?.GetTranslation(productModel.GetValue<string>("Brand")),
                Name = productModel == null ? orderRow.ArticleNumber : productModel.GetValue<string>(SystemFieldDefinitionConstants.Name),
                QuantityString = $"{orderRow.Quantity.ToString(unitOfMeasurementFormatString, CultureInfo.CurrentUICulture.NumberFormat)} {unitOfMeasurement?.Localizations.CurrentUICulture.Name ?? orderRow.SKUCode}",
                PriceInfo = new ProductPriceModel
                {
                    CampaignPrice = campaign == null ? null : SetFormattedPrice(new ProductPriceModel.CampaignPriceItem(0, orderRow.UnitCampaignPrice, orderRow.VATPercentage, orderRow.UnitCampaignPrice * (1 + orderRow.VATPercentage), campaign.ID), shoppingCartIncludeVat, currency),
                    Price = SetFormattedPrice(new ProductPriceModel.PriceItem(0, orderRow.UnitListPrice, orderRow.VATPercentage, orderRow.UnitListPrice * (1 + orderRow.VATPercentage)), shoppingCartIncludeVat, currency)
                },
                TotalPrice = currency.Format(totalPrice, true, CultureInfo.CurrentUICulture),
                Link = productModel?.SelectedVariant.MapTo<LinkModel>()
            };

            return model;
        }

        private OrderDetailsViewModel.DeliveryItem BuildDeliveryRow(Delivery delivery, bool includeVat, Currency currency, Guid languageSystemId)
        {
            var deliveryCost = includeVat ? delivery.TotalDeliveryCostWithVat : delivery.TotalDeliveryCost;
            var model = new OrderDetailsViewModel.DeliveryItem
            {
                DeliveryId = delivery.ID,
                DeliveryMethodTitle = delivery.DeliveryMethod.GetDisplayName(languageSystemId),
                DeliveryRowTotalCost = currency.Format(deliveryCost, true, CultureInfo.CurrentUICulture)
            };
            model.Address.MapFrom(delivery.Address);

            return model;
        }

        private string GetOrderHistoryUrl(Guid sytemId)
        {
            var orderHistoryPage = _pageServcie.Get(sytemId);
            return _urlService.GetUrl(orderHistoryPage);
        }

        private T SetFormattedPrice<T>(T item, bool shoppingCartIncludeVat, Currency currency) where T : ProductPriceModel.PriceItem
        {
            var price = shoppingCartIncludeVat ? item.PriceWithVat : item.Price;
            item.FormatPrice = x => currency.Format(price, x, CultureInfo.CurrentUICulture);
            return item;
        }
    }
}
