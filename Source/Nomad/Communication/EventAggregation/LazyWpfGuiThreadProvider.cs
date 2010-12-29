using System;
using System.Windows;

namespace Nomad.Communication.EventAggregation
{
    public class LazyWpfGuiThreadProvider : IGuiThreadProvider
    {
        private readonly IGuiThreadProvider _fallBackProvider = new NullGuiThreadProvider();

        public void RunInGui(Action action)
        {
            var wpfApplication = Application.Current;
            if(wpfApplication == null)
            {
                _fallBackProvider.RunInGui(action);
                return;
            }
            // here is the place where ensurance, that all actions in gui are serialized is broken.
            // there is a possibility that when application has run two actions will be invoked in parallel (one in nullguithreadprovider
            // and other one in wpf dispatcher!

            var dispatcher = wpfApplication.Dispatcher;
            dispatcher.BeginInvoke(action);
        }
    }
}