using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nomad.Communication.ServiceLocation;
using Nomad.Services;

namespace ListingAllLoadedModule
{
    /// <summary>
    ///     Interaction logic for ListingControl.xaml
    /// </summary>
    public partial class ListingControl : UserControl
    {
        private readonly IServiceLocator _serviceLocator;

        public IList<string > GetListOfModules
        {
            get
            {
                var list = _serviceLocator.Resolve<ILoadedModulesService>().GetLoadedModules();

                return
                    new List<string>(
                        list.Select(x => x.Manifest.ModuleName + x.Manifest.ModuleVersion));
            }
        }

        public ListingControl(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            InitializeComponent();
            DataContext = this;
        }
    }
}
