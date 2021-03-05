using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Litium.Accelerator.ViewModels.Product;
using Litium.Studio.Extenssions;

namespace Litium.Accelerator.Mvc.Extensions
{
    public static class ProductItemViewModelHtmlExtensions
    {
        public static MvcHtmlString BuyButton<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, ProductItemViewModel>> expression, string cssClass = null, bool? isBuyButton = null)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            return htmlHelper.BuyButton((ProductItemViewModel)modelMetadata.Model, cssClass, isBuyButton);
        }

        public static MvcHtmlString BuyButton(this HtmlHelper<ProductItemViewModel> htmlHelper, string cssClass = null, bool? isBuyButton = null)
        {
            return htmlHelper.BuyButton(htmlHelper.ViewData.Model, cssClass, isBuyButton);
        }

        private static MvcHtmlString BuyButton(this HtmlHelper htmlHelper, ProductItemViewModel model, string cssClass, bool? isBuyButton)
        {
            var canBuy = model.ShowBuyButton && (isBuyButton.HasValue && isBuyButton.Value || model.UseVariantUrl) && model.IsInStock;
            var buyButtonTag = new TagBuilder("buy-button");
            var label = canBuy || (isBuyButton.HasValue && isBuyButton.Value) ? "product.buy".AsWebSiteString() : "product.show".AsWebSiteString();

            cssClass = $"button {(canBuy || (isBuyButton.HasValue && isBuyButton.Value) ? "buy-button" : "show-button")} {cssClass ?? string.Empty}".Trim();
            if (model.ShowBuyButton)
            {
                if (canBuy)
                {
                    var enabled = !string.IsNullOrEmpty(model.Url) && model.Price.HasPrice && model.IsInStock;
                    if (enabled)
                    {
                        var quantityFieldId = model.ShowQuantityField ? $"{model.QuantityFieldId}" : string.Empty;
                        buyButtonTag.Attributes.Add("data-article-number", model.Id);
                        buyButtonTag.Attributes.Add("data-quantity-field-id", quantityFieldId);
                    }
                    else
                    {
                        cssClass += " disabled";
                    }
                }
                else
                {
                    if (isBuyButton.HasValue && isBuyButton.Value)
                    {
                        cssClass += " disabled";
                    }
                    else
                    {
                        buyButtonTag.Attributes.Add("data-href", model.Url);
                    }
                }
            }
            else if (string.IsNullOrEmpty(model.Url))
            {
                cssClass += " disabled";
            }

            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                buyButtonTag.Attributes.Add("data-css-class", cssClass);
            }
            buyButtonTag.InnerHtml = $"<span><a class='{cssClass}'>{label}</a></span>";
            buyButtonTag.Attributes.Add("data-label", label);

            return MvcHtmlString.Create(buyButtonTag.ToString(TagRenderMode.Normal));
        }
    }
}