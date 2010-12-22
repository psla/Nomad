using System;
using System.Windows;
using System.Windows.Controls;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions.Adapters
{
    public class ToolbarTrayAdapter : IRegionAdapter
    {
        public Type SupportedType
        {
            get { return typeof (ToolBarTray); }
        }

        public IRegion AdaptView(DependencyObject regionHost)
        {
            if (regionHost == null) throw new ArgumentNullException("regionHost");
            if (!(regionHost is ToolBarTray))
                throw new InvalidOperationException(
                    "This adapter only supports regionHosts that are Toolbars");

            var tabControl = (ToolBarTray) regionHost;
            var region = new SingleActiveViewRegion();
            new SynchronizeToolbarsBehavior().Attach(region, tabControl);
            return region;
        }
    }
}