using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Regions;

namespace ListingAllLoadedModule
{
    public class ListingAllLoadedModule : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceLocator _serviceLocator;
        private readonly RegionManager _regionManager;


        public ListingAllLoadedModule(IServiceLocator serviceLocator, IEventAggregator eventAggregator, RegionManager regionManager)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            _eventAggregator.Subscribe<NomadAllModulesLoadedMessage>(AllModulesLoaded, DeliveryMethod.GuiThread);
        }


        public void OnUnLoad()
        {
        }

        #endregion

        private void AllModulesLoaded(NomadAllModulesLoadedMessage obj)
        {
            var listingControl = new ListingControl(_serviceLocator);
            var region = _regionManager.GetRegion("rightSideMenu");
            region.AddView(listingControl);
        }
    }
}