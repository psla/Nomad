using System;
using System.Windows;
using System.Windows.Controls;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions.Adapters
{
    public class TabControlAdapter : IRegionAdapter
    {
        public Type SupportedType
        {
            get { return typeof (TabControl); }
        }


        public IRegion AdaptView(DependencyObject view)
        {
            var tabControl = view as TabControl;
            var region = new SingleActiveViewRegion();

            new SynchronizeItemsBehavior().Attach(region, tabControl);
            new SynchronizeActiveItemsBehavior().Attach(region, tabControl);
            return region;
        }
    }
}