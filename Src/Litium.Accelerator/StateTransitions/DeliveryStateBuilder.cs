using Litium.Accelerator.Utilities;
using Litium.Connect.Erp;
using Litium.Connect.Erp.Events;
using Litium.Events;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.StateTransitionSystem;
using Litium.Runtime.DependencyInjection;
using System.Linq;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.StateTransitions
{
    /// <summary>
    ///     Builds delivery states.
    /// </summary>
    [Service(ServiceType = typeof(DeliveryStateBuilder), Lifetime = DependencyLifetime.Transient)]
    public class DeliveryStateBuilder
    {
        private readonly OrderUtilities _orderUtilities;
        private const string RELATED_PAYMENT_KEY = "RelatedPaymentInfo";
        private readonly EventBroker _eventBroker;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="orderUtilities"></param>
        public DeliveryStateBuilder(
            OrderUtilities orderUtilities, EventBroker eventBroker)
        {
            _orderUtilities = orderUtilities;
            _eventBroker = eventBroker;
        }

        /// <summary>
        ///     Builds the delivery states.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        public virtual void Build(DeliveryStateMachine stateMachine)
        {
            //ReadyToShip: Package is ready to ship.
            var readyToShip = new State<DeliveryCarrier>((short)DeliveryState.ReadyToShip,
                DeliveryState.ReadyToShip.ToString(),
                (deliveryCarrier, currentState, token) =>
                {
                    //Delivery processing entry action.

                    //TODO: Integration code to execute, when start processing the delivery.
                    //...
                },
                null);
            
            //Processing: Delivery has started processing.
            var processing = new State<DeliveryCarrier>((short)DeliveryState.Processing,
                DeliveryState.Processing.ToString(),
                (deliveryCarrier, currentState, token) =>
                {
                    //Delivery processing entry action.

                    //TODO: Integration code to execute, when start processing the delivery.
                    //...
                },
                null);

            //Delivered: Delivery is completed, usually this state means that delivery is sent from merchants warehouse to the distributor.
            var delivered = new State<DeliveryCarrier>((short)DeliveryState.Delivered,
                DeliveryState.Delivered.ToString(),
                (deliveryCarrier, currentState, token) =>
                {
                    //Delivery processing entry action.

                    //TODO: Integration code to execute, when delivery is sent out from warehouse.
                    //...

                    //Collect the payment, if the payment is still Reserved.

                    //For partial shipping: If delivery has related payment then this is a splitted delivery and
                    //when setting this to delivered, we should not trigger auto capture all order's payments.
                    if (!deliveryCarrier.AdditionalDeliveryInfo.Any(x => x.Key == RELATED_PAYMENT_KEY))
                    {
                        _orderUtilities.CompletePayments(ModuleECommerce.Instance.Orders[deliveryCarrier.OrderID, token], token);
                    }
                    
                },
                null);

            //Cancelled: Delivery is cancelled, e.g. requested by end customer, or cancelled due to delivery provider failed to deliver the item.
            var cancelled = new State<DeliveryCarrier>((short)DeliveryState.Cancelled,
                DeliveryState.Cancelled.ToString(),
                (deliveryCarrier, currentState, token) =>
                {
                    //Delivery cancellation entry action.

                    //TODO: Integration code to execute, when the delivery is cancelled.
                    //...

                    //TODO: Send delivery cancellation email.
                    //...

                    //Return all the payments, if they are already Paid, or Cancel them if they are still reserved.
                    _orderUtilities.ReturnOrCancelAllPayments(ModuleECommerce.Instance.Orders[deliveryCarrier.OrderID, token], token);
                },
                null);

            //Failed: Delivery failed, e.g. delivery provider failed to delivery the item to delivery address specified.
            var failed = new State<DeliveryCarrier>((short)DeliveryState.Failed,
                DeliveryState.Failed.ToString(),
                (deliveryCarrier, currentState, token) =>
                {
                    //Delivery failure entry action.

                    //TODO: Integration code to execute, when the delivery was sent to distributor, but then it failed.
                    //...

                    //TODO: Send delivery failure notice email.
                    //...
                },
                null);

            //Returned: Delivery is returned by the end customer.
            var returned = new State<DeliveryCarrier>((short)DeliveryState.Returned,
                DeliveryState.Returned.ToString(),
                (deliveryCarrier, currentState, token) =>
                {                    
                    //TODO: Integration code to execute, when the delivery was returned.
                    //...

                    //TODO: Send delivery return notice email.
                    //...

                    //Return all the payments, if they are already Paid, or Cancel them if they are still reserved.
                    _orderUtilities.ReturnOrCancelAllPayments(ModuleECommerce.Instance.Orders[deliveryCarrier.OrderID, token], token);
                },
                null);

            //add state transitions.
            //from Init to Processing.
            stateMachine.AddStateTransition(new State<DeliveryCarrier>((short)DeliveryState.Init, DeliveryState.Init.ToString()),
                processing,
                (deliveryCarrier, startState, endState, token) =>
                {
                    //Condition for delivery to go from Init to Processing.

                    //Delivery needs a confirmed order to start processing.
                    var order = ModuleECommerce.Instance.Orders[deliveryCarrier.OrderID, token];
                    return order != null && order.OrderStatus == (short)OrderState.Confirmed &&
                    //Should have at least one row to process.
                    order.OrderRows.Any(x => x.DeliveryID == deliveryCarrier.ID);
                },
                true);

            //from init to cancelled.
            stateMachine.AddStateTransition(new State<DeliveryCarrier>((short)DeliveryState.Init, DeliveryState.Init.ToString()), cancelled);
            //from processing to ReadyToship.
            stateMachine.AddStateTransition(new State<DeliveryCarrier>((short)DeliveryState.Processing, DeliveryState.Processing.ToString()), readyToShip, (deliveryCarrier, startState, endState, token) =>
            {
                //Condition for delivery to go from Init to ReadyToShip.

                //Delivery needs a processing order to move to ReadyToShip.
                var order = ModuleECommerce.Instance.Orders[deliveryCarrier.OrderID, token];
                return order != null && order.OrderStatus == (short)OrderState.Processing &&
                //Should have at least one row to process.
                order.OrderRows.Any(x => x.DeliveryID == deliveryCarrier.ID);
            }, 
            true,
            (deliveryCarrier, currentState, previousState, token) =>
            {
                //Notify all webhook subscribers for package ready to ship.
                _eventBroker.Publish(new ReadyToShip(deliveryCarrier.ID, deliveryCarrier.MapTo<Shipment>()));
            });
            //from ReadyToship to Delivered.
            stateMachine.AddStateTransition(readyToShip, delivered, (deliveryCarrier, startState, endState, token) =>
            {
                //Condition for delivery to go from ReadyToShip to Delivered.
                var order = ModuleECommerce.Instance.Orders[deliveryCarrier.OrderID, token];
                var paymentAdditionInfo = deliveryCarrier.AdditionalDeliveryInfo.FirstOrDefault(x => x.Key == "RelatedPaymentInfo");
                //Related payment should have status of Captured to move the delivery from ReadyToShip to Delivered.
                
                var relatedPayment = order.PaymentInfo.Count() == 1 ? order.PaymentInfo.First(): order.PaymentInfo.FirstOrDefault(x => x.ReferenceID == paymentAdditionInfo.Value);
                if (relatedPayment == null)
                {
                    return false;
                }
                return relatedPayment.PaymentStatus == PaymentStatus.Paid;
            }, true);
            //from Processing to Delivered.
            stateMachine.AddStateTransition(processing, delivered);
            //from Processing to Cancelled.
            stateMachine.AddStateTransition(processing, cancelled);            
            //from delivered to returned.
            stateMachine.AddStateTransition(delivered, returned);
            //from delivered to failed.
            stateMachine.AddStateTransition(delivered, failed);
            //from failed to delivered.
            stateMachine.AddStateTransition(failed, delivered);
            //from failed to cancelled
            stateMachine.AddStateTransition(failed, cancelled);
        }
    }
}
