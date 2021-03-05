using System.Globalization;
using System.Threading.Tasks;
using Litium.Data;
using Litium.Products;
using Litium.Search;
using Litium.Search.Indexing;
using Microsoft.Extensions.Localization;
using Nest;

namespace Litium.Accelerator.Search.Indexing.Products
{
    public class ProductIndexConfiguration : MultilingualIndexConfigurationBase<ProductDocument>
    {
        private readonly DataService _dataService;
        private readonly IStringLocalizer _localizer;

        public ProductIndexConfiguration(
            IndexConfigurationDependencies dependencies,
            DataService dataService,
            IStringLocalizer<IndexConfigurationActionResult> localizer)
            : base(dependencies)
        {
            _dataService = dataService;
            _localizer = localizer;
        }
        protected override CreateIndexDescriptor BuildIndexDescriptor(CultureInfo cultureInfo, CreateIndexDescriptor descriptor)
        {
            return base.BuildIndexDescriptor(cultureInfo, descriptor);
        }

        protected override TypeMappingDescriptor<ProductDocument> BuildTypeMapDescriptor(CultureInfo cultureInfo, TypeMappingDescriptor<ProductDocument> descriptor)
        {
            return base.BuildTypeMapDescriptor(cultureInfo, descriptor);
        }

        protected override Task<IndexConfigurationActionResult> QueueIndexRebuildAsync(IndexQueueService indexQueueService)
        {
            using (var query = _dataService.CreateQuery<BaseProduct>())
            {
                foreach (var systemId in query.ToSystemIdList())
                {
                    indexQueueService.Enqueue(new IndexQueueItem<ProductDocument>(systemId));
                }
            }

            return Task.FromResult(new IndexConfigurationActionResult
            {
                Message = _localizer.GetString("index.products.queued")
            });
        }
    }
}
