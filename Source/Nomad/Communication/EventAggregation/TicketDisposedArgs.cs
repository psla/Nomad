using System;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Contains information about ticket which was disposed
    ///</summary>
    public class TicketDisposedArgs : EventArgs
    {
        public TicketDisposedArgs()
        {
            
        }

        ///<summary>
        /// Creates ticket disposed event arguments
        ///</summary>
        ///<param name="eventType">contains type of the event which was disposed</param>
        public TicketDisposedArgs(Type eventType)
        {
            EventType = eventType;
        }


        ///<summary>
        /// Type of the event the ticket stops to listen
        ///</summary>
        public Type EventType { get; private set; }
    }
}