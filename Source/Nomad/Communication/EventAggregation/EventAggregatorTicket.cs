using System;
using System.Threading;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Standard event aggregation implementation, which allows to receive more information, such as target thread.
    ///</summary>
    ///<typeparam name="T">Type of event ticket concerns</typeparam>
    public class EventAggregatorTicket<T> : IEventAggregatorTicket<T> where T : class
    {
        private readonly IGuiThreadProvider _guiThreadProvider;


        /// <summary>
        /// Creates ticket for specified action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="deliveryMethod"></param>
        ///<param name="guiThreadProvider">implementation of gui thread</param>
        public EventAggregatorTicket(Action<T> action, DeliveryMethod deliveryMethod,
                                     IGuiThreadProvider guiThreadProvider)
        {
            _guiThreadProvider = guiThreadProvider;
            Action = action;
            DeliveryMethod = deliveryMethod;
            ActionType = typeof (T);
        }


        /// <summary>
        /// Action which will be invoked, if the event occurs and ticket is active
        /// </summary>
        public Action<T> Action { get; private set; }

        #region IEventAggregatorTicket<T> Members

        ///<summary>
        /// Way to deliver the action
        ///</summary>
        public DeliveryMethod DeliveryMethod { get; private set; }


        public void Execute(T payload)
        {
            //TODO?: This may be replaced by specializations of EventAggregatorTicket & Factory of EventAggregatorTicket
            switch (DeliveryMethod)
            {
                case (DeliveryMethod.AnyThread):
                    Action(payload);
                    break;
                case (DeliveryMethod.GuiThread):
                    // m_guiThreadExecute
                    _guiThreadProvider.RunInGui((ThreadStart) (() => Action(payload)));
                    break;
                case (DeliveryMethod.SeparateThread):
                    ThreadPool.QueueUserWorkItem(delegate { Action(payload); });
                    break;
            }
        }


        public Type ActionType { get; private set; }


        public void Execute(object payload)
        {
            var typedPayload = payload as T;
            if (typedPayload == null)
                throw new ArgumentException(
                    "EventAggregator payload couldn't be casted to proper type");

            Execute(typedPayload);
        }


        ///<summary>
        /// Invoked when <see cref="Dispose"/> was finished
        ///</summary>
        public event EventHandler<TicketDisposedArgs> TicketDisposed;

        #endregion

        public void InvokeTicketDisposed(TicketDisposedArgs e)
        {
            EventHandler<TicketDisposedArgs> handler = TicketDisposed;
            if (handler != null) handler(this, e);
        }

        #region == IDisposable ==

        private bool _disposed;

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            //TODO: Is it ok to deliver message which was enqeueued before disposal?
            if (!_disposed)
            {
                if (disposing)
                {
                    InvokeTicketDisposed(new TicketDisposedArgs());
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
            }
        }


        // Use C# destructor syntax for finalization code.
        ~EventAggregatorTicket()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion
    }
}