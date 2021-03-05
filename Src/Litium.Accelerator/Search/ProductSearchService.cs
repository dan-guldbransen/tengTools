using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Brand;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search;
using Litium.Framework.Search;
using Litium.Products;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Customers.TargetGroups;
using Litium.Web.Customers.TargetGroups.Events;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Search
{
    [Service(ServiceType = typeof(ProductSearchService), Lifetime = DependencyLifetime.Scoped)]
    public abstract class ProductSearchService
    {
        public abstract SearchResponse Search(
            SearchQuery searchQuery,
            IDictionary<string, ISet<string>> tags = null,
            bool addPriceFilterTags = false,
            bool addNewsFilterTags = false,
            bool addCategoryFilterTags = false);

        public abstract SearchResult Transform(
            SearchQuery searchQuery,
            SearchResponse searchResponse);

        public abstract List<TagTerms> GetTagTerms(
            SearchQuery searchQuery,
            IEnumerable<string> tagNames);
    }

    internal class ProductSearchServiceImpl : ProductSearchService
    {
        private readonly BaseProductService _baseProductService;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly ProductModelBuilder _productModelBuilder;
        private readonly VariantService _variantService;
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly SearchService _searchService;
        private readonly SearchQueryBuilderFactory _searchQueryBuilderFactory;
        private readonly CategoryService _categoryService;

        public ProductSearchServiceImpl(
            SearchService searchService,
            ProductModelBuilder productModelBuilder,
            BaseProductService baseProductService,
            VariantService variantService,
            FieldDefinitionService fieldDefinitionService,
            UrlService urlService,
            RequestModelAccessor requestModelAccessor,
            SearchQueryBuilderFactory searchQueryBuilderFactory,
            CategoryService categoryService)
        {
            _productModelBuilder = productModelBuilder;
            _baseProductService = baseProductService;
            _variantService = variantService;
            _fieldDefinitionService = fieldDefinitionService;
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _searchService = searchService;
            _searchQueryBuilderFactory = searchQueryBuilderFactory;
            _categoryService = categoryService;
        }

        public override SearchResponse Search(SearchQuery searchQuery,
            IDictionary<string, ISet<string>> tags = null,
            bool addPriceFilterTags = false,
            bool addNewsFilterTags = false,
            bool addCategoryFilterTags = false)
        {
            var pageModel = _requestModelAccessor.RequestModel.CurrentPageModel;
            var isBrandPageType = pageModel.IsBrandPageType();

            if (string.IsNullOrEmpty(searchQuery.Text) && searchQuery.CategorySystemId.GetValueOrDefault() == Guid.Empty && searchQuery.ProductListSystemId == null && !isBrandPageType)
            {
                return null;
            }

            var searchQueryBuilder = _searchQueryBuilderFactory.Create(CultureInfo.CurrentCulture, ProductCatalogSearchDomains.Products, searchQuery);
            if (isBrandPageType)
            {
                if (tags != null)
                {
                    if (!tags.ContainsKey(BrandListViewModel.TagName))
                    {
                        tags.Add(BrandListViewModel.TagName, new SortedSet<string>(new[] { pageModel.Page.Localizations.CurrentCulture.Name ?? string.Empty }));
                    }
                }
                else
                {
                    tags = new Dictionary<string, ISet<string>> { { BrandListViewModel.TagName, new SortedSet<string>(new[] { pageModel.Page.Localizations.CurrentCulture.Name ?? string.Empty }) } };
                }
            }

            searchQueryBuilder.ApplySelectedFilterTags(tags);

            if (addCategoryFilterTags)
            {
                searchQueryBuilder.ApplySelectedFilterCategories();
            }

            if (addPriceFilterTags)
            {
                searchQueryBuilder.ApplyPriceFilterTags();
            }

            if (addNewsFilterTags)
            {
                searchQueryBuilder.ApplyNewsFilterTags();
            }

            searchQueryBuilder.AddFilterReadTags();
            searchQueryBuilder.ApplyProductCatalogDefaultSearchTag();
            searchQueryBuilder.ApplyOrganizationSearchTags();

            if (searchQuery.ProductListSystemId == null)
            {
                searchQueryBuilder.ApplyCategoryTags();
                searchQueryBuilder.ApplyCategorySorting();
                searchQueryBuilder.ApplyDefaultSortOrder();
                searchQueryBuilder.ApplyFreeTextSearchTags();
            }
            else
            {
                searchQueryBuilder.ApplyProductListSorting();
            }

            if (searchQuery.ArticleNumbers?.Any() == true)
            {
                searchQueryBuilder.ApplyArticleNumbersTags();
            }

            var request = searchQueryBuilder.Build();
            request.ReadTags.Add(VariantIndexDocumentMerger.MergeTagName);
            request.ReadTags.Add(TagNames.ArticleNumber);
            var searchResponse = _searchService.Search(request);
            var priceTagName = _requestModelAccessor.RequestModel.Cart.IncludeVAT ? "-price-incl_vat" : "-price";
            var searchResult = searchResponse.Hits
                                             .Select(hit =>
                                             {
                                                 var priceTags = hit.Tags
                                                                    .Where(x => x.Name.EndsWith(priceTagName, StringComparison.OrdinalIgnoreCase))
                                                                    .Select(x => x.OriginalValue)
                                                                    .Cast<decimal>()
                                                                    .DefaultIfEmpty(decimal.MinusOne)
                                                                    .Min();

                                                 return new SortableSearchHit(hit) { Price = priceTags };
                                             })
                                             .ToList();

            if (addPriceFilterTags && (searchQuery.ContainsPriceFilter() || searchQuery.SortBy == SearchQueryConstants.Price))
            {
                searchResult = FilterAndSortOnPrice(searchResult, searchQuery);
            }

            return new SearchResponse
            {
                ETag = searchResponse.ETag,
                Hits = new Collection<Hit>(searchResult.Cast<Hit>().ToList()),
                MaxScore = searchResult.Count == 0 ? 0 : searchResult.Max(x => x.Score),
                TotalHitCount = searchResult.Count
            };
        }

        public override SearchResult Transform(SearchQuery searchQuery, SearchResponse searchResponse)
        {
            return new SearchResult
            {
                Items = new Lazy<IEnumerable<SearchResultItem>>(() => TransformSearchResult(searchQuery, searchResponse)),
                PageSize = searchQuery.PageSize.GetValueOrDefault(100000),
                Total = searchResponse.TotalHitCount
            };
        }

        public override List<TagTerms> GetTagTerms(SearchQuery searchQuery, IEnumerable<string> tagNames)
        {
            var searchQueryBuilder = _searchQueryBuilderFactory.Create(CultureInfo.CurrentCulture, ProductCatalogSearchDomains.Products, searchQuery);
            searchQueryBuilder.ApplyProductCatalogDefaultSearchTag();

            var request = searchQueryBuilder.Build();
            return _searchService.GetTagTerms(request, tagNames.Select(x => x.GetFieldDefinitionForProducts().GetTagName()).ToList(), false);
        }

        private ProductModel CreateProductModel(SearchQuery searchQuery, IEnumerable<Variant> variants)
        {
            if (searchQuery.CategorySystemId != null && searchQuery.CategorySystemId != Guid.Empty)
            {
                var baseProduct = variants.First().GetBaseProduct();
                var category = _categoryService.Get(searchQuery.CategorySystemId.Value);
                var categoryLink = category.ProductLinks.FirstOrDefault(x => x.BaseProductSystemId == baseProduct.SystemId);
                if (categoryLink != null)
                {
                    variants = variants.Where(x => categoryLink.ActiveVariantSystemIds.Contains(x.SystemId));
                }
            }

            variants = variants
                .Where(x => x.ChannelLinks.Any(z => z.ChannelSystemId == _requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId))
                .OrderBy(x => x.SortIndex);

            if (searchQuery.Tags.Count > 0)
            {
                var order = new ConcurrentDictionary<Variant, int>();
                Variant firstVariant = null;
                foreach (var tag in searchQuery.Tags)
                {
                    var fieldDefinition = _fieldDefinitionService.Get<ProductArea>(tag.Key);
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var variant in variants)
                    {
                        if (firstVariant == null)
                        {
                            firstVariant = variant;
                        }

                        CalculateTagRelevance(order, tag, fieldDefinition, variant);
                    }
                }

                var vt = order.OrderByDescending(x => x.Value).Select(x => x.Key).FirstOrDefault() ?? firstVariant;
                return _productModelBuilder.BuildFromVariant(vt);
            }

            return _productModelBuilder.BuildFromVariant(variants.First());
        }

        private ProductModel CreateProductModel(SearchQuery searchQuery, BaseProduct baseProduct)
        {
            var variants = baseProduct.GetPublishedVariants(_requestModelAccessor.RequestModel.WebsiteModel.SystemId, _requestModelAccessor.RequestModel.ChannelModel.SystemId).ToList();

            if (searchQuery.Tags.Count > 0)
            {
                var order = new ConcurrentDictionary<Variant, int>();
                foreach (var tag in searchQuery.Tags)
                {
                    var fieldDefinition = _fieldDefinitionService.Get<ProductArea>(tag.Key);
                    foreach (var variant in variants)
                    {
                        CalculateTagRelevance(order, tag, fieldDefinition, variant);
                    }
                }

                var vt = order.OrderByDescending(x => x.Value).Select(x => x.Key).FirstOrDefault() ?? variants.FirstOrDefault();
                if (vt != null)
                {
                    return _productModelBuilder.BuildFromVariant(vt);
                }
            }

            var firstVariant = variants.FirstOrDefault();
            if (firstVariant != null)
            {
                return _productModelBuilder.BuildFromBaseProduct(baseProduct, firstVariant);
            }

            return null;
        }

        private List<ProductModel> CreateProductModels(Collection<Hit> hits, SearchQuery searchQuery)
        {
            var productModels = new List<ProductModel>();
            if (hits.Count == 0)
            {
                return productModels;
            }

            foreach (var hit in hits)
            {
                var systemId = new Guid(hit.Id);
                var baseProduct = _baseProductService.Get(systemId);

                ProductModel model;
                if (baseProduct != null)
                {
                    model = CreateProductModel(searchQuery, baseProduct);
                }
                else
                {
                    if (hit.Tags.Any(x => x.Name == VariantIndexDocumentMerger.MergeTagName))
                    {
                        model = CreateProductModel(searchQuery, hit.Tags.Where(x => x.Name == TagNames.ArticleNumber).Select(x => _variantService.Get(x.Value)));
                    }
                    else
                    {
                        var variant = _variantService.Get(systemId);
                        model = _productModelBuilder.BuildFromVariant(variant);
                    }
                }

                if (model != null)
                {
                    productModels.Add(model);
                }
            }

            return productModels;
        }

        private List<SortableSearchHit> FilterAndSortOnPrice(IEnumerable<SortableSearchHit> items, SearchQuery searchQuery)
        {
            var rQuery = items.Where(x => x.Price > decimal.MinusOne);
            // filter the items out and sort the result
            if (searchQuery.PriceRanges.Count > 0)
            {
                rQuery = rQuery.Where(x => searchQuery.PriceRanges.Any(range => x.Price >= range.Item1 && x.Price <= range.Item2));
            }

            if (searchQuery.SortBy == SearchQueryConstants.Price)
            {
                rQuery = SortDirection.Descending == searchQuery.SortDirection
                    ? rQuery.OrderByDescending(x => x.Price)
                    : rQuery.OrderBy(x => x.Price);
            }
            return rQuery.ToList();
        }

        private static void CalculateTagRelevance(ConcurrentDictionary<Variant, int> order, KeyValuePair<string, ISet<string>> tag, FieldDefinition fieldDefinition, Variant variant)
        {
            var variantValue = variant.Fields[tag.Key, CultureInfo.CurrentCulture] ?? variant.Fields[tag.Key];
            if (fieldDefinition == null || variantValue is null)
            {
                return;
            }

            if (!(variantValue is string) && variantValue is IEnumerable enumVariantValue)
            {
                foreach (var item in enumVariantValue)
                {
                    var value = GetFormattedValue(item);
                    if (tag.Value.Contains(value))
                    {
                        order.AddOrUpdate(variant, _ => 1, (_, c) => c + 1);
                    }
                }
            }
            else
            {
                var value = GetFormattedValue(variantValue);
                if (tag.Value.Contains(value))
                {
                    order.AddOrUpdate(variant, _ => 1, (_, c) => c + 1);
                }
            }

            string GetFormattedValue(object value)
            {
                switch (fieldDefinition?.FieldType)
                {
                    case SystemFieldTypeConstants.Date:
                    case SystemFieldTypeConstants.DateTime:
                        return new Tag("-", ((DateTimeOffset)value).DateTime).Value ?? value as string;
                    case SystemFieldTypeConstants.Decimal:
                    case SystemFieldTypeConstants.DecimalOption:
                        return new Tag("-", (decimal)value).Value ?? value as string;
                    case SystemFieldTypeConstants.Int:
                    case SystemFieldTypeConstants.IntOption:
                        return new Tag("-", (int)value).Value ?? value as string;
                    case SystemFieldTypeConstants.Long:
                        return new Tag("-", (long)value).Value ?? value as string;
                    case SystemFieldTypeConstants.TextOption:
                    default:
                        return value as string;
                }
            }
        }

        private IEnumerable<SearchResultItem> TransformSearchResult(SearchQuery searchQuery, SearchResponse searchResponse)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery.Text))
            {
                IoC.Resolve<TargetGroupEngine>().Process(new SearchEvent
                {
                    SearchText = searchQuery.Text,
                    TotalHits = searchResponse.TotalHitCount
                });
            }

            var result = new Collection<Hit>(searchResponse.Hits.Skip((searchQuery.PageNumber - 1) * searchQuery.PageSize.GetValueOrDefault(100000)).Take(searchQuery.PageSize.GetValueOrDefault(100000)).ToList());
            var products = CreateProductModels(result, searchQuery);

            return products
                .Select(x => new ProductSearchResult
                {
                    Item = x,
                    Id = x.SelectedVariant.SystemId,
                    Name = x.GetValue<string>(SystemFieldDefinitionConstants.Name),
                    Url = x.UseVariantUrl ? x.SelectedVariant.GetUrl(channelSystemId: _requestModelAccessor.RequestModel.ChannelModel.SystemId) : _urlService.GetUrl(x.BaseProduct)
                }).ToList();
        }

        internal class SortableSearchHit : Hit
        {
            public SortableSearchHit(Hit hit)
                : base(hit.IndexName, hit.LanguageId, hit.Id, hit.Title, hit.Score, hit.Tags)
            {
                Price = 0;
            }

            public decimal Price { get; set; }
        }
    }
}
