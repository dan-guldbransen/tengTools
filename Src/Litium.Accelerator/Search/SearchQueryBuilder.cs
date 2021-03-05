using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search.Filtering;
using Litium.Accelerator.Utilities;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search;
using Litium.Framework.Search;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Globalization;

namespace Litium.Accelerator.Search
{
    public class SearchQueryBuilder
    {
        private SearchQuery _searchQuery;
        private QueryRequest _request;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PriceFilterService _priceFilterService;
        private readonly PersonStorage _personStorage;
        private static readonly string _organizationPointer = Foundation.Search.Constants.TagNames.GetTagNameForProperty(ProductFieldNameConstants.OrganizationsPointer);

        public SearchQueryBuilder(
            RequestModelAccessor requestModelAccessor,
            PriceFilterService priceFilterService,
            PersonStorage personStorage)
        {
            _requestModelAccessor = requestModelAccessor;
            _priceFilterService = priceFilterService;
            _personStorage = personStorage;
        }

        public void Init(CultureInfo culture, string searchType, SearchQuery searchQuery)
        {
            _searchQuery = searchQuery;
            _request = new QueryRequest(culture, searchType);
        }

        public QueryRequest Build(int? pageSize = null, int pageNumber = 1)
        {
            _request.Paging = new Paging(pageNumber, pageSize.GetValueOrDefault(100000));
            return _request;
        }

        public void AddFilterReadTags()
        {
            _priceFilterService.AddFilterReadTags(_request, _searchQuery.CategorySystemId ?? Guid.Empty);
        }

        public void ApplyArticleNumbersTags()
        {
            if (_searchQuery.ArticleNumbers?.Any() == true)
            {
                var tag = new OptionalTagClause();
                foreach (var articleNumber in _searchQuery.ArticleNumbers)
                {
                    tag.Tags.Add(new Tag(TagNames.VariantArticleNumbers, articleNumber));
                }

                _request.FilterTags.Add(tag);
            }
        }

        public void ApplyCategorySorting()
        {
            switch (_searchQuery.SortBy)
            {
                case SearchQueryConstants.Price:
                    // dont need any special sortings from the searchindex
                    break;

                case SearchQueryConstants.Name:
                    // dont need any special sortings from the searchindex
                    break;

                case SearchQueryConstants.News:
                    var tagName = "News".GetFieldDefinitionForProducts()?.GetTagName(CultureInfo.CurrentCulture);
                    if (tagName != null)
                    {
                        _request.Sortings.Add(new Sorting(tagName, SortDirection.Descending, SortingFieldType.Date));
                    }
                    break;

                case SearchQueryConstants.Popular:
                    _request.Sortings.Add(new Sorting(FilteringConstants.GetMostSoldTagName(_requestModelAccessor.RequestModel.WebsiteModel.SystemId), SortDirection.Descending, SortingFieldType.Float));
                    break;

                case SearchQueryConstants.Recommended:
                    if (_searchQuery.CategorySystemId != null)
                    {
                        _request.Sortings.Add(new Sorting(TagNames.GetTagNameForCategorySortIndex(_searchQuery.CategorySystemId.Value), SortDirection.Ascending, SortingFieldType.Int));
                    }
                    break;
                default:
                    {
                        if (!string.IsNullOrWhiteSpace(_searchQuery.Text) || _requestModelAccessor.RequestModel.CurrentPageModel.IsSearchResultPageType())
                        {
                            // always sort products by their score, if no free-text is entered the score will be the same for all the products
                            _request.Sortings.Add(new Sorting(string.Empty, SortingFieldType.Score));
                        }
                        else
                        {
                            if (_searchQuery.Type == SearchType.Products)
                            {
                                goto case SearchQueryConstants.Popular;
                            }
                            if (_searchQuery.Type == SearchType.Category)
                            {
                                goto case SearchQueryConstants.Recommended;
                            }
                        }
                        goto case SearchQueryConstants.Name;
                    }
            }

            // default sorting is to always sort products after their name, article number
            _request.Sortings.Add(new Sorting(TagNames.Name, _searchQuery.SortDirection, SortingFieldType.String));
            _request.Sortings.Add(new Sorting(TagNames.ArticleNumber, SortingFieldType.String));
        }

        public void ApplyCategoryTags()
        {
            if (_searchQuery.CategorySystemId != null)
            {
                _request.FilterTags.Add(new Tag(_searchQuery.CategoryShowRecursively
                    ? TagNames.GetChannelTagNameForCategoryTree(_requestModelAccessor.RequestModel?.ChannelModel?.Channel?.SystemId ?? Guid.Empty)
                    : TagNames.CategorySystemId, _searchQuery.CategorySystemId.Value));
            }
        }

        public void ApplyDefaultSortOrder(params string[] sortColumns)
        {
            _request.Sortings.Add(new Sorting(string.Empty, SortingFieldType.Score));
            if (sortColumns != null)
            {
                foreach (var item in sortColumns)
                {
                    var tagName = item.GetFieldDefinitionForProducts()?.GetTagName(CultureInfo.CurrentCulture);
                    _request.Sortings.Add(new Sorting(tagName + "-Sortable", SortingFieldType.String));
                }
            }

            _request.Sortings.Add(new Sorting(Foundation.Search.Constants.TagNames.Title, SortingFieldType.String));
        }

