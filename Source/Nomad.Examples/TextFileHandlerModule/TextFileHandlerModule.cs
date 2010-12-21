using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileLoaderModule;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Regions;

namespace TextFileHandlerModule
{
    public class TextFileHandlerModule : IModuleBootstraper
    {
        private readonly IServiceLocator _serviceLocator;
        private IEventAggregator _eventAggregator;
        private RegionManager _regionManager;


        public TextFileHandlerModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = _serviceLocator.Resolve<IEventAggregator>();
        }



        public void OnLoad()
        {
            _eventAggregator.Subscribe<NomadAllModulesLoadedMessage>(AllModulesLoaded);
        }


        private void AllModulesLoaded(NomadAllModulesLoadedMessage obj)
        {
            _eventAggregator.Subscribe<FileSelectedMessage>(FileSelected, DeliveryMethod.GuiThread);
            _regionManager = _serviceLocator.Resolve<RegionManager>();
        }


        private void FileSelected(FileSelectedMessage obj)
        {
            if (obj.FilePath.EndsWith("txt"))
            {
                var pp = new TextPresenter(obj.FilePath);
                var tabItem = new TabItem() { Header = obj.FilePath, Content = pp };
                var region = _regionManager.GetRegion("mainTabs");
                region.AddView(tabItem);
                region.Activate(tabItem);
            }
        }


        public void OnUnLoad()
        {
        }
    }
}
