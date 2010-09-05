using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace TestsShared.FunctionalTests
{
    public static class GuiApplicationContainer
    {
        private static Thread _thread;
        private static Application _application;

        public static Dispatcher Dispatcher
        {
            get { return _application.Dispatcher; }
        }


        public static Window OpenWindow(Func<Window> windowFactory)
        {
            Window window = null;

            lock (typeof (GuiApplicationContainer))
            {
                EnsureApplicationIsRunning();
                Invoke(() =>
                                {
                                    window = windowFactory();
                                    window.Show();
                                });
            }

            return window;
        }

        public static void ShutdownApplication()
        {
            lock (typeof(GuiApplicationContainer))
            {
                // no application is running
                if(_application == null)
                    return;

                Invoke(() => _application.Shutdown());
                _thread.Join(TimeSpan.FromSeconds(2));    
            }
        }

        private static void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private static void EnsureApplicationIsRunning()
        {
            if (_application != null)
                return;


            _thread = new Thread(RunApplication);
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Name = "Gui test runner thread";
            
            var synchronization = new AutoResetEvent(false);
            _thread.Start(synchronization);
            synchronization.WaitOne();
            synchronization.Close();
        }

        private static void RunApplication(object userState)
        {
            var synchronization = (AutoResetEvent) userState;

            _application = new Application {ShutdownMode = ShutdownMode.OnExplicitShutdown};
            _application.Startup += (s, e) => synchronization.Set();
            _application.Run();
        }
    }
}