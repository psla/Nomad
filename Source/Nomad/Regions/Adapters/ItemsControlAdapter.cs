using System;
using System.Windows;
using System.Windows.Controls;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions.Adapters
{
    public class ItemsControlAdapter : IRegionAdapter
    {
        public Type SupportedType
        {
            get { return typeof (ItemsControl); }
        }

        public IRegion AdaptView(DependencyObject regionHost)
        {
            if (regionHost == null) throw new ArgumentNullException("regionHost");
            if (!(regionHost is ItemsControl))
                throw new InvalidOperationException(
                    "This adapter only supports regionHosts that are ItemsControls");

            var itemsControl = (ItemsControl)regionHost;
            var region = new SingleActiveViewRegion(); //TODO: Implement multiple active view region

            new SynchronizeItemsBehavior().Attach(region, itemsControl);
            new ActiveAwareBehavior().Attach(region, itemsControl);
            return region;
        }
    }
}