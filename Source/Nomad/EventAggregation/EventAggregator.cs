using System;
using System.Collections.Generic;

namespace Nomad.EventAggregation
{
    public class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Type, IList<object>> _dictionary =
            new Dictionary<Type, IList<object>>();

        #region Implementation of IEventAggregator

        /// <summary>
        /// Adds action for execution.
        /// <see cref="IEventAggregator.Subscribe{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void Subscribe<T>(Action<T> action) where T : class
        {
            //_action = action;
            Type type = typeof (T);
            IList<object> events = null;
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(type, out events))
                {
                    events.Add(action);
                }
                else
                {
                    _dictionary[type] = new List<object> {action};
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
        public void Unsubsribe<T>(Action<T> action) where T : class
        {
            IList<object> actions = null;

            //two parts of methods prevents stopping another thread for waiting to the end of the lock
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(typeof (T), out actions))
                {
                    actions.Remove(action);
                }
            }
        }


        /// <summary>
        /// Notifies event listeners. Thread safe.
        /// <see cref="IEventAggregator.Notify{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Notify<T>(T message)
        {
            IList<object> actions = null;
            IList<object> actionsToExecute = new List<object>();

            //two parts of methods prevents stopping another thread for waiting to the end of the lock
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(typeof (T), out actions))
                {
                    actionsToExecute = new List<object>(actions);
                }
            }

            foreach (object action in actionsToExecute)
            {
                var act = action as Action<T>;
                if (act != null)
                    act(message);
            }
        }

        #endregion
    }
}