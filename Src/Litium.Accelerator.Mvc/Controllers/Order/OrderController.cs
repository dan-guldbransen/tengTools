using System;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Order;
using Litium.Accelerator.Services;
using Litium.Accelerator.StateTransitions;
using Litium.Web.Models.Websites;

namespace Litium.Accelerator.Mvc.Controllers.Order
{
    public class OrderController : ControllerBase
    {
        private readonly OrderHistoryViewModelBuilder _orderHistoryViewModelBuilder;
        private readonly OrderViewModelBuilder _orderViewModelBuilder;
        private readonly OrderHistoryViewModelService _orderHistoryViewModelService;
        private readonly OrderConfirmationViewModelBuilder _orderConfirmationViewModelBuilder;
        private readonly CartService _cartService;

        private readonly string BusinessCustomerOrderView = "BusinessCustomerOrder";
        private readonly string OrderConfirmationEmailView = "../Mail/ConfirmationEmail";

        public OrderController(OrderHistoryViewModelBuilder orderHistoryViewModelBuilder, 
            OrderViewModelBuilder orderViewModelBuilder, 
            OrderHistoryViewModelService orderHistoryViewModelService,
            CartService cartService,
            OrderConfirmationViewModelBuilder orderConfirmationViewModelBuilder)
        {
            _orderHistoryViewModelBuilder = orderHistoryViewModelBuilder;
            _orderViewModelBuilder = orderViewModelBuilder;
            _orderHistoryViewModelService = orderHistoryViewModelService;
            _orderConfirmationViewModelBuilder = orderConfirmationViewModelBuilder;
            _cartService = cartService;
        }

        [HttpGet]
        public ActionResult List(int page = 1, bool showMyOrders = false)
        {
            var model = _orderHistoryViewModelBuilder.Build(page, showMyOrders);
            return View(model);
        }

        [HttpGet]
        public ActionResult Order(Guid? id, bool print = false)
        {
            var model = _orderViewModelBuilder.Build(id.GetValueOrDefault(), print);
            if (model.IsBusinessCustomer)
            {
                return View(BusinessCustomerOrderView, model);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Confirmation(PageModel currentPageModel, Guid? orderId, bool isEmail = false)
        {
            if (isEmail)
            {
                if (orderId.HasValue && orderId.Value != Guid.Empty)
                {
                    // In case of sending the order confirmation email, we get the orderId from the query string
                    var model = _orderConfirmationViewModelBuilder.Build(currentPageModel, orderId.Value);
                    return View(OrderConfirmationEmailView, model);
                }
                else
                {
                    this.Log().Error("An error occured while trying to render order confirmation email");
                    return View(OrderConfirmationEmailView);
                }
            }
            else
            {
                // In case of loading the order confirmation page, we get the orderId from the order carrier
                var model = orderId == default ? _orderConfirmationViewModelBuilder.Build(currentPageModel) : _orderConfirmationViewModelBuilder.Build(currentPageModel, orderId.Value);
                _cartService.Clear();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveOrder(Guid id, bool isOrderPage = false, int page = 1, bool showMyOrders = false)
        {
            if (id != Guid.Empty)
            {
                _orderHistoryViewModelService.SaveOrder(OrderState.Confirmed, id);
            }

            if (isOrderPage)
            {
                return RedirectToAction(nameof(Order), new { Id = id});
            }

            if (showMyOrders)
            {
                return RedirectToAction(nameof(List), new { page, showMyOrders });
            }
            else
            {
                return RedirectToAction(nameof(List), new { page });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(Guid id, bool isOrderPage = false, int page = 1, bool showMyOrders = false)
        {
            if (id != Guid.Empty)
            {
                _orderHistoryViewModelService.SaveOrder(OrderState.Cancelled, id);
            }

            if (isOrderPage)
            {
                return RedirectToAction(nameof(Order), new { Id = id });
            }

            if (showMyOrders)
            {
                return RedirectToAction(nameof(List), new { page, showMyOrders });
            }
            else
            {
                return RedirectToAction(nameof(List), new { page });
            }
        }
    }
}
