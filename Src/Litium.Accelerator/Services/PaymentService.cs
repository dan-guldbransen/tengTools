using Litium.Accelerator.Payments;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Security;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(PaymentService))]
    public abstract class PaymentService
    {
        public abstract bool IsPaymentWidget(PaymentInfoCarrier paymentInfoCarrier);
        public abstract IPaymentWidget GetPaymentWidget(PaymentInfoCarrier paymentInfoCarrier);
        public abstract IPaymentWidget GetPaymentWidget(string paymentProvider);
        public abstract PaymentMethod GetPaymentMethod(string methodName, string providerName, SecurityToken token);
    }
}
