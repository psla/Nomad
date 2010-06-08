using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using WiPFlash.Components;

namespace TestsShared.FunctionalTests
{
    /// <summary>
    ///     Provides easy way to run in-process gui functional tests for any window
    /// </summary>
    /// <typeparam name="T">
    ///     Type of window to be run. Must have public parameterless constructor.
    /// </typeparam>
    public class GuiTestRunner<T> where T : System.Windows.Window, new()
    {
        private const string WindowName = "WindowUnderTest";
        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);

        private readonly ManualResetEvent _waitForApplication = new ManualResetEvent(false);

        /// <summary>
        ///     Gets window under test
        /// </summary>
        public T Window { get; private set; }

        /// <summary>
        ///     Gets automation wrapper for window under test
        /// </summary>
        public Window WindowAutomation { get; private set; }

        /// <summary>
        ///     Gets application under test
        /// </summary>
        public System.Windows.Application Application { get; private set; }

        /// <summary>
        ///     Gets automation wrapper for application under test
        /// </summary>
        public Application ApplicationAutomation { get; private set; }

        /// <summary>
        ///     Gets dispatcher used by application
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }


        /// <summary>
        ///     Runs fake application in new thread and gets basic
        ///     automation wrappers.
        /// </summary>
        public void Run()
        {
            // create application thread
            var thread = new Thread(RunApplicationUnderTest);
            thread.Name = "Tests runner for window of type " + typeof (T).Name;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            // wait until application has started
            if (!_waitForApplication.WaitOne(WaitTimeout))
            {
                var message = string.Format("Application didn't start in timespan of{0}",
                                            WaitTimeout);
                throw new TimeoutException(message);
            }

            // get dispatcher & automation objects
            Dispatcher = Dispatcher.FromThread(thread);
            ApplicationAutomation = new Application(Process.GetCurrentProcess(), WaitTimeout);
            WindowAutomation = ApplicationAutomation.FindWindow(WindowName);
        }


        /// <summary>
        ///     Tries to stop application - closes the window and waits for confirmation.
        /// </summary>
        public void Stop()
        {
            WindowAutomation.Close();
            WindowAutomation.WaitFor(wa => WindowAutomation.IsClosed());
        }


        /// <summary>
        ///     Invokes <paramref name="action"/> on application under test dispatcher thread,
        ///     with <see cref="DispatcherPriority.Normal">normal priority</see>.
        /// </summary>
        /// <remarks>
        ///     This method is synchronous. Once the caller regains control, 
        ///     the action has already been executed.
        /// </remarks>
        /// <param name="action">Action to be executed</param>
        /// <exception cref="ArgumentNullException">When <paramref name="action"/> is null</exception>
        public void Invoke(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            Dispatcher.Invoke(DispatcherPriority.Normal, action);
        }


        /// <summary>
        ///     Waits until input processing and data binding is done.
        /// </summary>
        public void Wait()
        {
            Dispatcher.Invoke(DispatcherPriority.Input,
                              WaitTimeout,
                              new ThreadStart(() => { }));
        }


        private void RunApplicationUnderTest()
        {
            Window = new T();
            Window.Name = WindowName;

            Application = new System.Windows.Application();
            Application.Startup += (s, e) => _waitForApplication.Set();
            Application.Run(Window);
        }
    }
}