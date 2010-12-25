using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Regions;

namespace FileLoaderModule
{
    public class FileLoaderModule : IModuleBootstraper
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly EventAggregator _eventAggregator;


        public FileLoaderModule(EventAggregator eventAggregator, IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = eventAggregator;
        }


        public void OnLoad()
        {
            _eventAggregator.Subscribe<NomadAllModulesLoadedMessage>(AllModulesLoaded, DeliveryMethod.GuiThread);
        }


        private void AllModulesLoaded(NomadAllModulesLoadedMessage obj)
        {
            var regionManager = _serviceLocator.Resolve<RegionManager>();
            var region = regionManager.GetRegion("leftSideMenu");
            region.AddView(new SelectFileView(_serviceLocator, _eventAggregator));
        }


        public void OnUnLoad()
        {
        }
    }
}