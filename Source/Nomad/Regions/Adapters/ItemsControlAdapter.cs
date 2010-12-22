using System;
using System.Windows;
using System.Windows.Controls;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions.Adapters
{
    /// <summary>
    /// Region adapter for <see cref="ItemsControl"/> region host
    /// </summary>
    /// <remarks>
    /// Regions created using this adapter are <see cref="MultipleActiveViewRegion"/></remarks>
    public class ItemsControlAdapter : IRegionAdapter
    {
        /// <summary>
        /// ItemsControl instances are supported with this adapter
        /// </summary>
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
            var region = new MultipleActiveViewRegion(); 

            new SynchronizeItemsBehavior().Attach(region, itemsControl);
            new ActiveAwareBehavior().Attach(region, itemsControl);
            return region;
        }
    }
}