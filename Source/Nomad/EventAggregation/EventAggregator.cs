using System;
using System.Collections.Generic;

namespace Nomad.EventAggregation
{
    public class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Type, Delegate> _dictionary =
            new Dictionary<Type, Delegate>();

        #region Implementation of IEventAggregator

        /// <summary>
        /// Adds action for execution.
        /// <see cref="IEventAggregator.Subscribe{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void Subscribe<T>(Action<T> action) where T : class
        {
            Type type = typeof (T);
            Delegate events = null;
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(type, out events))
                {
                    //events.Add(action);
                    _dictionary[type] = Delegate.Combine(events, action);
                }
                else
                {
                    _dictionary[type] = action;
                }
            }
        }


        //TODO: Unsubsribing new lambda won't work!
        /// <summary>
        /// Removes event from collection. Thread safe.
        /// <see cref="IEventAggregator.Unsubsribe{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <exception cref="KeyNotFoundException">when unsubscribing from type which was no subsription ever</exception>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Unsubsribe<T>(Action<T> action) where T : class
        {
            Delegate actions = null;
            var type = typeof (T);
            //two parts of methods prevents stopping another thread for waiting to the end of the lock
            lock (_dictionary)
            {
                _dictionary[type] = Delegate.Remove(_dictionary[type], action);
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
            Delegate actions;

            //dictionary implementation may enter race condition if tryget is not in critical section
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(typeof (T), out actions))
                {
                    if(actions != null)
                        actions.DynamicInvoke(message);
                }
            }
        }

        #endregion
    }
}