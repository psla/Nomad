using System;

namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    ///     Class for merging different event aggregators from various <see cref="AppDomain"/>
    /// </summary>
    /// <remarks>
    ///     Manages the calls between various domains to enable communication with serializable messages.
    /// </remarks>
    public class EventAggregatorFacade : IEventAggregator
    {
        private readonly IEventAggregator _onSiteEventAggregator;
        private readonly IEventAggregator _proxiedEventAggregator;


        ///<summary>
        ///     Initializes the instance of <see cref="EventAggregatorFacade"/> class.
        ///</summary>
        /// <remarks>
        ///     Default <see cref="Mode"/> is set to <see cref="EventAggregatorMode.AllDomain"/>.
        /// </remarks>
        ///<param name="proxiedEventAggregator">Implementation on of the remote <see cref="IEventAggregator"/></param>
        ///<param name="onSiteEventAggregator">Implementation of the local (this <see cref="AppDomain"/>) <see cref="IEventAggregator"/></param>
        public EventAggregatorFacade(IEventAggregator proxiedEventAggregator,
                                     IEventAggregator onSiteEventAggregator)
        {
            _proxiedEventAggregator = proxiedEventAggregator;
            _onSiteEventAggregator = onSiteEventAggregator;
            
            // set default mode of working
            Mode = EventAggregatorMode.AllDomain;
        }

        #region IEventAggregator Members

        public EventAggregatorMode Mode { get; set; }


        public IEventAggregatorTicket<T> Subscribe<T>(Action<T> action) where T : class
        {
            return Subscribe(action, DeliveryMethod.AnyThread);
        }


        public IEventAggregatorTicket<T> Subscribe<T>(Action<T> action,
                                                      DeliveryMethod deliveryMethod) where T : class
        {
            return _onSiteEventAggregator.Subscribe(action, deliveryMethod);
        }


        public void Publish<T>(T message) where T : class
        {
            if ((Mode == EventAggregatorMode.MyDomain) || Mode == EventAggregatorMode.AllDomain)
                _onSiteEventAggregator.Publish(message);

            if ((Mode == EventAggregatorMode.OtherDomain) || (Mode == EventAggregatorMode.AllDomain))
                _proxiedEventAggregator.Publish(message);
        }

        #endregion
    }
}