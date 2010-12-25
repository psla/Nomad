using System;
using System.Windows;

namespace Nomad.Communication.EventAggregation
{
    public class LazyWpfGuiThreadProvider : IGuiThreadProvider
    {
        private readonly IGuiThreadProvider _fallBackProvider = new NullGuiThreadProvider();

        public void RunInGui(Delegate @delegate)
        {
            var wpfApplication = Application.Current;
            if(wpfApplication == null)
            {
                _fallBackProvider.RunInGui(@delegate);
                return;
            }

            var dispatcher = wpfApplication.Dispatcher;
            dispatcher.BeginInvoke(@delegate);
        }
    }
}