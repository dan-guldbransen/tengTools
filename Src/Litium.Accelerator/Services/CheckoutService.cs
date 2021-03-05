using Litium.Accelerator.ViewModels.Checkout;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Payments;
using Litium.Runtime.DependencyInjection;
using System;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(CheckoutService))]
    [RequireServiceImplementation]
    public abstract class CheckoutService
    {
        public abstract void ChangePaymentMethod(string paymentMethodId);
        public abstract void ChangeDeliveryMethod(Guid deliveryMethodId);
        public abstract bool SetCampaignCode(string campaignCode);
        public abstract ExecutePaymentResult PlaceOrder(CheckoutViewModel model, string responseUrl, string cancelUrl, out string redirectUrl);
        public abstract void SetOrderDetails(CheckoutViewModel model);
        public abstract string GetOrderConfirmationPageUrl(OrderCarrier orderCarrier);
        public abstract string HandlePaymentResult(bool isSuccess, string errorMessage);
        public abstract bool Validate(ModelState modelState, CheckoutViewModel viewModel);
        public abstract bool ValidateOrder(out string message);
    }
}