        public void ApplyFreeTextSearchTags()
        {
            var text = _searchQuery.Text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var textTermClause = new MandatoryTagClause();
            foreach (var term in FilterTerms(SearchStringHelper.ExtractTerms(text)))
            {
                var textClause = new OptionalTagClause();

                textClause.Tags.Add(new Tag(Foundation.Search.Constants.TagNames.Body, term) { AllowFuzzy = true });
                textClause.Tags.Add(new Tag(Foundation.Search.Constants.TagNames.Body, term + "*"));
                textClause.Tags.Add(new Tag(Foundation.Search.Constants.TagNames.Body, "*" + term));
                textClause.Tags.Add(new Tag(Foundation.Search.Constants.TagNames.Body, term));

                if (textClause.TagsExist)
                {
                    textTermClause.Tags.Add(textClause);
                }
            }

            if (textTermClause.TagsExist)
            {
                _request.QueryTags.Add(textTermClause);
            }
        }

        public void ApplyNewsFilterTags()
        {
            if (_searchQuery.NewsDate != null)
            {
                var cultureInfo = new Guid(_request.LanguageId).GetLanguage()?.CultureInfo;
                var tagName = "News".GetFieldDefinitionForProducts()?.GetTagName(cultureInfo);
                _request.FilterTags.Add(new RangeTag(tagName, _searchQuery.NewsDate.Item1, _searchQuery.NewsDate.Item2));
            }
        }

        public void ApplyPriceFilterTags()
        {
            _priceFilterService.AddPriceFilterTags(_searchQuery, _request);
        }

        public void ApplyProductCatalogDefaultSearchTag()
        {
            _request.FilterTags.Add(new Tag(TagNames.ActiveChannelSystemId, _requestModelAccessor.RequestModel?.ChannelModel?.SystemId ?? Guid.Empty));
            _request.FilterTags.Add(new Tag(TagNames.AssortmentSystemId, _requestModelAccessor.RequestModel?.ChannelModel?.Channel?.MarketSystemId?.MapTo<MarketModel>()?.Market.AssortmentSystemId ?? Guid.Empty));

            if (_searchQuery.ProductListSystemId != null)
            {
                _request.FilterTags.Add(new Tag(TagNames.ProductListSystemId, _searchQuery.ProductListSystemId.Value) { AllowFuzzy = false });
            }
        }

        public void ApplyOrganizationSearchTags()
        {
            if (_personStorage.CurrentSelectedOrganization != null)
            {
                var tagClause = new OptionalTagClause();
                tagClause.Tags.Add(new Tag(_organizationPointer, Guid.Empty));
                tagClause.Tags.Add(new Tag(_organizationPointer, _personStorage.CurrentSelectedOrganization.SystemId));

                _request.FilterTags.Add(tagClause);
            }
            else
            {
                _request.FilterTags.Add(new Tag(_organizationPointer, Guid.Empty));
            }
        }

        public void ApplyProductListSorting()
        {
            switch (_searchQuery.SortBy)
            {
                case SearchQueryConstants.Price:
                    _request.Sortings.Add(new Sorting(TagNames.ArticleNumber, SortingFieldType.String));
                    break;
                case SearchQueryConstants.Name:
                    _request.Sortings.Add(new Sorting(TagNames.Name, _searchQuery.SortDirection, SortingFieldType.String));
                    _request.Sortings.Add(new Sorting(TagNames.ArticleNumber, SortingFieldType.String));
                    break;
                default:
                    _request.Sortings.Add(new Sorting(TagNames.GetTagNameForProductListSortIndex(_searchQuery.ProductListSystemId.GetValueOrDefault()), _searchQuery.SortDirection, SortingFieldType.Int));
                    break;
            }
        }

        public void ApplySelectedFilterCategories()
        {
            if (_searchQuery.Category.Count > 0)
            {
                var op = new OptionalTagClause();
                foreach (var item in _searchQuery.Category)
                {
                    op.Tags.Add(new Tag(TagNames.CategorySystemId, item));
                }

                _request.FilterTags.Add(op);
            }
        }

        public void ApplySelectedFilterTags()
        {
            ApplySelectedFilterTags(_searchQuery.Tags);
        }

        public void ApplySelectedFilterTags(IDictionary<string, ISet<string>> tags)
        {
            if (tags != null)
            {
                var cultureInfo = new Guid(_request.LanguageId).GetLanguage()?.CultureInfo;
                foreach (var tag in tags.Where(x => x.Value.Count > 0))
                {
                    var tagName = tag.Key.GetFieldDefinitionForProducts()?.GetTagName(cultureInfo) ?? tag.Key;
                    if (tag.Value.Count > 1)
                    {
                        var tagClause = new OptionalTagClause();
                        foreach (var tagValue in tag.Value)
                        {
                            var value = string.Concat("\"", tagValue, "\"");
                            tagClause.Tags.Add(new Tag(tagName, value.ToLower(CultureInfo.CurrentCulture)) { Analyzer = "keyword" });
                        }

                        _request.FilterTags.Add(tagClause);
                    }
                    else
                    {
                        _request.FilterTags.Add(new Tag(tagName, string.Concat("\"", tag.Value.First().ToLower(CultureInfo.CurrentCulture), "\"")) { Analyzer = "keyword" });
                    }
                }
            }
        }

        public static bool IsSortedBy(SearchQuery searchQuery, string field, SortDirection direction)
        {
            if (direction == SortDirection.Descending)
            {
                return SortDirection.Descending == searchQuery.SortDirection && field.Equals(searchQuery.SortBy, StringComparison.OrdinalIgnoreCase);
            }

            return SortDirection.Ascending == searchQuery.SortDirection && field.Equals(searchQuery.SortBy, StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> FilterTerms(IEnumerable<string> terms)
        {
            return terms.Except(new[] { "&" });
        }
    }
}
