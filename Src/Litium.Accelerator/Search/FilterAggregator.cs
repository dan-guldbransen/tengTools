using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.WebPages;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search.Filtering;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Framework.Search;
using Litium.Globalization;
using Litium.Products;
using Litium.Runtime.DependencyInjection;
using Litium.Security;
using Litium.Studio.Extenssions;
using Litium.Web;
namespace Litium.Accelerator.Search
{
    [Service(ServiceType = typeof(FilterAggregator), Lifetime = DependencyLifetime.Scoped)]
    public abstract class FilterAggregator
    {
        private readonly CategoryService _categoryService;
        private readonly CurrencyService _currencyService;
        private readonly UrlService _urlService;
        private readonly AuthorizationService _authorizationService;

        public FilterAggregator(
            CategoryService categoryService,
            CurrencyService currencyService,
            UrlService urlService,
            AuthorizationService authorizationService)
        {
            _categoryService = categoryService;
            _currencyService = currencyService;
            _urlService = urlService;
            _authorizationService = authorizationService;
        }

        public abstract IEnumerable<GroupFilter> GetFilter(SearchQuery searchQuery, IEnumerable<string> fieldNames);

        protected static int Round(int value, bool roundUp)
        {
            var roundLevels = new[]
            {
                // param 1: price below, param 2: round to nerest
                new[] { 100, 10 },
                new[] { 1000, 100 },
                new[] { 5000, 500 },
                new[] { 10000, 1000 },
                new[] { 15000, 1500 },
                new[] { 20000, 2000 },
                new[] { 25000, 2500 },
                new[] { 50000, 5000 },
                new[] { 100000, 10000 },
                new[] { 150000, 15000 },
                new[] { 500000, 50000 },
                new[] { 1000000, 100000 },
                new[] { 10000000, 1000000 },
                new[] { int.MaxValue, 10000000 }
            };
            foreach (var item in roundLevels)
            {
                if (value <= item[0])
                {
                    if (roundUp)
                    {
                        return value + item[1] - (value % item[1]);
                    }

                    return value - (value % item[1]);
                }
            }

            throw new ArgumentException("No matching roundings for actual value.", nameof(value));
        }

        private static string FindValue(FieldDefinition fieldDefinition, string term, CultureInfo cultureInfo)
        {
            if (fieldDefinition.FieldType == SystemFieldTypeConstants.TextOption)
            {
                var option = fieldDefinition.Option as TextOption;

                return option?.Items.FirstOrDefault(x => x.Name.TryGetValue(cultureInfo.Name, out string value)
                    && term.Equals(value, StringComparison.OrdinalIgnoreCase))?.Value;
            }

            return term;
        }

        private static string FormatPrice(Currency currency, int value)
        {
            return currency.Format(value, false, CultureInfo.CurrentUICulture);
        }

