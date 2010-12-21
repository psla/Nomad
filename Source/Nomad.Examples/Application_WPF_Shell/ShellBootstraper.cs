using System.Threading;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
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
            _app = new App();
            var guiThreadProvider = (new WpfGuiThreadProvider(_app.Dispatcher));
            RegionManager regionManager = null;
            guiThreadProvider.RunInGui((ThreadStart) delegate
                                                         {
                                                             regionManager =
                                                                 new RegionManager(
                                                                     new RegionFactory(
                                                                         GetRegionAdapters()));
                                                         });

            _locator.Register<IGuiThreadProvider>(guiThreadProvider);
            _locator.Register(regionManager);

            _app.Run(new MainWindow(_locator, _aggregator, _resetEvent));
        }


        private static IRegionAdapter[] GetRegionAdapters()
        {
            return new IRegionAdapter[]
                       {
                           new ItemsControlAdapter(),
                           new TabControlAdapter()
                       };
        }
    }
}