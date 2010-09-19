using System;

namespace Nomad.EventAggregation
{
    /// <summary>
    /// Responsible for passing events from the calee to the listeners
    /// </summary>
    public interface IEventAggregator
    {
        ///<summary>
        /// Subscribes for events of specific type
        ///</summary>
        ///<param name="action">Action to invoke when specific event is sent</param>
        ///<typeparam name="T">type of message we want to listen</typeparam>
        void Subscribe<T>(Action<T> action) where T : class;


        /// <summary>
        /// Unsubsribes specified action. Be carefull when using lambda! 
        /// Two same lambdas may not be equal. Use method group instead.
        /// </summary>
        /// <typeparam name="T">type of message we are stopping to listen</typeparam>
        /// <param name="action"></param>
        void Unsubsribe<T>(Action<T> action) where T : class;


        /// <summary>
        /// Notifies all subscribed members about passes <see cref="message"/>
        /// </summary>
        /// <typeparam name="T">Type of message to send</typeparam>
        /// <param name="message">Message to send</param>
        void Notify<T>(T message);
    }
}