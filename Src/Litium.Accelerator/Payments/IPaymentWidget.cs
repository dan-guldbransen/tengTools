using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.Payments;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Payments
{
    public interface IPaymentWidget
    {
        PaymentWidgetResult GetWidget(OrderCarrier order, PaymentInfoCarrier paymentInfo);
        bool IsEnabled(string paymentMethod);
    }

    [Service(ServiceType = typeof(IPaymentWidget<>), Lifetime = DependencyLifetime.Transient)]
    public interface IPaymentWidget<TPaymentProvider> : IPaymentWidget
       where TPaymentProvider : IPaymentProvider
    {
    }
}


