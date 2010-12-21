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
using Nomad.Modules.Manifest;
using Nomad.Services;

namespace ListingAllLoadedModule
{
    /// <summary>
    ///     Interaction logic for ListingControl.xaml
    /// </summary>
    public partial class ListingControl : UserControl
    {
        private readonly IServiceLocator _serviceLocator;

        public IList<ModuleManifest > GetListOfModules
        {
            get
            {
                var list = _serviceLocator.Resolve<ILoadedModulesService>().GetLoadedModules();

                return new List<ModuleManifest>(list.Select( x => x.Manifest));
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
