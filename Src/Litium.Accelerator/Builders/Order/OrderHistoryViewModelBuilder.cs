using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Routing;
using Litium.Accelerator.StateTransitions;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Order;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Search;
using Litium.Foundation.Search;
using Litium.Framework.Search;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Web;
using Litium.Web.Models.Websites;

namespace Litium.Accelerator.Builders.Order
{
    public class OrderHistoryViewModelBuilder : IViewModelBuilder<OrderHistoryViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly SecurityContextService _securityContextService;
        private readonly UrlService _urlService;
        private readonly LanguageService _languageService;
        private readonly ModuleECommerce _moduleECommerce;
        private readonly OrderViewModelBuilder _orderViewModelBuilder;
        private readonly PersonStorage _personStorage;
        private const int DefaultNumberOfOrderPerPage = 10;

        public OrderHistoryViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            SecurityContextService securityContextService,
            UrlService urlService,
            LanguageService languageService,
            ModuleECommerce moduleECommerce,
            OrderViewModelBuilder orderViewModelBuilder,
            PersonStorage personStorage)
        {
            _requestModelAccessor = requestModelAccessor;
            _securityContextService = securityContextService;
            _urlService = urlService;
            _languageService = languageService;
            _moduleECommerce = moduleECommerce;
            _orderViewModelBuilder = orderViewModelBuilder;
            _personStorage = personStorage;
        }

        public virtual OrderHistoryViewModel Build(int pageIndex, bool showOnlyMyOrders)
            => Build(_requestModelAccessor.RequestModel.CurrentPageModel, pageIndex, showOnlyMyOrders);

        public virtual OrderHistoryViewModel Build(PageModel pageModel, int pageIndex, bool showOnlyMyOrders)
        {
            var model = pageModel.MapTo<OrderHistoryViewModel>();
            model.IsBusinessCustomer = _personStorage.CurrentSelectedOrganization != null;

            if(model.IsBusinessCustomer)
            {
                model.HasApproverRole = _personStorage.HasApproverRole;
                model.ShowOnlyMyOrders = showOnlyMyOrders;
                model.MyOrdersLink = GetMyOrdersLink(pageModel, showOnlyMyOrders);
            }

            var itemsPerPage = model.NumberOfOrdersPerPage > 0 ? model.NumberOfOrdersPerPage : DefaultNumberOfOrderPerPage;
            model.Orders = GetOrders(model, pageIndex, itemsPerPage, out int totalOrders);
            model.Pagination = new PaginationViewModel(totalOrders, pageIndex, itemsPerPage);

            return model;
        }

        private List<OrderDetailsViewModel> GetOrders(OrderHistoryViewModel model, int pageIndex, int pageSize, out int totalOrderCount)
        {
            var orders = new List<OrderDetailsViewModel>();
            totalOrderCount = 0;
            var personId = _securityContextService.GetIdentityUserSystemId();

            if (!ModuleECommerce.ExistsInstance || !personId.HasValue)
            {
                return orders;
            }
            
            var searchRequest = new QueryRequest(_languageService.Get(CultureInfo.CurrentCulture).SystemId, ECommerceSearchDomains.Orders, Solution.Instance.SystemToken)
            {
                Paging = new Paging(pageIndex, pageSize)
            };

            if (model.IsBusinessCustomer)
            {
                searchRequest.FilterTags.Add(new Tag(TagNames.OrganizationID, _personStorage.CurrentSelectedOrganization.SystemId));
                if (model.ShowOnlyMyOrders || !model.HasApproverRole)
                {
                    searchRequest.FilterTags.Add(new Tag(TagNames.PersonID, personId.Value));
                }
            }
            else
            {
                searchRequest.FilterTags.Add(new Tag(TagNames.PersonID, personId.Value));
            }
                
            searchRequest.ExcludeTags.Add(new Tag(TagNames.OrderStatus, (short)OrderState.Invalid));
            searchRequest.ExcludeTags.Add(new Tag(TagNames.OrderStatus, (short)OrderState.Init));
            searchRequest.Sortings.Add(new Sorting(TagNames.OrderNumber, SortDirection.Descending, SortingFieldType.String));

            var responce = Solution.Instance.SearchService.Search(searchRequest);
            totalOrderCount = responce.TotalHitCount;

            foreach (var order in _moduleECommerce.Orders.GetOrders(responce.Hits.Select(x => new Guid(x.Id)), Solution.Instance.SystemToken))
            {
                if (order.OrderStatus != (short)OrderState.Invalid && order.OrderStatus != (short)OrderState.Init)
                {
                    orders.Add(_orderViewModelBuilder.Build(order));
                }
            }

            return orders;
        }

        private string GetMyOrdersLink(PageModel pageModel, bool showOnlyMyOrders)
        {
            var myOrdersLink = _urlService.GetUrl(pageModel.Page);
            return !showOnlyMyOrders ? $"{myOrdersLink}?showMyOrders={true}" : myOrdersLink;
        }
    }
}

