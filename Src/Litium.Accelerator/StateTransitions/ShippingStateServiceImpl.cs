using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Deliveries;
using Litium.Foundation.Security;
using Litium.Sales;

namespace Litium.Accelerator.StateTransitions
{
    public class ShippingStateServiceImpl : ShippingStateService
    {
        public override short GetReturnedState()
        {
            return (short)DeliveryState.Returned;
        }

        public override bool IsDeliveredState(DeliveryCarrier delivery)
        {
            return delivery.DeliveryStatus == (short)DeliveryState.Delivered;
        }

        public override bool IsInitState(DeliveryCarrier delivery)
        {
            return delivery.DeliveryStatus == (short)DeliveryState.Init;
        }

        public override bool IsProcessingState(DeliveryCarrier delivery)
        {
            return delivery.DeliveryStatus == (short)DeliveryState.Processing;
        }

        public override bool IsReadyToShipState(DeliveryCarrier delivery)
        {
            return delivery.DeliveryStatus == (short)DeliveryState.ReadyToShip;
        }

        public override bool IsReturnedState(DeliveryCarrier delivery)
        {
            return delivery.DeliveryStatus == (short)DeliveryState.Returned;
        }

        public override void SetDelivered(Delivery delivery, SecurityToken token)
        {
            delivery.SetDeliveryStatus((short)DeliveryState.Delivered , token);
        }

        public override void SetProcessing(Delivery delivery, SecurityToken token)
        {
            delivery.SetDeliveryStatus((short)DeliveryState.Processing, token);
        }

        public override void SetReadyToShip(Delivery delivery, SecurityToken token)
        {
            delivery.SetDeliveryStatus((short)DeliveryState.ReadyToShip, token);
        }

        public override void SetReturned(Delivery delivery, SecurityToken token)
        {
            delivery.SetDeliveryStatus((short)DeliveryState.Returned, token);
        }
    }
}
