using System.Threading;
using System.Windows.Controls;
using Nomad.Communication.EventAggregation;
using Nomad.Communication.ServiceLocation;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Regions;

namespace WpfButtonModule
{
    public class WpfButtonModuleBootstraper : IModuleBootstraper

    {
        private readonly IEventAggregator _aggregator;
        private readonly IServiceLocator _locator;


        public WpfButtonModuleBootstraper(IServiceLocator locator, IEventAggregator aggregator)
        {
            _locator = locator;
            _aggregator = aggregator;
        }

        #region IModuleBootstraper Members

        public void OnLoad()
        {
            _aggregator.Subscribe<NomadAllModulesLoadedMessage>(AllModulesLoaded);
        }


        public void OnUnLoad()
        {
        }

        #endregion

        private void AllModulesLoaded(NomadAllModulesLoadedMessage obj)
        {
            var threadProvider = _locator.Resolve<IGuiThreadProvider>();
            threadProvider.RunInGui(AllModulesLoadedInGui);
        }

        private void AllModulesLoadedInGui()
        {
            var buttonControl = new ButtonControl(_locator);
            var regionManager = _locator.Resolve<RegionManager>();
            IRegion region = regionManager.GetRegion("mainTab");
            var view = new TabItem
            {
                Header = "Dodawanie nowych tabów",
                Content = buttonControl
            };


            region.AddView(view);
            region.Activate(view);
        }
    }
}