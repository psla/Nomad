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

namespace Application_WPF_Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(IServiceLocator locator, IEventAggregator aggregator, ManualResetEvent resetEvent)
        {
            InitializeComponent();

            
        }
    }
}
