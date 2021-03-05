using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Studio.Extenssions;
using System;
using System.Linq;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Checks whether product prices has not changed in cart, since last time an order calculation was done.
    /// </summary>
    public class ProductPricesHasNotChanged : IPreOrderValidationRule
    {
        private readonly IOrderRowFactory _orderRowFactory;
        private readonly SecurityToken _securityToken;
        private readonly LanguageService _languageService;

        public ProductPricesHasNotChanged(
            IOrderRowFactory orderRowFactory,
            SecurityToken securityToken,
            LanguageService languageService)
        {
            _orderRowFactory = orderRowFactory;
            _securityToken = securityToken;
            _languageService = languageService;
        }

        /// <summary>
        ///     Validates the specified order carrier.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="checkoutFlowInfo">The checkout flow info.</param>
        public void Validate(OrderCarrier orderCarrier, CheckoutFlowInfo checkoutFlowInfo)
        {
            if (orderCarrier.OrderRows.Count != 0)
            {
                var languageSystemId = _languageService.Get(System.Globalization.CultureInfo.CurrentUICulture)?.SystemId
                    ?? _languageService.GetDefault().SystemId;

                var personId = orderCarrier.CustomerInfo?.PersonID ?? _securityToken.UserID;
                var orderRows = from orderRowCarrier in orderCarrier.OrderRows
                                let result = _orderRowFactory.Create(new ShoppingCartItemCarrier
                                {
                                    ArticleNumber = orderRowCarrier.ArticleNumber,
                                    ProductID = orderRowCarrier.ProductID,
                                    CustomerID = personId,
                                    LanguageID = languageSystemId,
                                    Quantity = orderRowCarrier.Quantity,
                                    Comments = orderRowCarrier.Comments
                                },
                                    orderCarrier.WebSiteID,
                                    orderCarrier.CurrencyID,
                                    personId,
                                    orderCarrier.CountryID,
                                    _securityToken)
                                where result != null
                                where Math.Round(orderRowCarrier.UnitListPrice, 2) != Math.Round(result.UnitListPrice, 2) ||
                                      Math.Round(orderRowCarrier.VATPercentage, 2) != Math.Round(result.VATPercentage, 2)
                                select orderRowCarrier;

                if (orderRows.Any())
                {
                    throw new PreOrderValidationException("accelerator.validation.productprices.haschanged".AsAngularResourceString());
                }
            }
        }
    }
}

