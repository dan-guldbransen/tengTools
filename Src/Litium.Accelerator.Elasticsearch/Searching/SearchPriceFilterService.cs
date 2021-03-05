using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Account;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Actions;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Conditions;
using Litium.Products.PriceCalculator;
using Litium.Runtime.DependencyInjection;
using Litium.Security;
using Nest;

namespace Litium.Accelerator.Searching
{
    [Service(ServiceType = typeof(SearchPriceFilterService), Lifetime = DependencyLifetime.Scoped)]
    internal class SearchPriceFilterService
    {
        private readonly ICampaignHandler _campaignHandler;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly IPriceCalculator _priceCalculator;
        private readonly AccountService _accountService;
        private readonly SecurityContextService _securityContextService;

        public SearchPriceFilterService(
            ICampaignHandler campaignHandler,
            RequestModelAccessor requestModelAccessor,
            IPriceCalculator priceCalculator,
            AccountService accountService,
            SecurityContextService securityContextService)
        {
            _campaignHandler = campaignHandler;
            _requestModelAccessor = requestModelAccessor;
            _priceCalculator = priceCalculator;
            _accountService = accountService;
            _securityContextService = securityContextService;
        }

        public Container GetPrices()
        {
            return new Container(new Lazy<IEnumerable<Guid>>(GetPriceListForUser), new Lazy<IEnumerable<Guid>>(GetCampaignsForUser));
        }

        public IEnumerable<Func<QueryContainerDescriptor<ProductDocument>, QueryContainer>> GetPriceFilterTags(
            SearchQuery searchQuery,
            Container container,
            Guid countrySystemId,
            bool filterForSorting = false)
        {
            if (searchQuery.ContainsPriceFilter())
            {
                foreach (var item in container.PriceLists)
                {
                    foreach (var priceItem in searchQuery.PriceRanges)
                    {
                        yield return q => q.Nested(n => n
                                .Path(x => x.Prices)
                                .Query(nq
                                    => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                                    && nq.Term(t => t.Field(f => f.Prices[0].CountrySystemId).Value(countrySystemId))
                                    && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(false))
                                    && nq.Range(t => t.Field(f => f.Prices[0].Price)
                                    .GreaterThanOrEquals(priceItem.Item1)
                                    .LessThanOrEquals(priceItem.Item2))
                                )
                            );
                    }
                }

                foreach (var item in container.Campaigns)
                {
                    foreach (var priceItem in searchQuery.PriceRanges)
                    {
                        yield return q => q.Nested(n => n
                              .Path(x => x.Prices)
                              .Query(nq
                                  => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                                  && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(true))
                                  && nq.Range(t => t.Field(f => f.Prices[0].Price)
                                    .GreaterThanOrEquals(priceItem.Item1)
                                    .LessThanOrEquals(priceItem.Item2))
                              )
                          );
                    }
                }
            }
            else if (filterForSorting)
            {
                foreach (var item in container.PriceLists)
                {
                    yield return q => q.Bool(b => b
                        .Must(nq
                            => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                            && nq.Term(t => t.Field(f => f.Prices[0].CountrySystemId).Value(countrySystemId))
                            && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(false))
                        )
                    );
                }

                foreach (var item in container.Campaigns)
                {
                    yield return q => q.Bool(b => b
                        .Must(nq
                            => nq.Term(t => t.Field(f => f.Prices[0].SystemId).Value(item))
                            && nq.Term(t => t.Field(f => f.Prices[0].IsCampaignPrice).Value(true))
                        )
                    );
                }
            }
        }

        private List<Guid> GetCampaignsForUser()
        {
            var currencyID = _requestModelAccessor.RequestModel.Cart.OrderCarrier.CurrencyID;
            var groups = _accountService.GetGroupsSystemId(_securityContextService.GetIdentityUserSystemId().GetValueOrDefault()).ToList();
            var dte = DateTime.Now;

            return _campaignHandler
                .GetCampaigns(new CampaignHandlerArgs
                {
                    OnlyProductCampaigns = true,
                    OrderContainsCampaignInfo = false,
                    ChannelId = _requestModelAccessor.RequestModel.ChannelModel.SystemId
                })
                .Where(x => x.StartDate < dte
                    && x.EndDate > dte
                    && x.CurrencyId == currencyID
                    && x.Action is ArticleCampaignPriceAction
                    && x.Conditions.OfType<UserBelongsToGroupCondition>().All(z => ((Foundation.Modules.ECommerce.Plugins.Campaigns.ICondition)z).Process(new ConditionArgs { UserGroupIDs = groups })))
                .Select(x => x.CampaignID)
                .ToList();
        }

        private List<Guid> GetPriceListForUser()
        {
            var currencySystemId = _requestModelAccessor.RequestModel.Cart.OrderCarrier.CurrencyID;
            var priceCalculatorArgs = new PriceCalculatorArgs
            {
                WebSiteSystemId = _requestModelAccessor.RequestModel.WebsiteModel.SystemId,
                CurrencySystemId = currencySystemId,
                UserSystemId = _securityContextService.GetIdentityUserSystemId().GetValueOrDefault(),
                DateTimeUtc = _requestModelAccessor.RequestModel.DateTimeUtc,
                CountrySystemId = _requestModelAccessor.RequestModel.CountryModel?.SystemId ?? Guid.Empty
            };
            var result = _priceCalculator.GetPriceLists(priceCalculatorArgs).Select(x => x.SystemId);
            return result.Distinct().ToList();
        }

        public class Container
        {
            private readonly Lazy<IEnumerable<Guid>> _priceListAccessor;
            private readonly Lazy<IEnumerable<Guid>> _campaignsAccessor;

            internal Container(Lazy<IEnumerable<Guid>> priceListAccessor, Lazy<IEnumerable<Guid>> campaignsAccessor)
            {
                _priceListAccessor = priceListAccessor;
                _campaignsAccessor = campaignsAccessor;
            }

            public IEnumerable<Guid> PriceLists => _priceListAccessor.Value;
            public IEnumerable<Guid> Campaigns => _campaignsAccessor.Value;
        }
    }
}
