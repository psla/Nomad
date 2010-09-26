using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using White.Core.Factory;
using WhiteApplication = White.Core.Application;


namespace TestsShared.FunctionalTests
{
    /// <summary>
    ///     Responsible for mantaining lifetime of single wpf's application used 
    ///     for all ui-based tests.
    /// </summary>
    public static class GuiApplicationContainer
    {
        private static Thread _thread;
        private static Application _application;
        private static WhiteApplication _whiteApplication;

        /// <summary>
        ///     Gets dispatcher associated with application
        /// </summary>
        public static Dispatcher Dispatcher
        {
            get
            {
                if (_application == null)
                    throw new InvalidOperationException("Application is not running");
                return _application.Dispatcher;
            }
        }


        /// <summary>
        ///     Opens new window and attaches white's automation object to it.
        /// </summary>
        /// <param name="windowFactory">A factory method used to create new window</param>
        /// <param name="window">Returns created window</param>
        /// <param name="whiteWindow">Returns white's application for created window</param>
        public static void OpenWindow(Func<Window> windowFactory, out Window window,
                                      out White.Core.UIItems.WindowItems.Window whiteWindow)
        {
            var title = Guid.NewGuid().ToString();

            Window temporaryWindow = null;
            lock (typeof (GuiApplicationContainer))
            {
                EnsureApplicationIsRunning();
                Invoke(() =>
                           {
                               temporaryWindow = windowFactory();
                               temporaryWindow.Title = title;
                               temporaryWindow.Show();
                           });
            }

            window = temporaryWindow;
            whiteWindow = _whiteApplication.GetWindow(title, InitializeOption.NoCache);
            if (whiteWindow == null)
                throw new InvalidStateException("whiteWindow cannot be null");
        }

        /// <summary>
        ///     Shutdowns test application. 
        /// </summary>
        /// <remarks>
        ///     If the application is not running, calls to this method are ignored.
        /// </remarks>
        public static void ShutdownApplication()
        {
            lock (typeof (GuiApplicationContainer))
            {
                // no application is running
                if (_application == null)
                    return;

                Invoke(() => _application.Shutdown());
                _thread.Join(TimeSpan.FromSeconds(2));
                _application = null;
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

            _whiteApplication = WhiteApplication.Attach(Process.GetCurrentProcess());
        }


        private static void RunApplication(object userState)
        {
            var synchronization = (AutoResetEvent) userState;

            _application = new Application {ShutdownMode = ShutdownMode.OnExplicitShutdown};
            _application.Startup += (s, e) => synchronization.Set();
            _application.Run();
        }
    }

    internal class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message)
        {
            
        }
    }
}