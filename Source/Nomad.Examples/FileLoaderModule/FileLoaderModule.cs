using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Modules;
using Nomad.Regions;

namespace FileLoaderModule
{
    public class FileLoaderModule : IModuleBootstraper
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IEventAggregator _eventAggregator;
        private readonly RegionManager _regionManager;


        public FileLoaderModule(IEventAggregator eventAggregator, IServiceLocator serviceLocator, RegionManager regionManager)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }


        public void OnLoad()
        {
            var region = _regionManager.GetRegion("leftSideMenu");
            region.AddView(new SelectFileView(_serviceLocator, _eventAggregator));
        }


        public void OnUnLoad()
        {
        }
    }
}