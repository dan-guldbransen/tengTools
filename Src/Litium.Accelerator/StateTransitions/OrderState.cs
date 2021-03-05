namespace Litium.Accelerator.StateTransitions
{
    /// <summary>
    ///     State of the order.
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        ///     When order is first created.
        /// </summary>
        Init = 0,
        /// <summary>
        ///     Order is placed, but not confirmed.
        /// </summary>
        WaitingConfirmation = 10,
        /// <summary>
        ///     When user confirms the order, all information should be correct by now.
        /// </summary>
        Confirmed = 1,
        /// <summary>
        ///     Order is dispatched, payments stated.
        /// </summary>
        Processing = 2,
        /// <summary>
        ///     Order is successfully Completed.
        /// </summary>
        Completed = 3,
        /// <summary>
        ///     Order is cancelled.
        /// </summary>
        Cancelled = 4,
        /// <summary>        
        ///     Order is returned.   
        /// </summary>
        Returned = 5,
        /// <summary>
        ///     Order needs administrative attention.
        /// </summary>
        Attention = 6,
        /// <summary>
        ///     Order is closed by administrator. Integration methods are not executed at the point of closing,
        ///     payment and delivery methods might not be according to accepted policy.
        /// </summary>
        ClosedByAdmin = 7,

        /// <summary>
        ///  Return items are confirmed by the ware house
        /// </summary>
        ReturnConfirmed = 21,

        /// <summary>
        ///  sales return order sent to ERP.
        ///   Start processing refunding payments
        /// </summary>
        ReturnProcessing = 22,

        /// <summary>
        ///  All payment processing completed.
        /// </summary>
        ReturnCompleted = 23,
        /// <summary>
        ///     Invalid order status code.
        /// </summary>
        Invalid = 99
    }
}
