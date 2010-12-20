using System;
using System.Threading;
using Nomad.Communication.ServiceLocation;
using Nomad.Modules;
using Nomad.Regions;
using Nomad.Regions.Adapters;

namespace WpfApplicationModule
{
    /// <summary>
    /// Responsible for running wpf application as a module
    /// </summary>
    public class WpfMainApplication : IModuleBootstraper
    {
        private readonly IServiceLocator _locator;
        private App _app;
        private Thread _thread;


        public WpfMainApplication(IServiceLocator locator)
        {
            _locator = locator;
            _locator.Register(new RegionManager(new RegionFactory(new IRegionAdapter[] { new TabControlAdapter()})));
        }

        #region IModuleBootstraper Members

        /// <summary>
        /// Start WPF application.
        /// </summary>
        public void OnLoad()
        {
            _thread = new Thread(StartApplication);
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        [STAThread]
        private void StartApplication()
        {
            _app = new App();

            _app.Run(new MainWindow(_locator));
        }


        /// <summary>
        /// Kill all threads when unload. Shutdown the application
        /// </summary>
        public void OnUnLoad()
        {
            // when killing appdomain thread.abort is allowed :]
            _thread.Abort();
        }

        #endregion
    }
}