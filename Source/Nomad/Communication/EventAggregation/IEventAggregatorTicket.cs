using System;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Contains necessary data about subscription ticket, it's status and all other necessary information
    ///</summary>
    /// <remarks>
    /// Instance of ticket is required to unsubscribe from <see cref="IEventAggregator"/>
    /// </remarks>
    ///<typeparam name="T">type of event you subscribed to</typeparam>
    public interface IEventAggregatorTicket<T>
    {
        ///<summary>
        /// Action which will be invoked, if the ticket is active
        ///</summary>
        Action<T> Action { get; }

        /// <summary>
        /// Thread to deliver action in
        /// </summary>
        DeliveryMethod DeliveryMethod { get; }
    }
}