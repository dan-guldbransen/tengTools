using System;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Mailing;
using Litium.Accelerator.Services;
using Litium.Accelerator.Utilities;
using Litium.Customers;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.StateTransitionSystem;
using Litium.Foundation.Security;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Events;
using Litium.Connect.Erp.Events;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Connect.Erp;
using OrderType = Litium.Sales.OrderType;

namespace Litium.Accelerator.StateTransitions
{
    /// <summary>
    ///     Build order states.
    /// </summary>
    [Service(ServiceType = typeof(OrderStateBuilder), Lifetime = DependencyLifetime.Transient)]
    public class OrderStateBuilder
    {
        private readonly OrderUtilities _orderUtilities;
        private readonly Sales.RmaService _rmaService;
        private readonly MailService _mailService;
        private readonly PersonService _personService;
        private readonly EventBroker _eventBroker;
        

        private readonly RoleService _roleService;
        private readonly AuthorizationService _authorizationService;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="orderUtilities"></param>
        public OrderStateBuilder(
            OrderUtilities orderUtilities,
            Sales.RmaService rmaService,
            MailService mailService,
            PersonService personService,
            RoleService roleService,
            AuthorizationService authorizationService,            
            EventBroker eventBroker)
        {
            _orderUtilities = orderUtilities;
            _rmaService = rmaService;
            _mailService = mailService;
            _personService = personService;
            _eventBroker = eventBroker;
            _roleService = roleService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        ///     Builds the order states.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        public virtual void Build(OrderStateMachine stateMachine)
        {
            //Order is placed, but waiting confirmation.
            var waitingConfirmation = new State<OrderCarrier>((short)OrderState.WaitingConfirmation, OrderState.WaitingConfirmation.ToString());

            //Order Confirmed
            var confirmed = new State<OrderCarrier>((short)OrderState.Confirmed, OrderState.Confirmed.ToString(),
                (orderCarrier, currentState, token) =>
                {
                    //Order confirmed entry action.

                    //now the order is confirmed, reduce the stock balances.
                    _orderUtilities.ReduceStockBalance(orderCarrier);

                    //notify campaigns engine to take order confirmation actions.
                    ModuleECommerce.Instance.CampaignCalculator.HandleOrderConfirmation(orderCarrier, ModuleECommerce.Instance.AdminToken);

                    //Send order confirmation email.
                    _mailService.SendEmail(new OrderConfirmationEmail(orderCarrier.ChannelID, orderCarrier.ID, orderCarrier.CustomerInfo.Address.Email), false);
                    
                    //TODO: integration code to execute when an order is confirmed.
                    //....
                },
                null);

            //Order Processing: Order has started processing. e.g. delivery packages are being processed.
            var processing = new State<OrderCarrier>((short)OrderState.Processing, OrderState.Processing.ToString());
            //Order Completed: All tasks relating to order is completed. 
            var completed = new State<OrderCarrier>((short)OrderState.Completed, OrderState.Completed.ToString());
            //Order Returned:order is returned.
            var returned = new State<OrderCarrier>((short)OrderState.Returned, OrderState.Returned.ToString());
            //Order Attention: requires administrators attention.
            var attention = new State<OrderCarrier>((short)OrderState.Attention, OrderState.Attention.ToString());
            //Order ClosedByAdmin: order is closed by administrator. 
            var closedByAdmin = new State<OrderCarrier>((short)OrderState.ClosedByAdmin, OrderState.ClosedByAdmin.ToString());
            //Order Cancelled: order is Cancelled either by end customer or administrator.
            var cancelled = new State<OrderCarrier>((short)OrderState.Cancelled, OrderState.Cancelled.ToString());

            //ReturnManagement State - These are new states for SaleReturnOrder only 

            // Rma is approved and the return is confirmed, this is the innitial status of Sales return order
            var returnConfirmed = new State<OrderCarrier>((short)OrderState.ReturnConfirmed, OrderState.ReturnConfirmed.ToString(),
                (orderCarrier, currentState, token) =>
                {
                    //TODO: integration code to execute when a sales return order is registered.
                    //TODO: send a returned items acceptance email to the end-customer.
                },
                null);
            // Sales return order is in processing.
            var returnProcessing = new State<OrderCarrier>((short)OrderState.ReturnProcessing, OrderState.ReturnProcessing.ToString());
            // Sales return order is now completed.
            var returnCompleted = new State<OrderCarrier>((short)OrderState.ReturnCompleted, OrderState.ReturnCompleted.ToString());

            //build state transitions.
            TransitionsFromInitState(stateMachine, waitingConfirmation, cancelled);
            TransitionsFromWaitingConfirmationState(stateMachine, waitingConfirmation, confirmed, cancelled);
            TransitionsFromConfirmedState(stateMachine, confirmed, processing, cancelled);
            TransitionsFromProcessingState(stateMachine, processing, completed, attention, cancelled);
            TransitionsFromCompletedState(stateMachine, completed, returned, attention);
            TransitionsFromAttentionState(stateMachine, completed, returned, attention, closedByAdmin, cancelled);
            stateMachine.AddStateTransition(closedByAdmin, attention);

            //state transitions for Sale Return Order
            SaleReturnOrderTransitionFromInitState(stateMachine, returnConfirmed);
            SaleReturnOrderTransitionFromReturnConfirmed(stateMachine, returnConfirmed, returnProcessing);
            SaleReturnOrderTransitionFromReturnProcessing(stateMachine, returnProcessing, returnCompleted);
        }

        private void SaleReturnOrderTransitionFromInitState(OrderStateMachine stateMachine, State<OrderCarrier> returnConfirmed)
        {
            //build SRO state transitions.
            //From Init to ReturnConfirmed.
            stateMachine.AddStateTransition(new State<OrderCarrier>((short)OrderState.Init, OrderState.Init.ToString()), returnConfirmed,
                (orderCarrier, startState, endState, token) =>
                {
                    //Conditions for SRO from init to ReturnConfirmed.
                    //Precodition:
                    //+RMA in Completed state and it should be Approved
                    //+Order is of type SalesReturnOrder                    
                    if (orderCarrier.Type != OrderType.SalesReturnOrder || orderCarrier.RmaSystemId is null || orderCarrier.RmaSystemId == Guid.Empty)
                    {
                        return false;
                    }
                    var rma = _rmaService.Get(orderCarrier.RmaSystemId.Value);

                    //RmaState.Completed denote that the Rma process is completed, the ApprovateCode.Approved denote that it actually is approved.
                    return rma?.State == RmaState.Completed && rma?.ApprovalCode == ApprovalCode.Approved;
                },
                false);
        }

        private void SaleReturnOrderTransitionFromReturnConfirmed(OrderStateMachine stateMachine, State<OrderCarrier> returnConfirmed, State<OrderCarrier> returnProcessing)
        {
            stateMachine.AddStateTransition(returnConfirmed, returnProcessing);
        }

        private void SaleReturnOrderTransitionFromReturnProcessing(OrderStateMachine stateMachine, State<OrderCarrier> returnProcessing, State<OrderCarrier> returnCompleted)
        {
            stateMachine.AddStateTransition(returnProcessing, returnCompleted);
        }



        /// <summary>
        ///     Determines whether current user can approve orders. Only users with role
        ///     order approval or administrators with content permissions for sales area can approve orders.
        /// </summary>
        /// <param name="orderCarrier">The order carrier.</param>
        /// <param name="token">The token.</param>
        private bool HasApproverRole(OrderCarrier orderCarrier, SecurityToken token)
        {
            if (orderCarrier.CustomerInfo.OrganizationID != Guid.Empty) // business customer case
            {
                using (token.Use())
                {
                    // Allow all account that have content permissions for sales to place orders for a user in the organization,
                    // i.e. merchant will approve the placed order.
                    if (_authorizationService.HasOperation(Operations.Function.Sales.Content))
                    {
                        return true;
                    }
                }

                //check permissions.
                var person = _personService.Get(token.UserID);
                if (person != null)
                {
                    var organization = person.OrganizationLinks.FirstOrDefault(x => x.OrganizationSystemId == orderCarrier.CustomerInfo.OrganizationID);
                    if (organization != null)
                    {
                        var roles = organization.RoleSystemIds.Select(x => _roleService.Get(x));
                        return roles.Any(item => item.Id == RolesConstants.RoleOrderApprover);
                    }
                }

                this.Log().Error("For the order with Id {0} ({1}), " +
                    "state transition engine could not determine whether user has order approval role or not for the user id {2}. " +
                    "User with this ID does not exist in as a customer or organization with organizationId {3} was not found.",
                    orderCarrier.ID,
                    orderCarrier.ExternalOrderID,
                    token.UserID,
                    orderCarrier.CustomerInfo.OrganizationID);

                return false;
            }
            else // private customer
            {
                return true;
            }
        }

        private static void TransitionsFromAttentionState(OrderStateMachine stateMachine, State<OrderCarrier> completed, State<OrderCarrier> returned, State<OrderCarrier> attention, State<OrderCarrier> closedByAdmin, State<OrderCarrier> cancelled)
        {
            //from Attention to Completed.
            //order can only be in completed if all payments paid and deliveries delivered.
            stateMachine.AddStateTransition(attention, completed,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from attention to completed. 

                    //order can only be in completed if all payments paid and deliveries delivered.
                    var allPaymentsPaid = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Paid);

                    var allDeliveriesMade = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Delivered);

                    return allPaymentsPaid && allDeliveriesMade;
                },
                false);

