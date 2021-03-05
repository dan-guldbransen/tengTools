using System.Globalization;
using System.Threading.Tasks;
using Litium.Data;
using Litium.Search;
using Litium.Search.Indexing;
using Microsoft.Extensions.Localization;
using Nest;

namespace Litium.Accelerator.Search.Indexing.Pages
{
    public class PageIndexConfiguration : MultilingualIndexConfigurationBase<PageDocument>
    {
        private readonly DataService _dataService;
        private readonly IStringLocalizer _localizer;

        public PageIndexConfiguration(
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

        protected override TypeMappingDescriptor<PageDocument> BuildTypeMapDescriptor(CultureInfo cultureInfo, TypeMappingDescriptor<PageDocument> descriptor)
        {
            return base.BuildTypeMapDescriptor(cultureInfo, descriptor)
                .Properties(p => p
                    .Text(t => t
                        .Name(x => x.Name)
                        .Similarity("BM25")
                        .Boost(10)
                        .Analyzer(cultureInfo.AsAnalyzer())
                    )
                    .Text(t => t
                        .Name(x => x.Content)
                    )
                );
        }

        protected override Task<IndexConfigurationActionResult> QueueIndexRebuildAsync(IndexQueueService indexQueueService)
        {
            using (var query = _dataService.CreateQuery<Websites.Page>())
            {
                foreach (var systemId in query.ToSystemIdList())
                {
                    indexQueueService.Enqueue(new IndexQueueItem<PageDocument>(systemId));
                }
            }

            return Task.FromResult(new IndexConfigurationActionResult
            {
                Message = _localizer.GetString("index.pages.queued")
            });
        }
    }
}
