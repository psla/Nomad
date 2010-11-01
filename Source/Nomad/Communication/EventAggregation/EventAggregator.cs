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
        /*private readonly IGuiThreadProvider _guiThreadProvider;

        private readonly IDictionary<DeliveryMethod, IDictionary<Type, Delegate>> _subscriptions =
            new Dictionary<DeliveryMethod, IDictionary<Type, Delegate>>();
        */
        /*private readonly IDictionary<Type, Delegate> _guiThreadSubscriptions =
            new Dictionary<Type, Delegate>();

        private readonly IDictionary<Type, Delegate> _anyThreadSubscriptions =
            new Dictionary<Type, Delegate>();*/
        public IDictionary<Type, HashSet<IEventAggregatorTicket>> _subscriptions = new Dictionary<Type, HashSet<IEventAggregatorTicket>>();

        ///<summary>
        /// Initializes <see cref="EventAggregator"/> with provided <see cref="guiThreadProvider"/>.
        ///</summary>
        
        public EventAggregator(IGuiThreadProvider guiThreadProvider)
        {
            _guiThreadProvider = guiThreadProvider;
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
            var ticket = new EventAggregatorTicket<T>(action, deliveryMethod, _guiThreadProvider);
            HashSet<IEventAggregatorTicket> tickets = null;
            lock (_subscriptions)
            {
                if(!_subscriptions.TryGetValue(type, out tickets))
                {
                    tickets = new HashSet<IEventAggregatorTicket>();
                    _subscriptions[type] = tickets;
                }
            }

            lock (tickets)
            {
                tickets.Add(ticket);
            }

            return ticket;
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
            HashSet<IEventAggregatorTicket> tickets = null;
            lock(_subscriptions)
            {
                tickets = _subscriptions[type];
            }
            lock (tickets)
            {
                tickets.Remove(ticket);
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
            Type type = typeof (T);
            HashSet<IEventAggregatorTicket> tickets = null;
            lock (_subscriptions)
            {
                _subscriptions.TryGetValue(type, out tickets);
            }

            //prevention from throwing exception
            if (tickets == null)
                return;

            List<IEventAggregatorTicket> ticketsList = null;
            lock (tickets)
            {
                ticketsList = new List<IEventAggregatorTicket>(tickets);
            }

            foreach (var ticket in ticketsList)
            {
                ticket.Execute(message);
            }

        }

        #endregion
    }
}