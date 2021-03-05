using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Foundation.GUI;
using Litium.Foundation.Modules.ECommerce;
using Litium.Products;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Services
{
    internal class ProductServiceImpl : ProductService
    {
        private readonly ModuleECommerce _moduleECommerce;
        private readonly RelatedModelBuilder _relatedModelBuilder;
        private readonly RelationshipTypeService _relationshipTypeService;

        public ProductServiceImpl(RelationshipTypeService relationshipTypeService, RelatedModelBuilder relatedModelBuilder, ModuleECommerce moduleECommerce, VariantService variantService)
        {
            _relationshipTypeService = relationshipTypeService;
            _relatedModelBuilder = relatedModelBuilder;
            _moduleECommerce = moduleECommerce;
        }

        public override List<Variant> GetMostSoldProducts(Guid webSiteId, Guid channelId, string articleNumber = null, Guid[] productGroupIds = null, int count = 4, string ignoreVariantId = "")
        {
            if (productGroupIds == null)
            {
                return new List<Variant>();
            }

            return _moduleECommerce.Statistics.GetMostSoldArticles(articleNumber, webSiteId, channelId, productGroupIds, count, FoundationContext.Current.Token, ignoreVariantId)
                .GroupBy(x => x.Variant.BaseProductSystemId)
                .Select(x => x.First().Variant)
                .Take(count)
                .ToList();
        }

        public override RelatedModel GetProductRelationships(ProductModel productModel, string relationTypeName, bool includeBaseProductRelations = true, bool includeVariantRelations = true)
        {
            if (string.IsNullOrEmpty(relationTypeName))
            {
                return null;
            }
            
            var relationshipType = _relationshipTypeService.Get(relationTypeName);
            return relationshipType != null ? _relatedModelBuilder.Build(productModel, relationshipType, includeBaseProductRelations, includeVariantRelations) : null;
        }
    }
}
