using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions.Adapters
{
    public class StatusBarAdapter : IRegionAdapter
    {
        public Type SupportedType
        {
            get { return typeof (StatusBar); }
        }

        public IRegion AdaptView(DependencyObject regionHost)
        {
            if (regionHost == null) throw new ArgumentNullException("regionHost");
            if (!(regionHost is StatusBar))
                throw new InvalidOperationException(
                    "This adapter only supports regionHosts that are StatusBars");

            var tabControl = (StatusBar)regionHost;
            var region = new SingleActiveViewRegion();
            new SynchronizeItemsBehavior().Attach(region, tabControl);
            return region;
        }
    }
}