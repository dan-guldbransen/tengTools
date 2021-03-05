using System;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search.Providers;
using Litium.Framework.Search.Indexing;

namespace Litium.Accelerator.Search.Indexing
{
    internal class CustomerSpecificAssortmentIndexingProviderPreProcessor : IIndexingProviderPreProcessor
    {
        private readonly string _organizationPointer;

        public CustomerSpecificAssortmentIndexingProviderPreProcessor()
        {
            _organizationPointer = Foundation.Search.Constants.TagNames.GetTagNameForProperty(ProductFieldNameConstants.OrganizationsPointer);
        }

        public IndexDocument PreProcessDocument(IndexDocument document, string indexName)
        {
            if (indexName == ProductCatalogSearchDomains.Categories || indexName == ProductCatalogSearchDomains.Products)
            {
                if (document.Tags.All(x => x.Name != _organizationPointer))
                {
                    document.Tags.Add(new DocumentTag(_organizationPointer, Guid.Empty));
                }
            }

            return document;
        }
    }
}
