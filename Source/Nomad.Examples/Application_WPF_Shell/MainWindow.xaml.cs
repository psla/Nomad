using System.Threading;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Regions;

namespace Application_WPF_Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IEventAggregator _aggregator;
        private readonly RegionManager _regionManager;


        public MainWindow(IServiceLocator locator, IEventAggregator aggregator,
                          ManualResetEvent resetEvent, RegionManager regionManager)
        {
            _aggregator = aggregator;
            _regionManager = regionManager;
            InitializeComponent();

            resetEvent.Set();

            AddButtonsForLanguage();
        }


        private void AddButtonsForLanguage()
        {
            IRegion region = _regionManager.GetRegion("leftSideMenu");
            region.AddView(new LanguageSelecting(_aggregator));
        }
    }
}