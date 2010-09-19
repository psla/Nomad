using System;
using System.Collections;
using System.Collections.Generic;

namespace Nomad.EventAggregation
{
    public class EventAggregator : IEventAggregator
    {
        private IDictionary<Type, IList<object>> _dictionary = new Dictionary<Type, IList<object>>();

        #region Implementation of IEventAggregator

        public void Subscribe<T>(Action<T> action) where T : class
        {
            //_action = action;
            var type = typeof (T);
            IList<object> events = null;
            if(_dictionary.TryGetValue(type, out events))
            {
                events.Add(action);
            }
            else
            {
                _dictionary[type] = new List<object>() { action };
            }
        }


        public void Unsubsribe<T>(Action<T> action) where T : class
        {
            throw new NotImplementedException();
        }


        public void Notify<T>(T message)
        {
            IList<object> actions=null;
            if(_dictionary.TryGetValue(typeof(T), out actions))
            {
                foreach (var action in actions)
                {
                    var act = action as Action<T>;
                    if (act != null)
                        act(message);
                }
            }
        }

        #endregion
    }
}