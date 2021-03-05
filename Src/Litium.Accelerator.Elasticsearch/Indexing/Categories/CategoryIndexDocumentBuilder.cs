using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Indecies;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using Litium.Products;
using Litium.Search;
using Litium.Search.Indexing;

namespace Litium.Accelerator.Search.Indexing.Categories
{
    public class CategoryIndexDocumentBuilder : MultilingualIndexDocumentBuilderBase<CategoryDocument>
    {
        private readonly ChannelService _channelService;
        private readonly LanguageService _languageService;
        private readonly CategoryService _categoryService;
        private readonly SearchPermissionService _searchPermissionService;
        private readonly ContentBuilderService _contentBuilderService;

        public CategoryIndexDocumentBuilder(
            IndexDocumentBuilderDependencies dependencies,
            ChannelService channelService,
            LanguageService languageService,
            CategoryService categoryService,
            SearchPermissionService searchPermissionService,
            ContentBuilderService contentBuilderService) : base(dependencies)
        {
            _channelService = channelService;
            _languageService = languageService;
            _categoryService = categoryService;
            _searchPermissionService = searchPermissionService;
            _contentBuilderService = contentBuilderService;
        }

        public override IEnumerable<(CultureInfo, IDocument)> BuildIndexDocuments(IndexQueueItem item)
        {
            var category = _categoryService.Get(item.SystemId);
            if (category == null)
            {
                yield break;
            }

            var cultureContentCache = new ConcurrentDictionary<CultureInfo, ISet<string>>();
            var permissions = _searchPermissionService.GetPermissions(category);

            foreach (var channelLink in category.ChannelLinks)
            {
                var channel = _channelService.Get(channelLink.ChannelSystemId);
                if (channel is null)
                {
                    // Orphaned category link exists, skip to create new index document.
                    continue;
                }

                var cultureInfo = _languageService.Get(channel.ProductLanguageSystemId.GetValueOrDefault())?.CultureInfo;
                if (cultureInfo is null)
                {
                    // Channel does not have a culture.
                    continue;
                }

                var localization = category.Localizations[cultureInfo];
                yield return (cultureInfo, new CategoryDocument
                {
                    CategorySystemId = category.SystemId,
                    Content = cultureContentCache.GetOrAdd(cultureInfo, _ => _contentBuilderService.BuildContent<CategoryFieldTemplate, ProductArea>(category.FieldTemplateSystemId, cultureInfo, category.Fields)),
                    Name = localization.Name,
                    ChannelSystemId = channel.SystemId,
                    Permissions = permissions,
                    Assortment = category.AssortmentSystemId,
                    Organizations = category.Fields.GetValue<IList<PointerItem>>(ProductFieldNameConstants.OrganizationsPointer)?.Select(x => x.EntitySystemId).ToHashSet()
                                        ?? new HashSet<Guid> { Guid.Empty }
                });
            }
        }

        public override IEnumerable<IDocument> BuildRemoveIndexDocuments(IndexQueueItem item)
        {
            yield return RemoveByFieldDocument.Create<CategoryDocument, Guid>(x => x.CategorySystemId, item.SystemId);
        }
    }
}
