namespace Litium.Accelerator.StateTransitions

{
    /// <summary>
    ///     set of user defined delivery states.
    /// </summary>
    public enum DeliveryState
    {
        /// <summary>
        ///     Initial state.
        /// </summary>
        Init = 0,
        /// <summary>
        ///     Delivery completed. Usually this means is that the delivery package is sent to distributer.
        ///     That is, package has left merchants warehouse.
        /// </summary>
        Delivered = 1,
        /// <summary>
        ///     Delivery failed. The distributer could not delivery the package.
        /// </summary>
        Failed = 2,
        /// <summary>
        ///     Delivery cancelled. Either customer or merchant cancelled delivery, before package is sent to distributer.
        /// </summary>
        Cancelled = 3,
        /// <summary>
        ///     Delivery is returned by the customer.
        /// </summary>
        Returned = 4,
        /// <summary>
        ///     Delivery is in processing. Package is being prepared to be sent to distributer.
        /// </summary>
        Processing = 5,
        /// <summary>
        ///     Package is ready to ship.
        /// </summary>
        ReadyToShip = 6,        
    }
}
