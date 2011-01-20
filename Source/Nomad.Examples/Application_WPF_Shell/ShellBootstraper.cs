using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Application_WPF_Shell.Resources;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Internationalization;
using Nomad.Messages;
using Nomad.Modules;
using Nomad.Regions;
using Nomad.Regions.Adapters;

namespace Application_WPF_Shell
{
    public class ShellBootstraper : IModuleBootstraper
    {
        private readonly IEventAggregator _aggregator;
        private readonly IServiceLocator _locator;

        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        private App _app;
        private Thread _guiThread;


        public ShellBootstraper(IServiceLocator locator, IEventAggregator aggregator)
        {
            _locator = locator;
            _aggregator = aggregator;
        }

        #region IModuleBootstraper Members

        /// <summary>
        ///     Start WPF shell
        /// </summary>
        public void OnLoad()
        {
            _guiThread = new Thread(StartApplication);
            _guiThread.SetApartmentState(ApartmentState.STA);
            _guiThread.Start();
            _resetEvent.WaitOne();
        }


        /// <summary>
        ///         Kill all threads when unload. Shutdown the application
        /// </summary>
        public void OnUnLoad()
        {
            // when killing appdomain thread.abort is allowed :]
            _guiThread.Abort();
        }

        #endregion

        private void StartApplication()
        {
            var regionManager = new RegionManager(new RegionFactory(GetRegionAdapters()));
            _locator.Register(regionManager);

            var resourceProvider = ResourceProvider.CurrentResourceProvider;
            //var resourceManager = new ResourceManager("Application_WPF_Shell", GetType().Assembly);
            var assembly = GetType().Assembly;
            
            var resourceManager = new ResourceManager("Application_WPF_Shell.Resources.en-GB",  assembly);
            var resourceManagerPL = new ResourceManager("Application_WPF_Shell.Resources.pl-PL",  assembly);
            resourceProvider.AddSource("pl-PL", new ResourceManagerResourceSource(resourceManagerPL));
            resourceProvider.AddSource("en-GB", new ResourceManagerResourceSource(resourceManager));

            
            _aggregator.Subscribe<NomadCultureChangedMessage>(x => resourceProvider.ChangeUiCulture(x.CurrentCulture));

            _app = new App();
            _app.Run(new MainWindow(_locator, _aggregator, _resetEvent, regionManager));
        }
        
        private static IRegionAdapter[] GetRegionAdapters()
        {
            return new IRegionAdapter[]
                       {
                           new ItemsControlAdapter(),
                           new TabControlAdapter(),
                           new ToolbarTrayAdapter()
                       };
        }
    }
}