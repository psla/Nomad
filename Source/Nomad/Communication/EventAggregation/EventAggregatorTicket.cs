using System;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Standard event aggregation implementation, which allows to receive more information, such as target thread.
    ///</summary>
    ///<typeparam name="T">Type of event ticket concerns</typeparam>
    public class EventAggregatorTicket<T> : IEventAggregatorTicket<T>
    {
        /// <summary>
        /// Creates ticket for specified action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="deliveryMethod"></param>
        public EventAggregatorTicket(Action<T> action, DeliveryMethod deliveryMethod)
        {
            Action = action;
            DeliveryMethod = deliveryMethod;
        }


        /// <summary>
        /// Action which will be invoked, if the event occurs and ticket is active
        /// </summary>
        public Action<T> Action { get; private set; }

        ///<summary>
        /// Way to deliver the action
        ///</summary>
        public DeliveryMethod DeliveryMethod { get; private set; }
    }
}