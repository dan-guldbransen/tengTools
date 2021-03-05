using System;
using System.Globalization;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Products;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Extensions
{
    /// <summary>
    /// ProductModel extensions
    /// </summary>
    public static class ProductModelExtensions
    {
        /// <summary>
        /// Gets the products name.
        /// If <see cref="ProductModel.UseVariantUrl" /> is true, priority is given to the field value from Variant.
        /// </summary>
        /// <param name="productModel">The product model.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>System.String.</returns>
        public static string GetName(this ProductModel productModel, CultureInfo cultureInfo)
        {
            return productModel.GetValue<string>(SystemFieldDefinitionConstants.Name, cultureInfo);
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <param name="productModel">The product model.</param>
        /// <param name="webSiteSystemId">The web site system identifier.</param>
        /// <param name="currentCategory">The current category.</param>
        /// <param name="channelSystemId">The channel system identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetUrl(this ProductModel productModel, Guid webSiteSystemId, Category currentCategory = null, Guid? channelSystemId = null)
        {
            return productModel.UseVariantUrl ? productModel.SelectedVariant.GetUrl(currentCategory: currentCategory, channelSystemId: channelSystemId) : productModel.BaseProduct.GetUrl(webSiteSystemId, currentCategory: currentCategory, channelSystemId: channelSystemId);
        }
    }
}