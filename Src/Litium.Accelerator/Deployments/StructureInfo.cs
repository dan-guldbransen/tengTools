using System;
using System.Collections.Generic;
using Litium.Blocks;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Globalization;
using Litium.Media;
using Litium.Products;
using Litium.Websites;
using Category = Litium.Products.Category;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    /// Structire information for import / export.
    /// </summary>
    /// <remarks>Disclaimer: Class is still under development and can be changed without notification and with breaking changes.</remarks>
    [Serializable]
    public class StructureInfo
    {
        /// <summary>
        /// Gets or sets the CMS.
        /// </summary>
        /// <value>The CMS.</value>
        public WebsiteStructure Website { get; set; }

        /// <summary>
        /// Create a set of example products and product groups.
        /// </summary>
        /// <value><c>true</c> if [create example products]; otherwise, <c>false</c>.</value>
        public bool CreateExampleProducts { get; set; }

        /// <summary>
        /// Gets or sets the e commerce.
        /// </summary>
        /// <value>The e commerce.</value>
        public ECommerceStructure ECommerce { get; set; }

        /// <summary>
        /// Gets or sets the foundation.
        /// </summary>
        /// <value>The solution.</value>
        public FoundationStructure Foundation { get; set; }

        /// <summary>
        /// Gets or sets the mappings.
        /// </summary>
        /// <value>The mappings.</value>
        public Dictionary<Guid, Guid> Mappings { get; set; } = new Dictionary<Guid, Guid>();

        /// <summary>
        /// Gets or sets the media archive.
        /// </summary>
        /// <value>The media archive.</value>
        public MediaArchiveStructure MediaArchive { get; set; }

        /// <summary>
        /// Gets or sets the prefix to use for example url's.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the product catalog.
        /// </summary>
        /// <value>The product catalog.</value>
        public ProductCatalogStructure ProductCatalog { get; set; }

        /// <summary>
        /// Ids the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Guid.</returns>
        public Guid Id(Guid id)
        {
            if (Mappings.TryGetValue(id, out Guid r))
            {
                return r;
            }

            return id;
        }

        /// <summary>
        /// Class WebsiteStructure.
        /// </summary>
        [Serializable]
        public class WebsiteStructure
        {
            /// <summary>
            /// Gets or sets the assortment.
            /// </summary>
            /// <value>The assortment.</value>
            public Assortment Assortment { get; set; }

            /// <summary>
            /// Gets or sets the channel.
            /// </summary>
            /// <value>The channel.</value>
            public Channel Channel { get; set; }

            /// <summary>
            /// Gets or sets the name of the domain.
            /// </summary>
            /// <value>The name of the domain.</value>
            public DomainName DomainName { get; set; }

            /// <summary>
            /// Gets or sets the market.
            /// </summary>
            /// <value>The market.</value>
            public Market Market { get; set; }

            /// <summary>
            /// Gets or sets the countries.
            /// </summary>
            /// <value>The countries.</value>
            public List<Country> Countries { get; set; }

            /// <summary>
            /// Gets or sets the website.
            /// </summary>
            /// <value>The website.</value>
            public Website Website { get; set; }

            /// <summary>
            /// Gets or sets the child pages.
            /// </summary>
            /// <value>The child pages.</value>
            public List<Page> ChildPages  { get; set; }

            /// <summary>
            /// Gets or sets the blocks.
            /// </summary>
            /// <value>The blocks.</value>
            public List<Block> Blocks { get; set; }

            /// <summary>
            /// Gets or sets the tax classes.
            /// </summary>
            /// <value>The tax classes.</value>
            public List<TaxClass> TaxClasses { get; set; }

            /// <summary>
            /// Gets or sets the website template map.
            /// </summary>
            /// <value>The website template map.</value>
            public Dictionary<Guid, string> WebsiteTemplateMap { get; set; }

            /// <summary>
            /// Gets or sets the page template map.
            /// </summary>
            /// <value>The page template map.</value>
            public Dictionary<Guid, string> PageTemplateMap { get; set; }

            /// <summary>
            /// Gets or sets the block template map.
            /// </summary>
            /// <value>The block template map.</value>
            public Dictionary<Guid, string> BlockTemplateMap { get; set; }

            /// <summary>
            /// Gets or sets the channel template map.
            /// </summary>
            /// <value>The channel template map.</value>
            public Dictionary<Guid, string> ChannelTemplateMap { get; set; }

            /// <summary>
            /// Gets or sets the market template map.
            /// </summary>
            /// <value>The market template map.</value>
            public Dictionary<Guid, string> MarketTemplateMap { get; set; }

            /// <summary>
            /// Gets or Sets the thumbnail data for Page
            /// </summary>
            public Dictionary<Guid, byte[]> PageThumbnailData { get; set; }

            /// <summary>
            /// Gets or Sets the thumbnail data for Block </summary>
            public Dictionary<Guid, byte[]> BlockThumbnailData { get; set; }

        }

        /// <summary>
        /// Class ECommerceStructure.
        /// </summary>
        [Serializable]
        public class ECommerceStructure
        {
            /// <summary>
            /// Gets or sets the delivery method.
            /// </summary>
            /// <value>The delivery method.</value>
            public List<DeliveryMethodCarrier> DeliveryMethod { get; set; }

            /// <summary>
            /// Gets or sets the payment method.
            /// </summary>
            /// <value>The payment method.</value>
            public List<PaymentMethodCarrier> PaymentMethod { get; set; }
        }

        /// <summary>
        /// Foundation structure
        /// </summary>
        [Serializable]
        public class FoundationStructure
        {
            /// <summary>
            /// Gets or sets the currencies.
            /// </summary>
            /// <value>The currencies.</value>
            public List<Currency> Currencies { get; set; }

            /// <summary>
            /// Gets or sets the language map.
            /// </summary>
            /// <value>The language map.</value>
            public Dictionary<Guid, string> LanguageMap { get; set; }

            /// <summary>
            /// Gets or sets the relation templates.
            /// </summary>
            /// <value>The relation templates.</value>
            //public IEnumerable<Customers.FieldTemplateBase> RelationTemplates { get; set; }
            public Dictionary<Guid, string> RelationTemplates { get; set; }

            /// <summary>
            /// Gets or sets the visitor group id.
            /// </summary>
            /// <value>The visitor group id.</value>
            public Guid VisitorGroupId { get; set; }
        }

        /// <summary>
        /// Media archive structure for import / epoxt
        /// </summary>
        [Serializable]
        public class MediaArchiveStructure
        {
            /// <summary>
            /// Gets or sets the file data.
            /// </summary>
            /// <value>The file data.</value>
            public Dictionary<Guid, byte[]> FileData { get; set; }

            /// <summary>
            /// Gets or sets the files.
            /// </summary>
            /// <value>The files.</value>
            public List<File> Files { get; set; }

            /// <summary>
            /// Gets or sets the folders.
            /// </summary>
            /// <value>The folders.</value>
            public List<Folder> Folders { get; set; }

            /// <summary>
            /// Gets or sets the template id.
            /// </summary>
            /// <value>The template id.</value>
            public Guid FolderTemplateId { get; set; }
        }

        /// <summary>
        /// Product catalog structure for import / epoxt
        /// </summary>
        [Serializable]
        public class ProductCatalogStructure
        {
            /// <summary>
            /// Gets or sets the assortment.
            /// </summary>
            /// <value>The assortment.</value>
            public Assortment Assortment { get; set; }

            /// <summary>
            /// Gets or sets the variant groups.
            /// </summary>
            /// <value>The variant groups.</value>
            public List<BaseProduct> BaseProducts { get; set; }

            /// <summary>
            /// Gets or sets the categories.
            /// </summary>
            /// <value>The categories.</value>
            public List<Category> Categories { get; set; } = new List<Category>();

            /// <summary>
            /// Gets or sets the inventory.
            /// </summary>
            /// <value>The inventory.</value>
            public Inventory Inventory { get; set; }

            /// <summary>
            /// Gets or sets the inventory items.
            /// </summary>
            /// <value>The inventory items.</value>
            public List<InventoryItem> InventoryItems { get; set; }

            /// <summary>
            /// Gets or sets the price list.
            /// </summary>
            /// <value>The price list.</value>
            public PriceList PriceList { get; set; }

            /// <summary>
            /// Gets or sets the price list items.
            /// </summary>
            /// <value>The price list items.</value>
            public List<PriceListItem> PriceListItems { get; set; }

            /// <summary>
            /// Gets or sets the product lists.
            /// </summary>
            /// <value>The product lists.</value>
            public List<ProductList> ProductLists { get; set; }

            /// <summary>
            /// Gets or sets the product list items.
            /// </summary>
            /// <value>The price list items.</value>
            public List<ProductListItem> ProductListItems { get; set; }

            /// <summary>
            /// Gets or sets the product relation types map.
            /// </summary>
            /// <value>The template map.</value>
            public Dictionary<Guid, string> RelationTypeMap { get; set; }

            /// <summary>
            /// Gets or sets the template map.
            /// </summary>
            /// <value>The template map.</value>
            public Dictionary<Guid, string> TemplateMap { get; set; }

            /// <summary>
            /// Gets or sets the unit of measurements.
            /// </summary>
            /// <value>The unit of measurements.</value>
            public List<UnitOfMeasurement> UnitOfMeasurements { get; set; }

            /// <summary>
            /// Gets or sets the articles.
            /// </summary>
            /// <value>The articles.</value>
            public List<Variant> Variants { get; set; } = new List<Variant>();

            /// <summary>
            /// Gets or sets the currency identifier.
            /// </summary>
            /// <value>The currency identifier.</value>
            public string CurrencyId { get; set; }

            /// <summary>
            /// Gets or sets the product filter list
            /// </summary>
            public List<string> ProductFilters { get; set; }
        }
    }
}
