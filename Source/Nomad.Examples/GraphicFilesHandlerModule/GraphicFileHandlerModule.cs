using System;
using System.Windows;
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
        private readonly IServiceLocator _serviceLocator;
        private RegionManager _regionManager;


        public GraphicFileHandlerModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _eventAggregator = serviceLocator.Resolve<IEventAggregator>();
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            _eventAggregator.Subscribe<NomadAllModulesLoadedMessage>(AllModulesLoaded);
            _eventAggregator.Subscribe<FileLoaderMenuRegionRegisteredMessage>(NewMenu,
                                                                              DeliveryMethod.
                                                                                  GuiThread);
        }


        private void NewMenu(FileLoaderMenuRegionRegisteredMessage obj)
        {
            var region = _regionManager.GetRegion(obj.RegionName);
            var menuItem = new MenuItem() {Header = "About Graphic FileHandler"};
            menuItem.Click += (x, y) => MessageBox.Show("Piotr Ślatała, obsługa JPG i PNG, 1.0.2");
            region.AddView(menuItem);
        }


        public void OnUnLoad()
        {
        }

        #endregion

        private void AllModulesLoaded(NomadAllModulesLoadedMessage obj)
        {
            _eventAggregator.Subscribe<FileSelectedMessage>(FileSelected, DeliveryMethod.GuiThread);
            _regionManager = _serviceLocator.Resolve<RegionManager>();
        }


        private void FileSelected(FileSelectedMessage obj)
        {
            // this happens always in gui thread
            if (obj.FilePath.EndsWith("png") || obj.FilePath.EndsWith("jpg"))
            {
                var pp = new PicturePresenter(obj.FilePath);
                var tabItem = new TabItem {Header = obj.FilePath, Content = pp};
                IRegion region = _regionManager.GetRegion("mainTabs");
                region.AddView(tabItem);
                region.Activate(tabItem);
            }
        }
    }
}