using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Web;
using AutoMapper;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Framework.Search;
using Litium.Runtime.AutoMapper;
using Litium.Web.Products.Routing;
using Litium.Web.Routing;

namespace Litium.Accelerator.Mvc.Routing
{
    internal class SearchQueryMapper : IAutoMapperConfiguration
    {
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<HttpContextBase, SearchQuery>()
                .ConvertUsing<SearchQueryResolver>();
            cfg.CreateMap<HttpRequestMessage, SearchQuery>()
                .ConvertUsing<SearchQueryResolver>();
        }

        private class SearchQueryResolver : ITypeConverter<HttpContextBase, SearchQuery>, ITypeConverter<HttpRequestMessage, SearchQuery>
        {
            private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
            private readonly RouteRequestInfoAccessor _routeRequestInfoAccessor;
            private readonly RequestModelAccessor _requestModelAccessor;

            public SearchQueryResolver(
                RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
                RouteRequestInfoAccessor routeRequestInfoAccessor,
                RequestModelAccessor requestModelAccessor)
            {
                _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
                _routeRequestInfoAccessor = routeRequestInfoAccessor;
                _requestModelAccessor = requestModelAccessor;
            }

            public SearchQuery Convert(HttpContextBase source, SearchQuery destination, ResolutionContext context)
                => Convert(new SearchQuery());

            public SearchQuery Convert(HttpRequestMessage source, SearchQuery destination, ResolutionContext context)
                => Convert(new SearchQuery());

            private SearchQuery Convert(SearchQuery query)
            {
                var productPageData = _routeRequestInfoAccessor.RouteRequestInfo.Data as ProductPageData;
                if (productPageData?.CategorySystemId != null)
                {
                    query.CategorySystemId = productPageData.CategorySystemId;

                    if (_requestModelAccessor.RequestModel.WebsiteModel.GetNavigationType() == NavigationType.Filter)
                    {
                        query.CategoryShowRecursively = true;
                    }
                }

                var page = _requestModelAccessor.RequestModel.CurrentPageModel;
                if (page?.IsProductListPageType() == true)
                {
                    var productSet = page.GetValue<Guid?>(PageFieldNameConstants.ProductListPointer);
                    query.ProductListSystemId = productSet.GetValueOrDefault();
                }

                query.WebsiteSystemId = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo?.Channel.WebsiteSystemId.GetValueOrDefault() ?? Guid.Empty;
                var queryString = _routeRequestLookupInfoAccessor.RouteRequestLookupInfo.QueryString;
                foreach (var key in queryString.AllKeys)
                {
                    switch (key)
                    {
                        // Google Analytics www.demo.se/?utm_source=source&utm_medium=medium&utm_term=term&utm_content=content&utm_campaign=campaignname
                        case "utm_source":
                        case "utm_medium":
                        case "utm_term":
                        case "utm_content":
                        case "utm_campaign":

                        // Google AdWords
                        case "gclid":

                        // Facebook
                        case "fbclid":

                        // Common query parameters that now should go into filters
                        case "_":
                        case "UseWorkCopy":
                        case "callback":
                        case "featureClass":
                        case "style":
                        case null:
                            {
                                break;
                            }
                        case SearchQueryConstants.Type:
                            {
                                query.Type = Enum.TryParse(queryString[key], true, out SearchType typeValue)
                                    ? typeValue
                                    : SearchType.Products;
                                break;
                            }
                        case SearchQueryConstants.Text:
                            {
                                query.Text = queryString[key];
                                break;
                            }
                        case SearchQueryConstants.SortBy:
                            {
                                query.SortBy = queryString[key];
                                break;
                            }
                        case SearchQueryConstants.SortDirection:
                            {
                                query.SortDirection = Enum.TryParse(queryString[key], true, out SortDirection typeValue)
                                    ? typeValue
                                    : SortDirection.Ascending;
                                break;
                            }
                        case SearchQueryConstants.Page:
                            {
                                if (int.TryParse(queryString[key], out int pageNumber) && pageNumber > 1)
                                {
                                    query.PageNumber = pageNumber;
                                }
                                break;
                            }
                        case SearchQueryConstants.Category:
                            {
                                var values = queryString.GetValues(key);
                                if (values != null)
                                {
                                    foreach (var value in values)
                                    {
                                        if (Guid.TryParse(value, out Guid id))
                                        {
                                            query.Category.Add(id);
                                        }
                                    }
                                }

                                break;
                            }
                        case SearchQueryConstants.PriceRange:
                            {
                                foreach (var priceRange in queryString.GetValues(key))
                                {
                                    var prices = priceRange.Split('-');
                                    if (prices.Length == 2
                                        && int.TryParse(prices[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int fromPrice)
                                        && int.TryParse(prices[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int toPrice)
                                        && fromPrice > -1
                                        && fromPrice < toPrice)
                                    {
                                        query.PriceRanges.Add((fromPrice, toPrice));
                                    }
                                }

                                break;
                            }
                        case SearchQueryConstants.News:
                            {
                                var value = queryString[key].Split('-');
                                if (value.Length == 2)
                                {
                                    if (DateTime.TryParseExact(value[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime startDate)
                                        && DateTime.TryParseExact(value[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime endDate))
                                    {
                                        query.NewsDate = new Tuple<DateTime, DateTime>(startDate, endDate);
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                var k = key;
                                if (k.StartsWith("filter-", StringComparison.OrdinalIgnoreCase))
                                {
                                    k = k.Substring("filter-".Length);
                                }

                                if (!query.Tags.TryGetValue(k, out ISet<string> collection))
                                {
                                    collection = query.Tags[k] = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
                                }

                                var values = queryString.GetValues(key);
                                if (values != null)
                                {
                                    foreach (var value in values)
                                    {
                                        collection.Add(value);
                                    }
                                }

                                break;
                            }
                    }
                }

                return query;
            }
        }
    }
}
