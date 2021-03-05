using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.Search.Filtering;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Search;
using Litium.Security;
using Litium.Web;
using Litium.Web.Models.Globalization;
using Nest;

namespace Litium.Accelerator.Searching
{
    [ServiceDecorator(typeof(FilterAggregator))]
    internal class FilterAggregatorDecorator : FilterAggregator
    {
        private readonly FilterAggregator _parent;
        private readonly SearchClientService _searchClientService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductSearchServiceDecorator.SearchQueryBuilder _searchQueryBuilder;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly Lazy<MarketModel> _marketModel;
        private readonly Lazy<Guid> _assortmentSystemId;
        private readonly Lazy<Guid> _countrySystemId;
        private readonly Lazy<SearchPriceFilterService.Container> _priceContainer;

        public FilterAggregatorDecorator(
            FilterAggregator parent,
            CategoryService categoryService,
            CurrencyService currencyService,
            UrlService urlService,
            SearchClientService searchClientService,
            RequestModelAccessor requestModelAccessor,
            ProductSearchServiceDecorator.SearchQueryBuilder searchQueryBuilder,
            FieldDefinitionService fieldDefinitionService,
            SearchPriceFilterService priceFilterService,
            AuthorizationService authorizationService)
            : base(categoryService, currencyService, urlService, authorizationService)
        {
            _parent = parent;
            _searchClientService = searchClientService;
            _requestModelAccessor = requestModelAccessor;
            _searchQueryBuilder = searchQueryBuilder;
            _fieldDefinitionService = fieldDefinitionService;
            _marketModel = new Lazy<MarketModel>(() => _requestModelAccessor.RequestModel.ChannelModel?.Channel?.MarketSystemId?.MapTo<MarketModel>());
            _assortmentSystemId = new Lazy<Guid>(() => _marketModel.Value?.Market.AssortmentSystemId ?? Guid.Empty);
            _countrySystemId = new Lazy<Guid>(() => _requestModelAccessor.RequestModel.CountryModel.SystemId);
            _priceContainer = new Lazy<SearchPriceFilterService.Container>(() => priceFilterService.GetPrices());
        }

