using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using FunctionalTests.TestApp;
using NUnit.Framework;
using White.Core.UIItems;
using Application = White.Core.Application;
using Window = White.Core.UIItems.WindowItems.Window;

namespace FunctionalTests.FunctionalTests
{
    [TestFixture]
    public class InProcessRunner
    {
        private App _app;
        private Application _whiteApp;
        private Window _windowAutomation;
        private Dispatcher _dispatcher;
        private TestApp.Window _window;

        [TestFixtureSetUp]
        public void run_application()
        {
            var waitForApplicationStart = new ManualResetEvent(false);
            
            var thread = new Thread(() =>
                                        {
                                            _app = new App();
                                            _app.Startup += (s, e) =>
                                                                {
                                                                    waitForApplicationStart.Set();
                                                                };
                                            _app.Run();
                                        });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            waitForApplicationStart.WaitOne();

            _dispatcher = Dispatcher.FromThread(thread);
            _dispatcher.Invoke(new ThreadStart(() => _window = _app.MainWindow as TestApp.Window));

            _whiteApp = Application.Attach(Process.GetCurrentProcess());
            _windowAutomation = _whiteApp.GetWindow("MainWindow");
        }

        [Test]
        public void can_interact_with_the_window()
        {
            var button = _windowAutomation.Get<Button>("PleaseClickMe");
            button.Click();;
            
            Assert.IsTrue(_window.HasBeenClicked);
        }

        [Test]
        public void can_send_data_to_the_window()
        {
            _dispatcher.Invoke(new ThreadStart(() => _window.Text = "Lorem ipsum"));

            var textBox = _windowAutomation.Get<WPFLabel>("PleaseReadMe");
            Assert.AreEqual("Lorem ipsum", textBox.Text);
        }
    }
}