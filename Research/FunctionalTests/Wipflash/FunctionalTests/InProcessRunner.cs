using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using NUnit.Framework;
using WiPFlash.Components;
using WipflashFunctionalTests.TestApp;
using Window = WipflashFunctionalTests.TestApp.Window;


namespace WipflashFunctionalTests.FunctionalTests
{
    [TestFixture]
    public class InProcessRunner
    {
        private App _app;
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
                                         _app = new App();
                                         _app.Startup += (s, e) => waitForApplicationStart.Set();
                                         _app.Run();
                                     });
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Name = "Application under testing";
            _thread.Start();
            waitForApplicationStart.WaitOne();

            _dispatcher = Dispatcher.FromThread(_thread);
            _dispatcher.Invoke(new ThreadStart(() => _window = _app.MainWindow as TestApp.Window));

            _application = new Application(Process.GetCurrentProcess());
            _windowAutomation = _application.FindWindow("MainWindow");
        }

        [TestFixtureTearDown]
        public void kill_application()
        {
            _dispatcher.Invoke(new ThreadStart(() => { _window.Close(); _app.Shutdown(0); }));
            Thread.Sleep(100);
            
        }

        [Test]
        public void can_interact_with_the_window()
        {
            _windowAutomation.Find<Button>("PleaseClickMe").Click();
            Thread.Sleep(100);
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