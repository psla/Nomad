using System.Windows.Controls;
using FileLoaderModule;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;
using Nomad.Regions;

namespace TextFileHandlerModule
{
    public class TextFileHandlerModule : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly RegionManager _regionManager;


        public TextFileHandlerModule(IEventAggregator eventAggregator, RegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            _eventAggregator.Subscribe<FileSelectedMessage>(FileSelected, DeliveryMethod.GuiThread);
        }


        public void OnUnLoad()
        {
        }

        #endregion

        
        private void FileSelected(FileSelectedMessage obj)
        {
            if (obj.FilePath.EndsWith("txt"))
            {
                var pp = new TextPresenter(obj.FilePath);
                var tabItem = new TabItem {Header = obj.FilePath, Content = pp};
                var region = _regionManager.GetRegion("mainTabs");
                region.AddView(tabItem);
                region.Activate(tabItem);
            }
        }
    }
}