        protected GroupFilter GetFilterTag(SearchQuery searchQuery, FieldDefinition field, Dictionary<string, int> tagValues)
        {
            if (!searchQuery.Tags.TryGetValue(field.Id, out ISet<string> selectedValues))
            {
                selectedValues = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            return new GroupFilter
            {
                Name = field.Localizations.CurrentCulture.Name ?? field.Id,
                Attributes = new Dictionary<string, string>
                {
                    { "value", NormalizeTag(field.Id) },
                },
                IsSelected = searchQuery.Tags.ContainsKey(field.Id),
                Links = tagValues
                    .Select(x =>
                    {
                        string key;
                        switch (field.FieldType)
                        {
                            case SystemFieldTypeConstants.Decimal:
                            case SystemFieldTypeConstants.DecimalOption:
                                {
                                    if (decimal.TryParse(x.Key, NumberStyles.Any, CultureInfo.InvariantCulture, out var decValue))
                                    {
                                        key = decValue.ToString("0.########", CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        key = x.Key;
                                    }
                                    break;
                                }
                            case SystemFieldTypeConstants.Int:
                            case SystemFieldTypeConstants.IntOption:
                                {
                                    if (int.TryParse(x.Key, NumberStyles.Any, CultureInfo.InvariantCulture, out var intValue))
                                    {
                                        key = intValue.ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        key = x.Key;
                                    }
                                    break;
                                }
                            case SystemFieldTypeConstants.Long:
                                {
                                    if (long.TryParse(x.Key, NumberStyles.Any, CultureInfo.InvariantCulture, out var longValue))
                                    {
                                        key = longValue.ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        key = x.Key.TrimStart('0');
                                    }
                                    break;
                                }
                            case SystemFieldTypeConstants.Date:
                            case SystemFieldTypeConstants.DateTime:
                                {
                                    if (long.TryParse(x.Key, NumberStyles.Any, CultureInfo.InvariantCulture, out long l))
                                    {
                                        key = new DateTime(l).ToShortDateString();
                                    }
                                    else
                                    {
                                        goto default;
                                    }
                                    break;
                                }
                            case SystemFieldTypeConstants.Boolean:
                                {
                                    key = string.Format(@"accelerator.systembooleanfield.{0}",x.Key.ToLower()).AsWebSiteString();
                                    break;
                                }
                            default:
                                {
                                    key = x.Key;
                                    break;
                                }
                        }

                        var selected = selectedValues.Contains(x.Key);

                        return new FilterItem
                        {
                            Name = key,
                            IsSelected = selected,
                            Url = searchQuery.GetUrlTag(field.Id, x.Key, selected),
                            Count = x.Value,
                            Attributes = new Dictionary<string, string>
                            {
                                { "value", NormalizeTag(x.Key) },
                                { "cssValue", (FindValue(field, key, CultureInfo.CurrentCulture) ?? key)?.ToLowerInvariant() },
                            }
                        };
                    })
                    .OrderByDescending(x => x.Count)
                    .ThenBy(x => x.Name)
                    .ToList()
            };
        }

        protected GroupFilter GetNewsTag(SearchQuery searchQuery)
        {
            if (searchQuery.ProductListSystemId != null && searchQuery.ProductListSystemId == Guid.Empty)
            {
                return null;
            }

            var dteValue1 = new Tuple<DateTime, DateTime>(DateTime.Today.AddMonths(-1), DateTime.Today);
            var dteValue3 = new Tuple<DateTime, DateTime>(DateTime.Today.AddMonths(-3), DateTime.Today);

            return new GroupFilter
            {
                Attributes = new Dictionary<string, string>
                {
                    { "value", "news" },
                },
                Name = "filter.newsheadline".AsWebSiteString(),
                SingleSelect = true,
                Links = new List<FilterItem>(new[]
                {
                    new FilterItem
                    {
                        Name = "filter.newslastmonth".AsWebSiteString(),
                        IsSelected = Equals(dteValue1, searchQuery.NewsDate),
                        Url = searchQuery.GetUrlNews(dteValue1, Equals(dteValue1, searchQuery.NewsDate)),
                        Attributes = new Dictionary<string, string>
                        {
                            { "value", string.Format("{0:yyyyMMdd}-{1:yyyyMMdd}", dteValue1.Item1, dteValue1.Item2) }
                        }
                    },
                    new FilterItem
                    {
                        Name = "filter.newslast3month".AsWebSiteString(),
                        IsSelected = Equals(dteValue3, searchQuery.NewsDate),
                        Url = searchQuery.GetUrlNews(dteValue3, Equals(dteValue3, searchQuery.NewsDate)),
                        Attributes = new Dictionary<string, string>
                        {
                            { "value", string.Format("{0:yyyyMMdd}-{1:yyyyMMdd}", dteValue3.Item1, dteValue3.Item2) }
                        }
                    }
                })
            };
        }

        protected IEnumerable<(int LastPrice, int MaxSlotPrice, int Count)> GetPriceGroups(Dictionary<decimal, int> priceHits, int minPrice, int maxPrice)
        {
            const decimal slots = 7;

            var roundedMaxPrice = Round(maxPrice, true);
            var priceInEachInterval = (int)Math.Floor(roundedMaxPrice / slots);

            var result = new List<(int, int, int)>();
            var lastPrice = Round(minPrice, false);
            for (var i = 1; lastPrice < maxPrice; i++)
            {
                var i1 = i * priceInEachInterval;
                var maxSlotPrice = Round(i1, true);
                var items = priceHits.Where(x => x.Key >= lastPrice && x.Key <= maxSlotPrice).Sum(x => x.Value);

                //Rounding off can make lastPrice and maxSlotPrice have the same value 
                if (items > 0 && lastPrice < maxSlotPrice)
                {
                    result.Add((lastPrice, maxSlotPrice, items));
                }

                lastPrice = maxSlotPrice;
            }

            return result;
        }

        protected GroupFilter GetPriceTag(SearchQuery searchQuery, IList<(int LastPrice, int MaxSlotPrice, int Count)> priceHits, bool includeChild, Guid currencySystemId)
        {
            if (priceHits.Count > 0)
            {
                var currency = _currencyService.Get(currencySystemId);

                return new GroupFilter
                {
                    Name = "filter.price".AsWebSiteString(),
                    Attributes = new Dictionary<string, string>
                    {
                        { "value", "price_range" }
                    },
                    IsSelected = searchQuery.ContainsPriceFilter(),
                    Links = includeChild
                        ? priceHits
                            .Select(x =>
                            {
                                var extraInfo = x.Count.ToString(CultureInfo.CurrentCulture.NumberFormat);
                                var currentPriceRange = (x.LastPrice, x.MaxSlotPrice);
                                var selected = searchQuery.PriceRanges.Contains(currentPriceRange);
                                return new FilterItem
                                {
                                    Name = string.Format("{0}-{1}", FormatPrice(currency, x.LastPrice), FormatPrice(currency, x.MaxSlotPrice)),
                                    Count = int.Parse(x.Count.ToString(CultureInfo.CurrentCulture.NumberFormat)),
                                    IsSelected = selected,
                                    Url = searchQuery.GetUrlPrice(currentPriceRange, selected),
                                    Attributes = new Dictionary<string, string>
                                    {
                                        { "value", string.Format("{0}-{1}", x.LastPrice, x.MaxSlotPrice) },
                                    }
                                };
                            })
                            .ToList()
                        : null
                };
            }

            return null;
        }

        protected GroupFilter GetProductCategoryTag(SearchQuery searchQuery, Dictionary<Guid, int> tags)
        {
            return new GroupFilter
            {
                Attributes = new Dictionary<string, string>
                {
                    { "value", "category" }
                },
                Name = "filter.productcategories".AsWebSiteString(),
                Links = _categoryService
                    .Get(tags.Select(x => x.Key))
                    .Where(x => _authorizationService.HasOperation<Category>(Operations.Entity.Read, x.SystemId))
                    .Select(x => new { Category = x, Count = tags[x.SystemId] })
                    .Where(x => !string.IsNullOrEmpty(_urlService.GetUrl(x.Category)))
                    .Select(x =>
                    {
                        var isSelected = searchQuery.Category.Contains(x.Category.SystemId);

                        return new FilterItem
                        {
                            Name = x.Category.Fields.GetValue<string>(SystemFieldDefinitionConstants.Name, CultureInfo.CurrentCulture),
                            IsSelected = isSelected,
                            Url = searchQuery.GetUrlCategory(x.Category.SystemId, isSelected),
                            Count = x.Count,
                            Attributes = new Dictionary<string, string>
                            {
                                { "value", x.Category.SystemId.ToString("N") }
                            }
                        };
                    })
                    .OrderByDescending(x => x.Count)
                    .ThenBy(x => x.Name)
                    .ToList()
            };
        }

        protected virtual string NormalizeTag(string text)
            => text.ToLowerInvariant();
    }

    internal class FilterAggregatorImpl : FilterAggregator
    {
        private readonly ProductSearchService _productSearchService;
        private readonly RequestModelAccessor _requestModelAccessor;

        public FilterAggregatorImpl(
            CategoryService categoryService,
            CurrencyService currencyService,
            UrlService urlService,
            ProductSearchService productSearchService,
            RequestModelAccessor requestModelAccessor,
            AuthorizationService authorizationService)
            : base(categoryService, currencyService, urlService, authorizationService)
        {
            _productSearchService = productSearchService;
            _requestModelAccessor = requestModelAccessor;
        }

        public override IEnumerable<GroupFilter> GetFilter(SearchQuery searchQuery, IEnumerable<string> fieldNames)
        {
            var filterFullSearch = _productSearchService.Search(searchQuery, null, false, false, false);
            var filterTagTerms = filterFullSearch.Hits
                                             .SelectMany(x => x.Tags.Distinct(TagComparer.Default))
                                             .ToLookup(x => x.Name, StringComparer.OrdinalIgnoreCase)
                                             .ToDictionary(x => x.Key, x => x
                                                 .ToLookup(z => z.Value, StringComparer.OrdinalIgnoreCase)
                                                 .ToDictionary(z => z.Key, z => z.Count(), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

            foreach (var fieldName in fieldNames)
            {
                if (fieldName.Equals(FilteringConstants.FilterPrice, StringComparison.OrdinalIgnoreCase))
                {
                    var fullSearchResult = filterFullSearch
                       .Hits
                       .Cast<ProductSearchServiceImpl.SortableSearchHit>()
                       .Select(x => x.Price)
                       .ToLookup(x => x)
                       .ToDictionary(x => x.Key, x => x.Count());

                    var currentTagSearchResult = searchQuery.ContainsFilter()
                        ? _productSearchService
                                .Search(searchQuery, searchQuery.Tags.ToDictionary(x => x.Key, x => x.Value), false, true, true)
                                .Hits
                                .Cast<ProductSearchServiceImpl.SortableSearchHit>()
                                .Select(x => x.Price)
                                .ToLookup(x => x)
                                .ToDictionary(x => x.Key, x => x.Count())
                        : fullSearchResult;

                    var keys = fullSearchResult.Keys.Where(x => x > decimal.MinusOne).ToArray();
                    var minPrice = keys.Length > 0 ? (int)Math.Abs(keys.Min()) : 0;
                    var maxPrice = keys.Length > 0 ? (int)Math.Floor(keys.Max()) : 0;

                    var priceHits = GetPriceGroups(currentTagSearchResult, minPrice, maxPrice).ToList();
                    var filterGroup = GetPriceTag(searchQuery, priceHits, true, _requestModelAccessor.RequestModel.CountryModel.Country.CurrencySystemId);
                    if (filterGroup != null)
                    {
                        yield return filterGroup;
                    }
                }
                else if (fieldName.Equals(FilteringConstants.FilterNews, StringComparison.OrdinalIgnoreCase))
                {
                    var filterGroup = GetNewsTag(searchQuery);
                    if (filterGroup != null)
                    {
                        yield return filterGroup;
                    }
                }
                else if (fieldName.Equals(FilteringConstants.FilterProductCategories, StringComparison.OrdinalIgnoreCase))
                {
                    if (filterTagTerms.TryGetValue(TagNames.CategorySystemId, out Dictionary<string, int> tags))
                    {
                        var filterGroup = GetProductCategoryTag(searchQuery, tags.ToDictionary(x => new Guid(x.Key), x => x.Value));
                        if (filterGroup != null)
                        {
                            yield return filterGroup;
                        }
                    }
                }
                else
                {
                    var fieldDefinition = fieldName.GetFieldDefinitionForProducts();
                    if (fieldDefinition == null)
                    {
                        continue;
                    }

                    var tagName = fieldDefinition.GetTagName(CultureInfo.CurrentCulture);
                    if (filterTagTerms.TryGetValue(tagName, out Dictionary<string, int> tagValues) && tagValues.Count > 0)
                    {
                        var currentTagValues = tagValues;
                        var containsFilter = searchQuery.ContainsFilter();
                        if (containsFilter)
                        {
                            var currentResult = _productSearchService
                                .Search(searchQuery, searchQuery.Tags.Where(x => !x.Key.Equals(fieldName, StringComparison.OrdinalIgnoreCase)).ToDictionary(x => x.Key, x => x.Value), true, true, true)
                                .Hits
                                .SelectMany(x => x.Tags.Distinct(TagComparer.Default))
                                .Where(x => x.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                                .Select(x => x.Value)
                                .ToLookup(x => x, StringComparer.OrdinalIgnoreCase)
                                .ToDictionary(x => x.Key, x => x.Count(), StringComparer.OrdinalIgnoreCase);

                            currentTagValues = tagValues.ToDictionary(x => x.Key, x => currentResult.TryGetValue(x.Key, out var value) ? value : 0);
                        }

                        var filterGroup = GetFilterTag(searchQuery, fieldDefinition, currentTagValues);
                        if (filterGroup != null)
                        {
                            yield return filterGroup;
                        }
                    }
                }
            }
        }

        private class TagComparer : IEqualityComparer<Tag>
        {
            public static readonly TagComparer Default = new TagComparer();

            public bool Equals(Tag x, Tag y)
            {
                return x.Name == y.Name && Equals(x.OriginalValue, y.OriginalValue);
            }

            public int GetHashCode(Tag obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
