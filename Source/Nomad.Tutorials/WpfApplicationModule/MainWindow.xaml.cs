using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Nomad.Communication.ServiceLocation;
using Nomad.Regions;

namespace WpfApplicationModule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceLocator _locator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="resetEvent">invoked after all region defined</param>
        public MainWindow(IServiceLocator locator, ManualResetEvent resetEvent)
        {
            _locator = locator;
            InitializeComponent();

            var regionManager = locator.Resolve<RegionManager>();

            regionManager.AttachRegion("mainTab", OurTabcontrol);
            resetEvent.Set();
        }


    }
}