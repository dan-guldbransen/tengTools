using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Litium.Accelerator.Services;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search;
using Litium.Framework.Search.Indexing;
using Litium.Products;

namespace Litium.Accelerator.Search
{
    internal class VariantIndexDocumentMerger : IIndexDocumentMerger<Variant>
    {
        public const string MergeTagName = nameof(VariantIndexDocumentMerger);
        private readonly TemplateSettingService _templateSettingService;

        public VariantIndexDocumentMerger(TemplateSettingService templateSettingService)
        {
            _templateSettingService = templateSettingService;
        }

        public IEnumerable<IndexDocument> Merge(IEnumerable<IndexDocument> indexDocuments)
        {
            var documents = indexDocuments as IList<IndexDocument> ?? indexDocuments.ToList();
            var fieldTemplateId = documents.FirstOrDefault()?.Tags.Where(x => x.Name == TagNames.TemplateId).Select(x => (string)x.OriginalValue).FirstOrDefault();
            if (string.IsNullOrEmpty(fieldTemplateId))
            {
                return documents;
            }
            
            var field = _templateSettingService.GetTemplateGroupingField(fieldTemplateId);
            if (string.IsNullOrEmpty(field))
            {
                return documents;
            }
            var tagName = Foundation.Search.Constants.TagNames.GetTagNameForProperty(field);
            return documents.GroupBy(x => x.Tags.FirstOrDefault(z => z.Name == tagName)?.Value).Select(x => new IndexDocument
            {
                Id = x.First().Id,
                Tags = new Collection<DocumentTag>(new[]{new ReadableDocumentTag(MergeTagName, field) }.Concat(x.SelectMany(z => z.Tags)).ToList())
            });
        }
    }
}
