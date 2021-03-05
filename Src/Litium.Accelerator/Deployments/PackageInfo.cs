using System.Collections.Generic;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Websites;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Structure package info that contains information about import / export
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    public class PackageInfo
    {
        /// <summary>
        ///     Gets or sets the assortment.
        /// </summary>
        /// <value>
        ///     The assortment.
        /// </value>
        public Assortment Assortment { get; set; }
        /// <summary>
        ///     Gets or sets the delivery methods.
        /// </summary>
        public List<DeliveryMethodCarrier> DeliveryMethods { get; set; }
        /// <summary>
        ///     Gets or sets the folder.
        /// </summary>
        /// <value>
        ///     The folder.
        /// </value>
        public Folder Folder { get; set; }
        /// <summary>
        ///     Gets or sets the warehouse.
        /// </summary>
        public Inventory Inventory { get; set; }
        /// <summary>
        ///     Gets or sets the payment methods.
        /// </summary>
        public List<PaymentMethodCarrier> PaymentMethods { get; set; }
        /// <summary>
        ///     Gets or sets the price list.
        /// </summary>
        public PriceList PriceList { get; set; }
        /// <summary>
        ///     Gets or sets the web site.
        /// </summary>
        /// <value>
        ///     The web site.
        /// </value>
        public Website Website { get; set; }

        /// <summary>
        ///   Gets or sets the channel
        /// </summary>
        public Channel Channel { get; set; }

        ///// <summary>
        ///// Get or set the taxclasses
        ///// </summary>
        //public List<TaxClass> TaxClasses { get; set; }

        ///// <summary>
        ///// Get or set the countries
        ///// </summary>
        //public List<Country> Countries { get; set; }

        ///// <summary>
        ///// Get or set the markets
        ///// </summary>
        public Market Market { get; set; }
    }
}
