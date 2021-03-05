using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Account;
using Litium.Foundation.Modules.CMS;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Actions;
using Litium.Foundation.Modules.ECommerce.Plugins.Campaigns.Conditions;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.ExtensionMethods;
using Litium.Foundation.Modules.ProductCatalog.Search;
using Litium.Foundation.Search;
using Litium.Framework.Search;
using Litium.Products.PriceCalculator;
using Litium.Runtime.DependencyInjection;
using Litium.Security;

namespace Litium.Accelerator.Search.Filtering
{
    [Service(ServiceType = typeof(PriceFilterService), Lifetime = DependencyLifetime.Transient)]
    public class PriceFilterService
    {
        private readonly ICampaignHandler _campaignHandler;
        private readonly Lazy<List<Guid>> _campaignsForUser;
        private readonly CategoryFilterService _categoryFilterService;
        private readonly Lazy<List<Guid>> _priceListForUser;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly IPriceCalculator _priceCalculator;
        private readonly AccountService _accountService;
        private readonly SecurityContextService _securityContextService;

        public PriceFilterService(
            ICampaignHandler campaignHandler,
            CategoryFilterService categoryFilterService,
            RequestModelAccessor requestModelAccessor,
            IPriceCalculator priceCalculator,
            AccountService accountService,
            SecurityContextService securityContextService)
        {
            _campaignHandler = campaignHandler;
            _categoryFilterService = categoryFilterService;
            _requestModelAccessor = requestModelAccessor;
            _priceCalculator = priceCalculator;
            _accountService = accountService;
            _securityContextService = securityContextService;

            _priceListForUser = new Lazy<List<Guid>>(() => GetPriceListForUser());
            _campaignsForUser = new Lazy<List<Guid>>(() => GetCampaignsForUser());
        }

        public void AddFilterReadTags(QueryRequest request, Guid categorySystemId)
        {
            var cultureInfo = new Guid(request.LanguageId).GetLanguage()?.CultureInfo;
            var priceListIDsForUser = _priceListForUser.Value;
            var campaigns = _campaignsForUser.Value;
            var countryId = _requestModelAccessor.RequestModel.CountryModel.SystemId;

            request.ReadTags.Add(TagNames.CategorySystemId);

            foreach (var item in priceListIDsForUser)
            {
                var name = _requestModelAccessor.RequestModel.Cart.IncludeVAT ? TagNames.GetTagNameForPriceIncludingVAT(item, countryId) : TagNames.GetTagNameForPriceExludingVAT(item, countryId);
                if (!request.ReadTags.Contains(name))
                {
                    request.ReadTags.Add(name);
                }
            }

            foreach (var item in campaigns)
            {
                var name = FilteringConstants.GetCampaignTagName(item);
                if (!request.ReadTags.Contains(name))
                {
                    request.ReadTags.Add(name);
                }
            }

            foreach (var item in _categoryFilterService.GetFilters(categorySystemId).Where(x => !x.StartsWith("#")))
            {
                var name = item.GetFieldDefinitionForProducts()?.GetTagName(cultureInfo);
                if (name != null && !request.ReadTags.Contains(name))
                {
                    request.ReadTags.Add(name);
                }
            }
        }

        public void AddPriceFilterTags(SearchQuery searchQuery, QueryRequest request)
        {
            if (searchQuery.ContainsPriceFilter())
            {
                var priceListSystemIds = _priceListForUser.Value;
                var campaignSystemIds = _campaignsForUser.Value;
                var allPrices = new List<(int Min, int Max)>(searchQuery.PriceRanges);
                var countryId = _requestModelAccessor.RequestModel.CountryModel.SystemId;

                var tagClause = new OptionalTagClause();
                foreach (var item in priceListSystemIds)
                {
                    var name = _requestModelAccessor.RequestModel.Cart.IncludeVAT 
                        ? TagNames.GetTagNameForPriceIncludingVAT(item, countryId) 
                        : TagNames.GetTagNameForPriceExludingVAT(item, countryId);

                    request.ReadTags.Add(name);
                    foreach (var priceItem in allPrices)
                    {
                        tagClause.Tags.Add(new RangeTag(name, priceItem.Min, priceItem.Max));
                    }
                }

                foreach (var item in campaignSystemIds)
                {
                    var name = FilteringConstants.GetCampaignTagName(item);
                    request.ReadTags.Add(name);
                    foreach (var priceItem in allPrices)
                    {
                        tagClause.Tags.Add(new RangeTag(name, priceItem.Min, priceItem.Max));
                    }
                }

                if (tagClause.TagsExist)
                {
                    request.FilterTags.Add(tagClause);
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
                    && x.Conditions.OfType<UserBelongsToGroupCondition>().All(z => ((ICondition)z).Process(new ConditionArgs { UserGroupIDs = groups })))
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
    }
}
