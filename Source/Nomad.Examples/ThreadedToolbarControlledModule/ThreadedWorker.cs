using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Regions;


namespace ThreadedToolbarControlledModule
{
    public class ThreadedWorker : IModuleBootstraper
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly EventAggregator _eventAggregator;


        public ThreadedWorker(IServiceLocator serviceLocator, EventAggregator eventAggregator)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = eventAggregator;
        }


        public void OnLoad()
        {
            _eventAggregator.Subscribe<NomadAllModulesLoadedMessage>(AllModulesLoaded);
        }

        private void AllModulesLoaded(NomadAllModulesLoadedMessage obj)
        {
            //gui Runner
            var guiThread = _serviceLocator.Resolve<IGuiThreadProvider>();
            guiThread.RunInGui((ThreadStart)delegate
            {
                var regionManager =
                    _serviceLocator.Resolve<RegionManager>();
                var region =
                    regionManager.GetRegion("toolbarTrayRegion");
                region.AddView(
                    new ThreadedToolbarPanel());
            });

            //background Runner
        }


        public void OnUnLoad()
        {
            throw new NotImplementedException();
        }
    }
}
