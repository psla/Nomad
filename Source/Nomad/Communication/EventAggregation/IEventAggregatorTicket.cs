using System;

namespace Nomad.Communication.EventAggregation
{
    public interface IEventAggregatorTicket<T>
    {
        Action<T> Action { get;  }
    }
}