        public override IEnumerable<GroupFilter> GetFilter(SearchQuery searchQuery, IEnumerable<string> fieldNames)
        {
            var fieldNamesList = fieldNames as IList<string> ?? fieldNames.ToList();

            if (!_searchClientService.IsConfigured)
            {
                foreach (var item in _parent.GetFilter(searchQuery, fieldNames))
                {
                    yield return item;
                }
            }
            else
            {
                if (fieldNamesList.Count > 0
                    && !fieldNamesList.All(fieldName => fieldName.Equals(FilteringConstants.FilterNews, StringComparison.OrdinalIgnoreCase)))
                {
                    var noFilterFieldNames = new HashSet<string>(new[]
                    {
                        FilteringConstants.FilterPrice,
                        FilteringConstants.FilterNews,
                        FilteringConstants.FilterProductCategories
                    }, StringComparer.OrdinalIgnoreCase);

                    var result = _searchClientService.Search<ProductDocument>(CultureInfo.CurrentCulture, descriptor => descriptor
                        .Size(0)
                        .QueryWithPermission(queryContainerDescriptor => _searchQueryBuilder.BuildQuery(queryContainerDescriptor,
                                                                                          searchQuery,
                                                                                          tags: null,
                                                                                          addPriceFilterTags: false,
                                                                                          addNewsFilterTags: false,
                                                                                          addCategoryFilterTags: false,
                                                                                          addDefaultQuery: true))
                        .Aggregations(rootAgg =>
                        {
                            var aggs = new List<AggregationContainerDescriptor<ProductDocument>>();

                            var aggregationTagNames = fieldNames
                                .Where(fieldName => !noFilterFieldNames.Contains(fieldName));

                            aggs.AddRange(BuildFieldAggregations(rootAgg, aggregationTagNames));
                            aggs.Add(BuildFieldAggregation(rootAgg, aggregationTagNames));

                            if (fieldNamesList.Any(fieldName => fieldName.Equals(FilteringConstants.FilterPrice, StringComparison.OrdinalIgnoreCase)))
                            {
                                aggs.Add(BuildPriceAggregation(rootAgg));
                            }

                            if (fieldNamesList.Any(fieldName => fieldName.Equals(FilteringConstants.FilterProductCategories, StringComparison.OrdinalIgnoreCase)))
                            {
                                aggs.Add(BuildCategoryAggregation(rootAgg));
                            }

                            return aggs.Aggregate((a, b) => a & b);
                        }));

                    foreach (var fieldName in fieldNames)
                    {
                        if (fieldName.Equals(FilteringConstants.FilterPrice, StringComparison.OrdinalIgnoreCase))
                        {
                            var filterGroup = CollectPriceFacet();
                            if (filterGroup is object)
                            {
                                yield return filterGroup;
                            }
                        }
                        else if (fieldName.Equals(FilteringConstants.FilterNews, StringComparison.OrdinalIgnoreCase))
                        {
                            var filterGroup = GetNewsTag(searchQuery);
                            if (filterGroup is object)
                            {
                                yield return filterGroup;
                            }
                        }
                        else if (fieldName.Equals(FilteringConstants.FilterProductCategories, StringComparison.OrdinalIgnoreCase))
                        {
                            var filterGroup = CollectCategoryFacet();
                            if (filterGroup is object)
                            {
                                yield return filterGroup;
                            }
                        }
                        else
                        {
                            var tag = CollectFieldFacet(fieldName);
                            if (tag is object)
                            {
                                yield return tag;
                            }
                        }
                    }

                    AggregationContainerDescriptor<ProductDocument> BuildCategoryAggregation(AggregationContainerDescriptor<ProductDocument> selector)
                    {
                        return selector
                            .Nested("$Categories", filterContainer => filterContainer
                                .Path(x => x.MainCategories)
                                .Aggregations(a => a
                                    .Filter("filter", filterSelector => filterSelector
                                        .Filter(ff => ff
                                            .Bool(bq => bq
                                                .Must(m =>
                                                {
                                                    var qc = m
                                                    .Term(t => t
                                                        .Field(x => x.MainCategories[0].AssortmentSystemId)
                                                        .Value(_assortmentSystemId.Value)
                                                    );

                                                    if (searchQuery.ContainsFilter() && (!searchQuery.ContainsCategoryFilter() || searchQuery.ContainsMultipleFilters()))
                                                    {
                                                        qc &= _searchQueryBuilder.BuildQuery(
                                                                m,
                                                                searchQuery,
                                                                tags: searchQuery.Tags,
                                                                addPriceFilterTags: true,
                                                                addNewsFilterTags: true,
                                                                addCategoryFilterTags: false,
                                                                addDefaultQuery: false);
                                                    }

                                                    return qc;
                                                })
                                            )
                                        )
                                        .Aggregations(termAgg => termAgg
                                            .Terms("tags", termSelector => termSelector
                                                .Field(x => x.MainCategories[0].AssortmentSystemId)
                                                .Aggregations(subAggregation => subAggregation
                                                    .Terms("tag", valueSelector => valueSelector
                                                        .Field(x => x.MainCategories[0].CategorySystemId)
                                                        .Size(100)
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            );
                    }

                    IEnumerable<AggregationContainerDescriptor<ProductDocument>> BuildFieldAggregations(AggregationContainerDescriptor<ProductDocument> selector, IEnumerable<string> fieldNames)
                    {
                        return fieldNames
                            .Select(fieldName =>
                            {
                                if (searchQuery.ContainsFilter(exceptTag: fieldName))
                                {
                                    return selector
                                        .Filter(fieldName, filterContainer => filterContainer
                                            .Filter(filterSelector => filterSelector
                                                .Bool(bq => bq
                                                    .Must(m => _searchQueryBuilder.BuildQuery(
                                                            m,
                                                            searchQuery,
                                                            tags: FilterFields(fieldName),
                                                            addPriceFilterTags: true,
                                                            addNewsFilterTags: true,
                                                            addCategoryFilterTags: true,
                                                            addDefaultQuery: false)
                                                    )
                                                )
                                            )
                                            .Aggregations(x => BuildFilterAggregation(x, fieldName))
                                        );
                                }

                                return BuildFilterAggregation(selector, fieldName);
                            });

                        AggregationContainerDescriptor<ProductDocument> BuildFilterAggregation(AggregationContainerDescriptor<ProductDocument> container, string fieldName)
                        {
                            return container
                                .Nested(fieldName, nestedPerField => nestedPerField
                                    .Path(x => x.Tags)
                                    .Aggregations(fieldAggregation => fieldAggregation
                                        .Filter("filter", fieldFilter => fieldFilter
                                            .Filter(filter => filter
                                                .Term(filterTerm => filterTerm
                                                    .Field(field => field.Tags[0].Key)
                                                    .Value(fieldName)
                                                )
                                            )
                                            .Aggregations(tags => tags
                                                .Terms("tags", termSelector => termSelector
                                                    .Field(field => field.Tags[0].Key)
                                                    .Aggregations(subAggregation => subAggregation
                                                        .Terms("tag", tag => tag
                                                            .Field(x => x.Tags[0].Value)
                                                            .Size(100)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                );
                        }

                        IDictionary<string, ISet<string>> FilterFields(string fieldName)
                        {
                            return searchQuery
                                .Tags
                                .Where(x => !x.Key.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                                .ToDictionary(x => x.Key, x => x.Value);
                        }
                    }

                    AggregationContainerDescriptor<ProductDocument> BuildFieldAggregation(AggregationContainerDescriptor<ProductDocument> selector, IEnumerable<string> fieldNames)
                    {
                        return selector
                            .Nested("$all-tags", filterContainer => filterContainer
                                .Path(x => x.Tags)
                                .Aggregations(a => a
                                    .Filter("filter", filterSelector => filterSelector
                                        .Filter(ff => ff
                                            .Bool(bq => bq
                                                .Must(m => m
                                                    .Terms(t => t
                                                        .Field(x => x.Tags[0].Key)
                                                        .Terms(fieldNames)
                                                    )
                                                )
                                            )
                                        )
                                        .Aggregations(termAgg => termAgg
                                            .Terms("tags", termSelector => termSelector
                                                .Field(x => x.Tags[0].Key)
                                                .Aggregations(subAggregation => subAggregation
                                                    .Terms("tag", valueSelector => valueSelector
                                                        .Field(x => x.Tags[0].Value)
                                                        .Size(100)
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            );
                    }

                    AggregationContainerDescriptor<ProductDocument> BuildPriceAggregation(AggregationContainerDescriptor<ProductDocument> selector)
                    {
                        if (searchQuery.ContainsFilter(includePriceFilter: false))
                        {
                            return selector
                                    .Filter("$Prices", filterContainer => filterContainer
                                        .Filter(filterSelector => filterSelector
                                            .Bool(bq => bq
                                                .Must(m => _searchQueryBuilder.BuildQuery(
                                                        m,
                                                        searchQuery,
                                                        tags: searchQuery.Tags,
                                                        addPriceFilterTags: false,
                                                        addNewsFilterTags: true,
                                                        addCategoryFilterTags: true,
                                                        addDefaultQuery: false)
                                                )
                                            )
                                        )
                                        .Aggregations(x => BuildPriceAggregationItem(x))
                                    );
                        }
                        else
                        {
                            return BuildPriceAggregationItem(selector);
                        };

                        AggregationContainerDescriptor<ProductDocument> BuildPriceAggregationItem(AggregationContainerDescriptor<ProductDocument> selector)
                        {
                            return selector
                                .Terms("$Prices", prices => prices
                                    .Script(script => script
                                        .Source("double r; for (item in params._source.prices) { if (params.id.contains(item.systemId) && item.countrySystemId == params.country && item.isCampaignPrice == false) { r = r == 0 ? item.price : Math.min(r, item.price)}} return r")
                                        .Params(new Dictionary<string, object> {
                                            {"id", _priceContainer.Value.PriceLists.ToArray() },
                                            {"country", _countrySystemId.Value },
                                        }))
                                    .Size(10000));
                        }
                    }

                    GroupFilter CollectCategoryFacet()
                    {
                        var categoryBucket = result.Aggregations
                               .Global("$Categories")
                               .Filter("filter")?
                               .Terms("tags")?
                               .Buckets
                               .FirstOrDefault()?
                               .Terms("tag")?
                               .Buckets
                               .Select(x => new { CanConvert = Guid.TryParse(x.Key, out var id), Key = id, x.DocCount })
                               .Where(x => x.CanConvert)
                               .ToDictionary(x => x.Key, x => unchecked((int)x.DocCount));

                        if (categoryBucket == null)
                        {
                            return null;
                        }

                        return GetProductCategoryTag(searchQuery, categoryBucket);
                    }

                    GroupFilter CollectFieldFacet(string fieldName)
                    {
                        var fieldDefinition = _fieldDefinitionService.Get<ProductArea>(fieldName);
                        if (fieldDefinition == null)
                        {
                            return null;
                        }

                        var allBuckets = result.Aggregations
                            .Global("$all-tags")
                            .Filter("filter")?
                            .Terms("tags")?
                            .Buckets
                            .FirstOrDefault(x => x.Key.Equals(fieldName, StringComparison.OrdinalIgnoreCase))?
                            .Terms("tag")?
                            .Buckets;

                        if (allBuckets == null)
                        {
                            return null;
                        }

                        var topNode = result.Aggregations.Filter(fieldName);
                        var tagBuckets = (topNode?.Nested(fieldName) ?? topNode)?
                            .Filter("filter")?
                            .Terms("tags")?
                            .Buckets
                            .FirstOrDefault()?
                            .Terms("tag")?
                            .Buckets;

                        var tagValues = new Dictionary<string, int>();
                        foreach (var item in allBuckets)
                        {
                            var current = tagBuckets?.FirstOrDefault(x => x.Key.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
                            tagValues.Add(item.Key, unchecked((int)(current?.DocCount ?? 0)));
                        }

                        return GetFilterTag(searchQuery, fieldDefinition, tagValues);
                    }

                    GroupFilter CollectPriceFacet()
                    {
                        var priceBucket = result.Aggregations?.Filter("$Prices")?.Terms("$Prices")?.Buckets ?? result.Aggregations.Terms("$Prices")?.Buckets;
                        if (priceBucket != null)
                        {
                            var priceFacets = priceBucket.ToDictionary(x => decimal.Parse(x.Key, NumberStyles.Any, CultureInfo.InvariantCulture), x => unchecked((int)x.DocCount.GetValueOrDefault()));
                            var keys = priceFacets.Keys.Where(x => x > decimal.Zero).ToArray();
                            var minPrice = keys.Length > 0 ? (int)Math.Abs(keys.Min()) : 0;
                            var maxPrice = keys.Length > 0 ? (int)Math.Floor(keys.Max()) : 0;

                            var priceHits = GetPriceGroups(priceFacets, minPrice, maxPrice).ToList();
                            return GetPriceTag(searchQuery, priceHits, true, _requestModelAccessor.RequestModel.CountryModel.Country.CurrencySystemId);
                        }
                        return null;
                    }
                }
            }
        }

        protected override string NormalizeTag(string text)
        {
            return text;
        }
    }
}
