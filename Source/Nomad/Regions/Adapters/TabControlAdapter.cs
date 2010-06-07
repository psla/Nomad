using System;
using System.Windows;
using System.Windows.Controls;

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
            return new SingleActiveViewRegion();
        }
    }
}