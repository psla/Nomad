using System;
using System.Windows.Threading;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Uses wpf dispatcher tu run <see cref="Delegate"/> in gui thread
    ///</summary>
    public class WpfGuiThreadProvider : IGuiThreadProvider{
        private readonly Dispatcher _dispatcher;


        ///<summary>
        /// Initializes thread provider with current dispatcher
        ///</summary>
        ///<param name="dispatcher"></param>
        public WpfGuiThreadProvider()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            
        }
        /// <summary>
        /// responsible for providing provided dispatcher to all class that needs it
        /// </summary>
        /// <param name="dispatcher"></param>
        public WpfGuiThreadProvider(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void RunInGui(Action action)
        {
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.Invoke(action);
        }
    }
}