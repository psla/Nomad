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
        ///<typeparam name="T">type of communicate which is sent</typeparam>
        void Subscribe<T>(Action<T> action) where T : class;


        /// <summary>
        /// Unsubsribes delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        void Unsubsribe<T>(Action<T> action) where T : class;
        //TODO: Is it possible to unsubscribe delegate? it will be another reference :/


        /// <summary>
        /// Notifies all subscribed members about passes <see cref="message"/>
        /// </summary>
        /// <typeparam name="T">Type of message to send</typeparam>
        /// <param name="message">Message to send</param>
        void Notify<T>(T message);
    }
}