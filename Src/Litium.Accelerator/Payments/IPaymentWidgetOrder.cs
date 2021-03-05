using Litium.Foundation.Modules.ECommerce.Carriers;

namespace Litium.Accelerator.Payments
{
    public interface IPaymentWidgetOrder
    {
        string OrderStatus { get; }

        OrderCarrier OrderCarrier { get; }

        string PaymentProviderName { get; }
        string ProviderOrderId { get; }

        bool IsCompleted { get; }
        bool IsCreated { get; }

        void Refresh();

        void Update(OrderCarrier orderCarrier);
    }
}
