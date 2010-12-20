using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Regions;

namespace WpfButtonModule
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ButtonControl : UserControl
    {
        private readonly IServiceLocator _locator;
        private int _id;


        public ButtonControl(IServiceLocator locator)
        {
            _locator = locator;
            _id = 0;
            InitializeComponent();
        }


        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var regionManager = _locator.Resolve<RegionManager>();
            IRegion region = regionManager.GetRegion("mainTab");
            var view = new TabItem
                           {
                               Header = "From button header",
                               Content =
                                   new Button
                                       {
                                           Content =
                                               string.Format("Content of from button header {0}",
                                                             _id)
                                       }
                           };
            var threadProvider = _locator.Resolve<IGuiThreadProvider>();
            threadProvider.RunInGui((ThreadStart) delegate
                                        {
                                            region.AddView(view);
                                            region.Activate(view);
                                        });
            _id++;
        }
    }
}
