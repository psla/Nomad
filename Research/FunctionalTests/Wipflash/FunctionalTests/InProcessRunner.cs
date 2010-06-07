using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using NUnit.Framework;
using WiPFlash.Components;
using Window = WipflashFunctionalTests.TestApp.Window;


namespace WipflashFunctionalTests.FunctionalTests
{
    [TestFixture]
    public class InProcessRunner
    {
        private System.Windows.Application _app;
        private Dispatcher _dispatcher;
        private Window _window;
        private WiPFlash.Components.Window _windowAutomation;
        private Application _application;
        private Thread _thread;

        [TestFixtureSetUp]
        public void run_application()
        {
            var waitForApplicationStart = new ManualResetEvent(false);
            
            _thread = new Thread(() =>
                                     {
                                         _app = new System.Windows.Application();
                                         _app.Startup += (s, e) =>
                                                             {
                                                                 waitForApplicationStart.Set();
                                                                 _app.MainWindow = new Window();
                                                                 _app.MainWindow.ShowDialog();
                                                             };
                                         _app.Run();
                                     });
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Name = "Application under testing";
            _thread.Start();
            waitForApplicationStart.WaitOne();

            _dispatcher = Dispatcher.FromThread(_thread);
            _dispatcher.Invoke(new ThreadStart(() => _window = _app.MainWindow as TestApp.Window));

            _process = Process.GetCurrentProcess();
            _application = new Application(_process);
            _windowAutomation = _application.FindWindow("MainWindow");
          //  Thread.Sleep(TimeSpan.FromSeconds(20));
        }

        private Process _process;

        [TestFixtureTearDown]
        public void kill_application()
        {
            var watch = Stopwatch.StartNew();
            _dispatcher.Invoke(DispatcherPriority.SystemIdle, new ThreadStart(() => { _window.Close(); _app.Shutdown(0); }));
            _windowAutomation.WaitFor(c => _windowAutomation.IsClosed());
            watch.Stop();
            Console.WriteLine(watch.Elapsed.ToString());
        }

        [Test]
        public void can_interact_with_the_window()
        {
            _windowAutomation.Find<Button>("PleaseClickMe").Click();
            Thread.Sleep(100);
            Assert.IsTrue(_window.HasBeenClicked);
        }

        [Test]
        public void can_interact_with_the_window_and_wait_using_dispatcher()
        {
            _window.HasBeenClicked = false;
            _windowAutomation.Find<Button>("PleaseClickMe").Click();
            _dispatcher.Invoke(new ThreadStart(() => { }), DispatcherPriority.Input);
            Assert.IsTrue(_window.HasBeenClicked);
        }

        [Test]
        public void can_send_data_to_the_window()
        {
            _dispatcher.Invoke(new ThreadStart(() => _window.Text = "Lorem ipsum"));
            Thread.Sleep(100);

            var text = _windowAutomation.Find<Label>("PleaseReadMe").Text;
            Assert.AreEqual("Lorem ipsum", text);
        }
    }
}