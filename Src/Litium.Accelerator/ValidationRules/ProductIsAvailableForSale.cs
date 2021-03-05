using Litium.Foundation.GUI;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Checkout;
using Litium.Foundation.Modules.ECommerce.Plugins.Orders;
using Litium.Foundation.Security;
using Litium.Studio.Extenssions;
using System.Linq;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    ///     Validate whether product is still avaliabble to buy, since it was last put to cart.
    /// </summary>
    public class ProductIsAvailableForSale : IPreOrderValidationRule
    {
        private readonly IOrderRowFactory _orderRowFactory;
        private readonly SecurityToken _securityToken;

        public ProductIsAvailableForSale(IOrderRowFactory orderRowFactory, SecurityToken securityToken)
        {
            _orderRowFactory = orderRowFactory;
            _securityToken = securityToken;
        }

        /// <summary>
        ///     Validates the specified order carrier.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="checkoutFlowInfo">The checkout flow info.</param>
        public void Validate(OrderCarrier orderCarrier, CheckoutFlowInfo checkoutFlowInfo)
        {
            var personId = orderCarrier.CustomerInfo?.PersonID ?? _securityToken.UserID;
            var orderRows = orderCarrier.OrderRows.Select(orderRowCarrier =>
                _orderRowFactory.Create(new ShoppingCartItemCarrier
                {
                    ArticleNumber = orderRowCarrier.ArticleNumber,
                    ProductID = orderRowCarrier.ProductID,
                    CustomerID = personId,
                    LanguageID = FoundationContext.Current.LanguageID,
                    Quantity = orderRowCarrier.Quantity,
                    Comments = orderRowCarrier.Comments
                },
                    orderCarrier.WebSiteID,
                    orderCarrier.CurrencyID,
                    personId,
                    orderCarrier.CountryID,
                    _securityToken));

            if (orderRows.Any(result => result == null))
            {
                throw new PreOrderValidationException("accelerator.validation.product.nolongeravailableforsale".AsAngularResourceString());
            }
        }
    }
}

