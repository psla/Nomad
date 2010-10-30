using System;

namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    /// Provides means for publishing events, where listeners know little or nothing about publishers. Events are dispatched based on event type, not based on origin of the event
    /// </summary>
    public interface IEventAggregator
    {
        ///<summary>
        /// Subscribes for events of specific type
        ///</summary>
        ///<param name="action">Action to invoke when specific event is sent</param>
        ///<typeparam name="T">type of message we want to listen</typeparam>
        IEventAggregatorTicket<T> Subscribe<T>(Action<T> action) where T : class;


        /// <summary>
        /// Unsubsribes specified action. 
        /// </summary>
        /// <remarks>Be carefull when using lambda! 
        /// Two same lambdas may not be equal. Use method group instead.
        /// </remarks>
        /// <typeparam name="T">type of message we are stopping to listen</typeparam>
        /// <param name="ticket"></param>
        void Unsubscribe<T>(IEventAggregatorTicket<T> ticket) where T : class;


        /// <summary>
        /// Notifies all subscribed members about passes <paramref name="message"/>
        /// </summary>
        /// <typeparam name="T">Type of message to send</typeparam>
        /// <param name="message">Message to send</param>
        void Publish<T>(T message) where T : class;
    }
}