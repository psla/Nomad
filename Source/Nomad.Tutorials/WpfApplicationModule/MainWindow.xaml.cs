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


        public MainWindow(IServiceLocator locator)
        {
            _locator = locator;
            InitializeComponent();

            var regionManager = locator.Resolve<RegionManager>();

            regionManager.AttachRegion("mainTab", OurTabcontrol);
        }


        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var regionManager = _locator.Resolve<RegionManager>();
            IRegion region = regionManager.GetRegion("mainTab");
            region.AddView(new TabItem
                               {
                                   Header = "Nasz headerek",
                                   Content = new Button {Content = "Nasz batonik"}
                               });
        }
    }
}