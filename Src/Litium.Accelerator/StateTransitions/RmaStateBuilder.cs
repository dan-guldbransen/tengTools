using System;
using System.Linq;
using Litium.Foundation;
using Litium.Foundation.Log;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Plugins.ReturnManagement;
using Litium.Foundation.Modules.ECommerce.StateTransitionSystem;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;

namespace Litium.Accelerator.StateTransitions
{
    [Service(ServiceType = typeof(RmaStateBuilder), Lifetime = DependencyLifetime.Transient)]
    public class RmaStateBuilder
    {
        private readonly ISalesReturnOrderFactory _salesReturnOrderBuilder;
        private readonly ISalesReturnOrderCalculator _salesReturnOrderCalculator;

        public RmaStateBuilder(ISalesReturnOrderFactory salesReturnOrderBuilder, ISalesReturnOrderCalculator salesReturnOrderCalculator)
        {
            _salesReturnOrderBuilder = salesReturnOrderBuilder;
            _salesReturnOrderCalculator = salesReturnOrderCalculator;
        }

        public virtual void Build(RmaStateMachine stateMachine)
        {
            //Init: When a RMA object is created, its status is Init
            var init = new State<RmaCarrier>(RmaState.Init, RmaState.Init.Name, (rmaCarrier, currentState, token) =>
                {
                    //Init processing entry action

                    //TODO: Integration code to execute, when start processing the rma.
                    //...
                },
                null);

            //PackageReceived: The items are received to the warehouse, but not yet inspected. 
            //                 In certain merchant scenarios, this means TA system as received the package even though the physical package has not yet received to the warehouse.
            var packageReceived = new State<RmaCarrier>(RmaState.PackageReceived, RmaState.PackageReceived.Name, (rmaCarrier, currentState, token) =>
                {
                    //PackageReceived processing entry action

                    //TODO: Integration code to execute, when start processing after the package is received.
                    //...
                },
                null);

            //Processing: Inspection, approval and inventory decision process. This may be a list of larger sub-processes
            //            depending on merchant scenarios.
            var processing = new State<RmaCarrier>(RmaState.Processing, RmaState.Processing.Name, (rmaCarrier, currentState, token) =>
                {
                    //Rma Processing  entry action

                    //TODO: Integration code to execute, when rma is in processing.
                    //...
                },
                null);

            //Completed: RMA (Return merchandice authorization) process is completed:
            //           Precondition: if the RMA is approved, the SalesReturnOrder exists in init state. (return process will continue from there)
            var complete = new State<RmaCarrier>(RmaState.Completed, RmaState.Completed.Name, (rmaCarrier, currentState, token) =>
                {
                    //Rma Completed entry action
                    //TODO: Integration code to execute, when rma is in completed state.
                },
                null);

            //add state transition.


            //from Init to PackageReceived
            stateMachine.AddStateTransition(init, packageReceived);

            //from PackageReceived to Processing
            stateMachine.AddStateTransition(packageReceived, processing);

            //from Processing to completed
            stateMachine.AddStateTransition(processing, complete, (rmaCarrier, startState, nextState, token) =>
            {
                //Condition for complete RMA : SRO should be created successfully before RMA's state become completed
                // Create corresponding SRO base on current Rma
                try
                {
                    var sroCarrier = _salesReturnOrderBuilder.Create(new SalesReturnOrderCreateArgs(rmaCarrier.Rma));
                    _salesReturnOrderCalculator.Calculate(sroCarrier);                    
                    var order = ModuleECommerce.Instance.Orders.CreateOrder(sroCarrier, token);                    
                    return true;
                }
                catch (Exception ex)
                {
                    var msg =$"Rma with id {rmaCarrier.Rma.SystemId} could not go to complete because of this error {ex.Message}.";
                    Solution.Instance.Log.CreateLogEntry("Could not complete Rma", msg, LogLevels.ERROR);
                    return false;
                }
            },true);



        }

    }
}
