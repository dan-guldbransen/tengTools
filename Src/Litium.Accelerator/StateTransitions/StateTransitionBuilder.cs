using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.Plugins.StateTransition;
using Litium.Foundation.Modules.ECommerce.StateTransitionSystem;
using Litium.Sales;

namespace Litium.Accelerator.StateTransitions
{
    /// <summary>
    ///     Builds state transitions.
    /// </summary>
    public class StateTransitionBuilder : StateTransitionBuilderBase
    {
        private const int _notUsedStateId = 99;
        private readonly RmaStateBuilder _rmaStateBuilder;
        private readonly OrderStateBuilder _orderStateBuilder;
        private readonly DeliveryStateBuilder _deliveryStateBuilder;
        /// <summary>
        /// Main class to build the state transitions 
        /// </summary>
        /// <param name="rmaStateBuilder">Rma state builder</param>
        /// <param name="orderStateBuilder">Order state builder</param>
        /// <param name="deliveryStateBuilder">Delivery state builder</param>
        public StateTransitionBuilder(RmaStateBuilder rmaStateBuilder, OrderStateBuilder orderStateBuilder, DeliveryStateBuilder deliveryStateBuilder)
        {
            _rmaStateBuilder = rmaStateBuilder;
            _orderStateBuilder = orderStateBuilder;
            _deliveryStateBuilder = deliveryStateBuilder;
        }

        /// <summary>
        ///     Build meta information about the properties, based on OrderState.
        ///     This allows controlling of whether an order is editable from UI or not, based on the order state.
        /// </summary>
        /// <remarks>
        ///     This information is not affecting the functionality of public website or API.
        ///     This only affect the administration GUI.
        /// </remarks>
        /// <param name="propertyMetaInfoBuilder">Property meta info builder object utilized to build meta information.</param>
        public override void BuildOrderStatePropertyMetaInfo(PropertyMetaInfoBuilder<OrderCarrier> propertyMetaInfoBuilder)
        {
            base.BuildOrderStatePropertyMetaInfo(propertyMetaInfoBuilder);
            var customizedBuilder = new PropertyMetaInfoBuilder();
            customizedBuilder.Build(propertyMetaInfoBuilder);
        }

        /// <summary>
        ///     Builds the state transition system.
        /// </summary>
        public override void BuildStateTransitions(StateTransitionsManager stateManager)
        {
            //need init state for all the state machines, even if we dont use them.
            //the initial state of OrderDelivery is called Init.
            stateManager.OrderDeliveryStateMachine.AddInitialState(new State<OrderCarrier>(_notUsedStateId, "NotUsed"));
            stateManager.OrderRowStateMachine.AddInitialState(new State<OrderRowCarrier>(_notUsedStateId, "NotUsed"));

            //add init state to Order and delivery state machines.
            stateManager.OrderStateMachine.AddInitialState(new State<OrderCarrier>((short)OrderState.Init, OrderState.Init.ToString()));
            stateManager.DeliveryStateMachine.AddInitialState(new State<DeliveryCarrier>((short)DeliveryState.Init, DeliveryState.Init.ToString()));

            //add init state to Rma
            stateManager.RmaStateMachine.AddInitialState(new State<RmaCarrier>((short)RmaState.Init,RmaState.Init.Name));

            //build state transitions.
            _orderStateBuilder.Build(stateManager.OrderStateMachine);
            _deliveryStateBuilder.Build(stateManager.DeliveryStateMachine);
            _rmaStateBuilder.Build(stateManager.RmaStateMachine);

            //related state transitions. e.g "When Delivery and Payments go to a particular state, order should probably by in following states."
            //Confirmed state: 
            //Check whether order can be put into confirmed state when Payment state is Paid or Reserved or Pending.
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Reserved, FiniteStateMachineType.Order, (short)OrderState.WaitingConfirmation);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Reserved, FiniteStateMachineType.Order, (short)OrderState.Confirmed);

            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Paid, FiniteStateMachineType.Order, (short)OrderState.WaitingConfirmation);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Paid, FiniteStateMachineType.Order, (short)OrderState.Confirmed);

            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Pending, FiniteStateMachineType.Order, (short)OrderState.WaitingConfirmation);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Pending, FiniteStateMachineType.Order, (short)OrderState.Confirmed);

            stateManager.RmaStateMachine.AddRelatedState((short)RmaState.Completed, FiniteStateMachineType.Order, (short)OrderState.ReturnConfirmed);

            //Cancelled state:
            //check whether Order can be put into cancelled state after delivery is cancelled.
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Cancelled, FiniteStateMachineType.Order, (short)OrderState.Cancelled);
            //check whether order can be put into cancelled state after payments are returned, or cancelled.
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Cancelled, FiniteStateMachineType.Order, (short)OrderState.Cancelled);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Returned, FiniteStateMachineType.Order, (short)OrderState.Cancelled);

            //Processing state:
            //check whether order can be put into processing when delivery process has started.
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Processing, FiniteStateMachineType.Order, (short)OrderState.Processing);

            //Completed state:
            //check whether order can be put into completed when delivery is completed.
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Delivered, FiniteStateMachineType.Order, (short)OrderState.Completed);
            //check whether order can be put into completed when payment is paid.
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Paid, FiniteStateMachineType.Order, (short)OrderState.Completed);

            //Returned state:
            //check whether order can be put into returned when delivery is returned.
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Returned, FiniteStateMachineType.Order, (short)OrderState.ReturnProcessing);
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Returned, FiniteStateMachineType.Order, (short)OrderState.ReturnCompleted);
            //check whether order can be put into cancelled state after payments are returned, or cancelled.
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Returned, FiniteStateMachineType.Order, (short)OrderState.ReturnProcessing);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Returned, FiniteStateMachineType.Order, (short)OrderState.ReturnCompleted);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Cancelled, FiniteStateMachineType.Order, (short)OrderState.ReturnProcessing);
            stateManager.AddRelatedStateToPaymentState((short)PaymentStatus.Cancelled, FiniteStateMachineType.Order, (short)OrderState.ReturnCompleted);

            //Attention state:
            //check whether order can be put into attention when delivery is cancelled.            
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Cancelled, FiniteStateMachineType.Order, (short)OrderState.Attention);
            //check whether order can be put into attention when delivery is returned.
            stateManager.DeliveryStateMachine.AddRelatedState((short)DeliveryState.Returned, FiniteStateMachineType.Order, (short)OrderState.Attention);
        }
    }
}
