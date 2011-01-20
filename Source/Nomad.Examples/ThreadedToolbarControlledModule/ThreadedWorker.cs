using System;
using System.Resources;
using Nomad.Communication.EventAggregation;
using Nomad.Internationalization;
using Nomad.Modules;
using Nomad.Regions;


namespace ThreadedToolbarControlledModule
{
    public class ThreadedWorker : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly RegionManager _regionManager;
        private ProgressBarHelper _progressBarHelper;


        public ThreadedWorker(RegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }


        public void OnLoad()
        {
            LoadResources(); 
            
            var statusBarRegion = _regionManager.GetRegion("statusBarRegion");
            var statusBar = new ThreadedStatusBar();
            statusBarRegion.AddView(statusBar);

            var region = _regionManager.GetRegion("toolbarTrayRegion");
            region.AddView(new ThreadedToolbarPanel(_eventAggregator, statusBar));

            var region2 = _regionManager.GetRegion("rightSideMenu");
            _progressBarHelper = new ProgressBarHelper(_eventAggregator);
            region2.AddView(_progressBarHelper.ProgressBar);
        }


        private void LoadResources()
        {
            var resourceProvider = ResourceProvider.CurrentResourceProvider;
            var assembly = GetType().Assembly;
            var resourceManager = new ResourceManager("ThreadedToolbarControlledModule.Resources.en-GB", assembly);
            var resourceManagerPl = new ResourceManager("ThreadedToolbarControlledModule.Resources.pl-PL", assembly);
            resourceProvider.AddSource("pl-PL", new ResourceManagerResourceSource(resourceManagerPl));
            resourceProvider.AddSource("en-GB", new ResourceManagerResourceSource(resourceManager));
        }


        public void OnUnLoad()
        {
            _progressBarHelper.Dispose();
        }
    }
}