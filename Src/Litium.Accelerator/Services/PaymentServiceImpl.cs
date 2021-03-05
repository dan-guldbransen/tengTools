using Litium.Accelerator.Payments;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.Plugins.Payments;
using Litium.Foundation.Security;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(PaymentService), FallbackService = true)]
    public class PaymentServiceImpl : PaymentService
    {
        private readonly ModuleECommerce _moduleECommerce;

        public PaymentServiceImpl(ModuleECommerce moduleECommerce)
        {
            _moduleECommerce = moduleECommerce;
        }

        public override bool IsPaymentWidget(PaymentInfoCarrier paymentInfoCarrier)
            => GetPaymentWidget(paymentInfoCarrier) != null;

        public override IPaymentWidget GetPaymentWidget(PaymentInfoCarrier paymentInfoCarrier)
        {
            var paymentWidget = GetPaymentWidget(paymentInfoCarrier?.PaymentProvider);
            if (paymentWidget?.IsEnabled(paymentInfoCarrier?.PaymentMethod) == true)
            {
                return paymentWidget;
            }

            return null;
        }

        public override IPaymentWidget GetPaymentWidget(string paymentProvider)
        {
            return paymentProvider == null
                ? null
                : IoC.Resolve(typeof(IPaymentWidget<>).MakeGenericType(IoC.ResolvePlugin<IPaymentProvider>(paymentProvider).GetType())) as IPaymentWidget;
        }

        public override PaymentMethod GetPaymentMethod(string methodName, string providerName, SecurityToken token)
            => _moduleECommerce.PaymentMethods.Get(methodName, providerName, token);
    }
}
