using Litium.Foundation;
using Litium.Foundation.Log;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.StateTransitionSystem;

namespace Litium.Accelerator.StateTransitions
{
    /// <summary>
    ///     Controlling of whether an order is editable from UI or not, based on the order state.
    /// </summary>
    public class PropertyMetaInfoBuilder
    {
        public void Build(PropertyMetaInfoBuilder<OrderCarrier> propertyMetaInfoBuilder)
        {
            //set order to be not deletable for all states.
            propertyMetaInfoBuilder.SetEntityDeletable(false);
            //but, order is deletable in Init state.
            propertyMetaInfoBuilder.SetEntityDeletable((short)OrderState.Init, true);

            //set order to be not editable for all states.
            propertyMetaInfoBuilder.SetEntityEditable(false);
            //but, order is editable in Init State.
            propertyMetaInfoBuilder.SetEntityEditable((short)OrderState.Init, true);

            //set delivery cost readonly to false.
            propertyMetaInfoBuilder.CreateDeliveryPropertyMetaInfo("DeliveryCost", (short)OrderState.Init, false, false, true, true);

            //the OrderDelivery and OrderPayment states are not used.
            SetOrderDeliveryStatusMetaInfo(propertyMetaInfoBuilder);
            SetOrderPaymentStatusMetaInfo(propertyMetaInfoBuilder);
        }

        /// <summary>
        ///     Set meta information.
        /// </summary>
        /// <param name="propertyMetaInfoBuilder">Meta info builder to use.</param>
        private static void SetOrderDeliveryStatusMetaInfo(PropertyMetaInfoBuilder<OrderCarrier> propertyMetaInfoBuilder)
        {
            try
            {
                //the OrderDelivery states are not used.
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Attention, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Cancelled, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Completed, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Confirmed, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Init, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Processing, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("DeliveryStatus", (short)OrderState.Returned, false, false, true, false);
            }
            catch (InvalidStateException<OrderCarrier> ex)
            {
                Solution.Instance.Log.CreateLogEntry("order state" + ex.Message + " is not valid", ex, LogLevels.FATAL);
            }
        }

        /// <summary>
        ///     Set meta information.
        /// </summary>
        /// <param name="propertyMetaInfoBuilder">Meta info builder to use.</param>
        private static void SetOrderPaymentStatusMetaInfo(PropertyMetaInfoBuilder<OrderCarrier> propertyMetaInfoBuilder)
        {
            try
            {
                //the OrderPayment states are not used.
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Attention, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Cancelled, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Completed, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Confirmed, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Init, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Processing, false, false, true, false);
                propertyMetaInfoBuilder.CreateOrderPropertyMetaInfo("PaymentStatus", (short)OrderState.Returned, false, false, true, false);
            }
            catch (InvalidStateException<OrderCarrier> ex)
            {
                Solution.Instance.Log.CreateLogEntry("order state" + ex.Message + " is not valid", ex, LogLevels.FATAL);
            }
        }
    }
}
