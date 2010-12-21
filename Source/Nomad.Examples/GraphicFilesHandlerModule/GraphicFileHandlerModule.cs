using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using FileLoaderModule;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Regions;

namespace GraphicFilesHandlerModule
{
    public class GraphicFileHandlerModule : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ServiceLocator _serviceLocator;
        private RegionManager _regionManager;


        public GraphicFileHandlerModule(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = serviceLocator.Resolve<IEventAggregator>();
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
            // this happens always in gui thread
            if(obj.FilePath.EndsWith("png") || obj.FilePath.EndsWith("jpg"))
            {
                var pp = new PicturePresenter(obj.FilePath);
                var tabItem = new TabItem() {Header = obj.FilePath, Content = pp};
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
