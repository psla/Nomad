using System;
using System.Windows;
using System.Windows.Controls;

namespace Nomad.Regions.Behaviors
{
    public class TabControlHeaderTemplateSelectorBehavior : IRegionBehavior
    {
        public void Attach(IRegion region, DependencyObject regionHost)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionHost == null) throw new ArgumentNullException("regionHost");
            if (!(regionHost is TabControl))
                throw new InvalidOperationException("Only TabControl controls can be enchanced by this behavior");

            var tabControl = (TabControl)regionHost;

            tabControl.ItemTemplateSelector = new ItemsControlTemplateSelector();
        }
    }
}