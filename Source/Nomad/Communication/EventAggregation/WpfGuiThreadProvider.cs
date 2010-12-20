using System;
using System.Windows.Threading;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Uses wpf dispatcher tu run <see cref="Delegate"/> in gui thread
    ///</summary>
    public class WpfGuiThreadProvider : IGuiThreadProvider{
        private Dispatcher _dispatcher;


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

        public void RunInGui(Delegate @delegate)
        {
            if (_dispatcher.CheckAccess())
                @delegate.DynamicInvoke(null);
            else
                _dispatcher.Invoke(@delegate);
        }
    }
}