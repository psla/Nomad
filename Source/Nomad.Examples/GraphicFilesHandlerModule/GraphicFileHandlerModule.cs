using System.Windows;
using System.Windows.Controls;
using FileLoaderModule;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;
using Nomad.Regions;

namespace GraphicFilesHandlerModule
{
    public class GraphicFileHandlerModule : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly RegionManager _regionManager;
        private GraphicFilesEditToolbar _graphicFilesEditToolbar;


        public GraphicFileHandlerModule(IEventAggregator eventAggregator, RegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            _eventAggregator.Subscribe<FileLoaderMenuRegionRegisteredMessage>(NewMenu, DeliveryMethod.GuiThread);
            _eventAggregator.Subscribe<FileSelectedMessage>(FileSelected, DeliveryMethod.GuiThread);

            _graphicFilesEditToolbar = new GraphicFilesEditToolbar();
            var region = _regionManager.GetRegion("toolbarTrayRegion");
            region.AddView(_graphicFilesEditToolbar);
        }


        public void OnUnLoad()
        {
        }

        #endregion


        private void NewMenu(FileLoaderMenuRegionRegisteredMessage obj)
        {
            var region = _regionManager.GetRegion(obj.RegionName);
            var menuItem = new MenuItem() { Header = "About Graphic FileHandler" };
            menuItem.Click += (x, y) => MessageBox.Show("Piotr Ślatała, obsługa JPG i PNG, 1.0.2");
            region.AddView(menuItem);
        }


        private void FileSelected(FileSelectedMessage obj)
        {
            // this happens always in gui thread
            if (obj.FilePath.EndsWith("png") || obj.FilePath.EndsWith("jpg"))
            {
                var pp = new PicturePresenter(obj.FilePath, _graphicFilesEditToolbar);
                //var tabItem = new TabItem {Header = obj.FilePath, Content = pp};
                IRegion region = _regionManager.GetRegion("mainTabs");
                region.AddView(pp);
                region.Activate(pp);
            }
        }
    }
}