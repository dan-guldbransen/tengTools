using System;
using System.Web;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Foundation.Security;
using Litium.Products;
using Litium.Products.StockStatusCalculator;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(StockService), FallbackService = true)]
    internal class StockServiceImpl : StockService
    {
        private readonly IStockStatusCalculator _stockStatusCalculator;
        private readonly CartAccessor _cartAccessor;

        public StockServiceImpl(IStockStatusCalculator stockStatusCalculator, CartAccessor cartAccessor)
        {
            _stockStatusCalculator = stockStatusCalculator;
            _cartAccessor = cartAccessor;
        }

        public override string GetStockStatusDescription(Variant variant, string sourceId = null)
        {
            return GetStockStatus(variant, sourceId)?.Description ?? string.Empty;
        }

        public override bool HasStock(Variant variant, string sourceId = null)
        {
            var stock = GetStockStatus(variant, sourceId);
            return (stock != null && stock.InStockQuantity.HasValue && stock.InStockQuantity > 0m);
        }

        private StockStatusCalculatorResult GetStockStatus(Variant variant, string sourceId)
        {
            var calculatorArgs = new StockStatusCalculatorArgs
            {
                Date = HttpContext.Current?.Timestamp ?? DateTime.UtcNow,
                UserSystemId = SecurityToken.CurrentSecurityToken.UserID,
                WebSiteSystemId = _cartAccessor.Cart.OrderCarrier.WebSiteID,
                SourceId = sourceId,
                CountrySystemId = _cartAccessor.Cart.OrderCarrier.CountryID
            };
            var calculatorItemArgs = new StockStatusCalculatorItemArgs
            {
                VariantSystemId = variant.SystemId,
                Quantity = decimal.One
            };

            return _stockStatusCalculator.GetStockStatuses(calculatorArgs, calculatorItemArgs).TryGetValue(variant.SystemId, out StockStatusCalculatorResult calculatorResult)
                ? calculatorResult
                : null;
        }
    }
}