            //from attention to Returned.
            //order can only be in returned, if all payments returned and deliveries returned.
            stateMachine.AddStateTransition(attention, returned,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from completed to returned. 

                    //order can only be returned if all payments are returned or cancelled.
                    var allPaymentsReturnedOrCancelled = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Returned ||
                        x.PaymentStatus == (short)PaymentStatus.Cancelled);

                    //all deliveries returned.
                    var allDeliveriesReturned = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Returned);

                    return allPaymentsReturnedOrCancelled && allDeliveriesReturned;
                },
                false);

            //from attention to Cancelled.
            //order can only go to cancelled state, if all payments returned and deliveries cancelled.
            stateMachine.AddStateTransition(attention, cancelled,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from completed to returned. 

                    //order can only go to cancelled state if all payments are returned or cancelled.
                    var allPaymentsReturnedOrCancelled = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Returned ||
                        x.PaymentStatus == (short)PaymentStatus.Cancelled);

                    //all deliveries returned.
                    var allDeliveriesCancelled = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Cancelled);

                    return allPaymentsReturnedOrCancelled && allDeliveriesCancelled;
                },
                false);

            //order can go to closedByAdmin state without any restrictions, and can be set by administrator.
            stateMachine.AddStateTransition(attention, closedByAdmin);
        }

        private static void TransitionsFromCompletedState(OrderStateMachine stateMachine, State<OrderCarrier> completed, State<OrderCarrier> returned, State<OrderCarrier> attention)
        {
            //From completed to returned.
            //An order can go to returned only if the payments are returned and all deliveries are returned.
            stateMachine.AddStateTransition(completed, returned,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from completed to returned. 

                    //order can only be returned if all payments are returned or cancelled.
                    var allPaymentsReturnedOrCancelled = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Returned ||
                        x.PaymentStatus == (short)PaymentStatus.Cancelled);

                    //all deliveries returned.
                    var allDeliveriesReturned = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Returned);

                    return allPaymentsReturnedOrCancelled && allDeliveriesReturned;
                },
                false);

            //From Completed to Attention.
            //Order goes from completed to attention if any delivery is returned., and payments are not returned.
            stateMachine.AddStateTransition(completed, attention,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from completed to attention. 

                    //order can only go to attention if any payment is not returned or cancelled.
                    var allPaymentsReturnedOrCancelled = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Returned ||
                        x.PaymentStatus == (short)PaymentStatus.Cancelled);

                    //all deliveries returned.
                    var allDeliveriesReturned = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Returned);

                    //order can only go to attention if any payment is not returned or cancelled.
                    return !allPaymentsReturnedOrCancelled && allDeliveriesReturned;
                },
                false);
        }

        /// <summary>
        ///     Transitionses the state of from confirmed.
        ///     Confirmed to Processing.
        ///     Confirmed to Cancelled.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="confirmed">The confirmed.</param>
        /// <param name="processing">The processing.</param>
        /// <param name="cancelled">The cancelled.</param>
        private static void TransitionsFromConfirmedState(OrderStateMachine stateMachine, State<OrderCarrier> confirmed, State<OrderCarrier> processing, State<OrderCarrier> cancelled)
        {
            //From Confirmed to Processing.
            //processing can start only if payment has started, and deliveries has started.
            stateMachine.AddStateTransition(confirmed, processing,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from confirmed to processing. 

                    //processing can start only if payment has started, and deliveries has started.
                    var allPaymentsStarted = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Paid ||
                        x.PaymentStatus == (short)PaymentStatus.Reserved ||
                        x.PaymentStatus == (short)PaymentStatus.Pending);
                    var allDeliveriesStarted = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus != (short)DeliveryState.Init);

                    return allPaymentsStarted && allDeliveriesStarted;
                },
                false);

            //From Confirmed to Cancelled.
            //An order is confirmed can only be cancelled only if payment are cancelled or returned, and deliveries are cancelled.
            stateMachine.AddStateTransition(confirmed, cancelled,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from confirmed to cancelled. 

                    //processing can start only if payment has started, and deliveries has started.
                    var allPaymentsReturned = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Cancelled ||
                        x.PaymentStatus == (short)PaymentStatus.Returned);

                    var allDeliveriesCancelled = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Cancelled);

                    return allPaymentsReturned && allDeliveriesCancelled;
                },
                false);
        }

        /// <summary>
        ///     Transitionses the state of from init.
        ///     Init to WaitingConfirmation.
        ///     Init to Cancelled.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="waitingConfirmation">The confirmed.</param>
        /// <param name="cancelled">The cancelled.</param>
        private static void TransitionsFromInitState(OrderStateMachine stateMachine, State<OrderCarrier> waitingConfirmation, State<OrderCarrier> cancelled)
        {
            //build state transitions.
            //From Init to Waiting confirmation.
            stateMachine.AddStateTransition(new State<OrderCarrier>((short)OrderState.Init, OrderState.Init.ToString()), waitingConfirmation, (orderCarrier, startState, endState, token) => orderCarrier.Type != OrderType.SalesReturnOrder, false);

            stateMachine.AddStateTransition(new State<OrderCarrier>((short)OrderState.Init, OrderState.Init.ToString()), cancelled,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        var allPaymentsPaidOrReserved = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting)
                            .All(x =>
                                x.PaymentStatus == (short)PaymentStatus.Paid ||
                                x.PaymentStatus == (short)PaymentStatus.Reserved);
                        return allPaymentsPaidOrReserved;
                    }
                    //Conditions for order from init to cancelled.
                    //order can be cancelled only if payments has not started., or all payments are cancelled.
                    //we consider states PaymentStatus.ExecuteCharge and PaymentStatus.ExecuteReserve both as Payments not started if there are no transaction reference.
                    var allPaymentsNotStarted = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Init ||
                        x.PaymentStatus == (short)PaymentStatus.ExecuteCharge && string.IsNullOrEmpty(x.TransactionReference) ||
                        x.PaymentStatus == (short)PaymentStatus.ExecuteReserve && string.IsNullOrEmpty(x.TransactionReference) ||
                        x.PaymentStatus == (short)PaymentStatus.Cancelled);

                    var allDeliveriesNotStarted = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Init);

                    return allPaymentsNotStarted && allDeliveriesNotStarted;
                },
                true);
        }

        private static void TransitionsFromProcessingState(OrderStateMachine stateMachine, State<OrderCarrier> processing, State<OrderCarrier> completed, State<OrderCarrier> attention, State<OrderCarrier> cancelled)
        {
            //From Processing to Cancelled.
            //An order in processing can only be cancelled only if payment are cancelled or returned, and deliveries are cancelled.
            stateMachine.AddStateTransition(processing, cancelled,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go to processing from cancelled. 

                    //processing can be cancelled only if payment has cancelled or returned, and deliveries has cancelled.
                    var allPaymentsReturned = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Cancelled ||
                        x.PaymentStatus == (short)PaymentStatus.Returned);

                    var allDeliveriesCancelled = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Cancelled);

                    return allPaymentsReturned && allDeliveriesCancelled;
                },
                false);

            //From Processing to Completed.
            //An orer is completed only when all deliveries done, and payments paid.
            stateMachine.AddStateTransition(processing, completed,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from processing to completed. 

                    //order can only be completed if all payments paid and deliveries delivered.
                    var allPaymentsPaid = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Paid);

                    var allDeliveriesMade = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Delivered);

                    return allPaymentsPaid && allDeliveriesMade;
                },
                false);

            //From processing to Attention state.
            //An order can go to attention from processing only if delivery is cancelled and payment cancellation or return has failed.
            stateMachine.AddStateTransition(processing, attention,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to go from processing to attention. 

                    //An order can go to attention from processing only if delivery is cancelled and payment cancellation or return has failed.
                    var allPaymentReturned = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Cancelled ||
                        x.PaymentStatus == (short)PaymentStatus.Returned);

                    var anyDeliveryCancelled = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).Any(x =>
                        x.DeliveryStatus == (short)DeliveryState.Cancelled);

                    return !allPaymentReturned && anyDeliveryCancelled;
                },
                false);
        }

        /// <summary>
        ///     Transitionses from Waiting Confirmation.
        ///     Init to Confirmed.
        ///     Init to Cancelled.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="waitingConfirmation">The waiting confirmation.</param>
        /// <param name="confirmed">The confirmed.</param>
        /// <param name="cancelled">The cancelled.</param>
        private void TransitionsFromWaitingConfirmationState(OrderStateMachine stateMachine, State<OrderCarrier> waitingConfirmation, State<OrderCarrier> confirmed, State<OrderCarrier> cancelled)
        {
            //build state transitions.
            //From waitingConfirmation to Confirmed. Administrator is not allowed to Confirm orders, they are automatically confirmed when payment is done.
            stateMachine.AddStateTransition(waitingConfirmation, confirmed,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //Conditions for order to get confirmed. 
                    //order can only be confirmed if role order approver in B2B.
                    var hasApproverRole = HasApproverRole(orderCarrier, token);

                    //order is only confirmed if payment is paid, reserved or pending.
                    return hasApproverRole && orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                               x.PaymentStatus == (short)PaymentStatus.Paid ||
                               x.PaymentStatus == (short)PaymentStatus.Reserved ||
                               x.PaymentStatus == (short)PaymentStatus.Pending);
                },
                false, 
                (orderCarrier, currentState, previousState, token) =>
                {
                    //Notify all webhook subscribers for order confirmed.
                    _eventBroker.Publish(new OrderConfirmed(orderCarrier.ID, () => orderCarrier.MapTo<Order>()));
                });
            //From waitingConfirmation to Cancelled.
            stateMachine.AddStateTransition(waitingConfirmation, cancelled,
                (orderCarrier, startState, endState, token) =>
                {
                    if (orderCarrier.Type == OrderType.SalesReturnOrder)
                    {
                        return false;
                    }
                    //order can be cancelled only if payments has not started., or all payments are cancelled or returned.
                    //we consider states PaymentStatus.ExecuteCharge and PaymentStatus.ExecuteReserve both as Payments not started if there are no transaction reference.
                    var allPaymentsNotStarted = orderCarrier.PaymentInfo.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.PaymentStatus == (short)PaymentStatus.Init ||
                        x.PaymentStatus == (short)PaymentStatus.ExecuteCharge && string.IsNullOrEmpty(x.TransactionReference) ||
                        x.PaymentStatus == (short)PaymentStatus.ExecuteReserve && string.IsNullOrEmpty(x.TransactionReference) ||
                        x.PaymentStatus == (short)PaymentStatus.Cancelled ||
                        x.PaymentStatus == (short)PaymentStatus.Returned
                    );

                    //Conditions for order from init to cancelled., order can only be cancelled if deliveries have not yet started.
                    var allDeliveriesNotStarted = orderCarrier.Deliveries.Where(y => !y.CarrierState.IsMarkedForDeleting).All(x =>
                        x.DeliveryStatus == (short)DeliveryState.Init || x.DeliveryStatus == (short)DeliveryState.Cancelled);
                    return allPaymentsNotStarted && allDeliveriesNotStarted;
                },
                true);
        }
    }
}
