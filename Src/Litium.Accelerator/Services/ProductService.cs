using System;
using System.Collections.Generic;
using Litium.Products;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(ProductService), Lifetime = DependencyLifetime.Scoped)]
    public abstract class ProductService
    {
        public abstract List<Variant> GetMostSoldProducts(Guid webSiteId, Guid channelId, string articleNumber = null, Guid[] productGroupIds = null, int numberOfProducts = 4, string ignoreVariantId = "");
        public abstract RelatedModel GetProductRelationships(ProductModel productModel, string relationTypeName, bool includeBaseProductRelations = true, bool includeVariantRelations = true);
    }
}
