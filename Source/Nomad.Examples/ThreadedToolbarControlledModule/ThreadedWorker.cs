using Nomad.Communication.EventAggregation;
using Nomad.Modules;
using Nomad.Regions;


namespace ThreadedToolbarControlledModule
{
    public class ThreadedWorker : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly RegionManager _regionManager;


        public ThreadedWorker(RegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }


        public void OnLoad()
        {
            var region = _regionManager.GetRegion("toolbarTrayRegion");
            region.AddView(new ThreadedToolbarPanel(_eventAggregator));
        }


        public void OnUnLoad()
        {
        }
    }
}