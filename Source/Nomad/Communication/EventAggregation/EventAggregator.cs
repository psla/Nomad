using System;
using System.Collections.Generic;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Provides implementation for <see cref="IEventAggregator"/> based on delegates
    ///</summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly IGuiThreadProvider _guiThreadProvider;

        private readonly IDictionary<DeliveryMethod, IDictionary<Type, Delegate>> _subscriptions =
            new Dictionary<DeliveryMethod, IDictionary<Type, Delegate>>();

        /*private readonly IDictionary<Type, Delegate> _guiThreadSubscriptions =
            new Dictionary<Type, Delegate>();

        private readonly IDictionary<Type, Delegate> _anyThreadSubscriptions =
            new Dictionary<Type, Delegate>();*/


        ///<summary>
        /// Initializes <see cref="EventAggregator"/> with provided <see cref="guiThreadProvider"/>.
        ///</summary>
        ///<param name="guiThreadProvider">implementation of gui thread</param>
        public EventAggregator(IGuiThreadProvider guiThreadProvider)
        {
            _guiThreadProvider = guiThreadProvider;
            foreach (object deliveryMethod in Enum.GetValues(typeof (DeliveryMethod)))
            {
                _subscriptions[(DeliveryMethod) deliveryMethod] = new Dictionary<Type, Delegate>();
            }
        }

        #region Implementation of IEventAggregator

        /// <summary>
        /// Adds action for execution.
        /// <see cref="IEventAggregator.Subscribe{T}(System.Action{T})"/>
        /// </summary>
        /// <remarks>
        /// Will be executed in any thread
        /// </remarks>
        /// <typeparam name="T">type of event to subsribe for</typeparam>
        /// <param name="action">action delegate to fire when type T delivered</param>
        public IEventAggregatorTicket<T> Subscribe<T>(Action<T> action) where T : class
        {
            return Subscribe(action, DeliveryMethod.AnyThread);
        }


        public IEventAggregatorTicket<T> Subscribe<T>(Action<T> action,
                                                      DeliveryMethod deliveryMethod) where T : class
        {
            Type type = typeof (T);
            Delegate events = null;
            lock (_subscriptions)
            {
                IDictionary<Type, Delegate> subsriptionsForDelivery = _subscriptions[deliveryMethod];
                if (subsriptionsForDelivery.TryGetValue(type, out events))
                {
                    subsriptionsForDelivery[type] = Delegate.Combine(events, action);
                }
                else
                {
                    subsriptionsForDelivery[type] = action;
                }
            }
            return new EventAggregatorTicket<T>(action, deliveryMethod);
        }


        //TODO: Unsubsribing new lambda won't work!
        /// <summary>
        /// Removes event from collection. Thread safe.
        /// <see cref="IEventAggregator.Unsubscribe{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ticket">ticket have to be <see cref="EventAggregatorTicket{T}"/></param>
        /// <exception cref="KeyNotFoundException">when unsubscribing from type which was no subsription ever</exception>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="ArgumentException">when ticket is not proper ticket</exception>
        public void Unsubscribe<T>(IEventAggregatorTicket<T> ticket) where T : class
        {
            Type type = typeof (T);
            var castedTicket = ticket as EventAggregatorTicket<T>;
            if (castedTicket == null)
                throw new ArgumentException();
            //two parts of methods prevents stopping another thread for waiting to the end of the lock
            IDictionary<Type, Delegate> subsriptionsForDelivery =
                _subscriptions[ticket.DeliveryMethod];
            lock (_subscriptions)
            {
                subsriptionsForDelivery[type] = Delegate.Remove(subsriptionsForDelivery[type],
                                                                ticket.Action);
            }
        }


        /// <summary>
        /// Notifies event listeners. Thread safe.
        /// <see cref="IEventAggregator.Publish{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Publish<T>(T message) where T : class
        {
            Delegate actionsInAnyThread;
            Delegate actionsInGui;
            //dictionary implementation may enter race condition if tryget is not in critical section
            lock (_subscriptions)
            {
                if (_subscriptions[DeliveryMethod.AnyThread].TryGetValue(typeof (T),
                                                                         out actionsInAnyThread))
                    ;

                if (_subscriptions[DeliveryMethod.GuiThread].TryGetValue(typeof (T),
                                                                         out actionsInGui))
                    ;
            }
            if (actionsInAnyThread != null)
                actionsInAnyThread.DynamicInvoke(message);
            if (actionsInGui != null)
                _guiThreadProvider.RunInGui(actionsInGui);
        }

        #endregion
    }
}