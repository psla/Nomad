using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NUnit.Framework;
using WhiteWindow = White.Core.UIItems.WindowItems.Window;

namespace TestsShared.FunctionalTests
{
    /// <summary>
    ///     Provides easy way to run in-process gui functional tests for any window
    /// </summary>
    /// <typeparam name="T">
    ///     Type of window to be run. Must have public parameterless constructor.
    /// </typeparam>
    public class GuiTestFixture<T> where T : Window, new()
    {
        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     Gets window under test
        /// </summary>
        public T Window { get; private set; }

        /// <summary>
        ///     Get White's automation wrapper for window under test
        /// </summary>
        public WhiteWindow WhiteWindow { get; private set; }

        /// <summary>
        ///     Gets dispatcher used by application
        /// </summary>
        public Dispatcher Dispatcher
        {
            get { return Window.Dispatcher; }
        }


        /// <summary>
        ///     Runs fake application in new thread and gets basic
        ///     automation wrappers.
        /// </summary>
        [TestFixtureSetUp]
        public void Run()
        {
            Window window;
            WhiteWindow whiteWindow;
            GuiApplicationContainer.OpenWindow(() => new T(), out window, out whiteWindow);

            Window = (T) window;
            WhiteWindow = whiteWindow;
        }


        /// <summary>
        ///     Tries to stop application - closes the window and waits for confirmation.
        /// </summary>
        [TestFixtureTearDown]
        public void Stop()
        {
            Invoke(() => Window.Close());
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
    }
}