using System;
using System.Collections;
using System.Collections.Generic;

namespace Nomad.EventAggregation
{
    public class EventAggregator : IEventAggregator
    {
        private IDictionary<Type, object> _dictionary = new Dictionary<Type, object>();

        #region Implementation of IEventAggregator

        public void Subscribe<T>(Action<T> action) where T : class
        {
            //_action = action;
            _dictionary[typeof (T)] = action;
        }


        public void Unsubsribe<T>(Action<T> action) where T : class
        {
            throw new NotImplementedException();
        }


        public void Notify<T>(T message)
        {
            object action=null;
            if(_dictionary.TryGetValue(typeof(T), out action))
            {
                var act = action as Action<T>;
                if (act != null) 
                    act(message);
            }
        }

        #endregion
    }
}