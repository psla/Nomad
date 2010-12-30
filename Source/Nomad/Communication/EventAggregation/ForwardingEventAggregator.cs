using System;
using System.Collections.Generic;

namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    ///     Class for merging different event aggregators from various <see cref="AppDomain"/>
    /// </summary>
    /// <remarks>
    ///     Manages the calls between various domains to enable communication with serializable messages and forwards messages toward them.
    /// </remarks>
    public class ForwardingEventAggregator : IEventAggregator
    {
        /// <summary>
        /// to which all messages are being forwarded
        /// </summary>
        private readonly List<IEventAggregator> _eventAggregatorsToForwardTo =
            new List<IEventAggregator>();

        /// <summary>
        /// to which we are subsribing
        /// </summary>
        private readonly IEventAggregator _onSiteEventAggregator;


        ///<summary>
        ///     Initializes the instance of <see cref="ForwardingEventAggregator"/> class.
        ///</summary>
        ///<param name="proxiedEventAggregator">Implementation on of the remote <see cref="IEventAggregator"/>. May be <c>null</c></param>
        ///<param name="onSiteEventAggregator">Implementation of the local (this <see cref="AppDomain"/>) <see cref="IEventAggregator"/></param>
        public ForwardingEventAggregator(IEventAggregator proxiedEventAggregator,
                                         IEventAggregator onSiteEventAggregator)
        {
            _onSiteEventAggregator = onSiteEventAggregator;
            if (proxiedEventAggregator != null)
                _eventAggregatorsToForwardTo.Add(proxiedEventAggregator);
            _eventAggregatorsToForwardTo.Add(onSiteEventAggregator);
        }

        #region IEventAggregator Members

        /// <summary>
        /// Subsribes to local <see cref="IEventAggregator"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public IEventAggregatorTicket<T> Subscribe<T>(Action<T> action) where T : class
        {
            return Subscribe(action, DeliveryMethod.AnyThread);
        }


        public IEventAggregatorTicket<T> Subscribe<T>(Action<T> action,
                                                      DeliveryMethod deliveryMethod) where T : class
        {
            return _onSiteEventAggregator.Subscribe(action, deliveryMethod);
        }


        /// <summary>
        /// Publishes to all associated event aggregators.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Publish<T>(T message) where T : class
        {
            foreach (IEventAggregator eventAggregator in _eventAggregatorsToForwardTo)
                eventAggregator.Publish(message);
        }

        #endregion
    }
}