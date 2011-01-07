using System.Windows.Controls;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;
using Nomad.Regions;
using Nomad.Updater;

namespace WpfUpdaterModule
{
    /// <summary>
    ///     Bootstrapping the updater module.  
    /// </summary>
    /// <remarks>
    ///     This class is specific to application, because of the region names.
    /// </remarks>
    public class UpdaterModuleBootstraper : IModuleBootstraper
    {
        private readonly RegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUpdater _updater;


        public UpdaterModuleBootstraper(IUpdater updater, RegionManager regionManager,IEventAggregator eventAggregator)
        {
            // use injected services from Nomad, instead of using IServiceLocator mechanism
            _updater = updater;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            // this only dependency on the host application
            IRegion region = _regionManager.GetRegion("mainTab");

            // create the view in the mvvm style
            var updaterView = new UpdaterControl
                                  {
                                      DataContext = new UpdaterViewModel(_updater,_eventAggregator)
                                  };

            // create the compatible view 
            var tabView = new TabItem()
                              {
                                  Header = "Updater",
                                  Content = updaterView
                              };

            // inject view into application
            region.AddView(tabView);
        }


        public void OnUnLoad()
        {
        }

        #endregion
    }
}