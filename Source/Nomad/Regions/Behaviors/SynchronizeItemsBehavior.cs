using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Nomad.Regions.Behaviors
{
    /// <summary>
    ///     Behavior that synchronizes <see cref="IRegion.Views"/> and <see cref="ItemsControl.Items"/>
    ///     properties.
    /// </summary>
    /// <remarks>
    ///     <see cref="SynchronizeItemsBehavior"/> binds <see cref="ItemsControl.ItemsSource"/> property
    ///     to views collection. Therefore, <see cref="ItemsControl.Items"/> property of region host 
    ///     must be empty before attaching a region to it.
    /// </remarks>
    public class SynchronizeItemsBehavior
    {
        private IRegion _source;
        private ItemsControl _control;


        public void Attach(IRegion region, DependencyObject regionHost)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionHost == null) throw new ArgumentNullException("regionHost");
            if (!(regionHost is ItemsControl))
                throw new InvalidOperationException(
                    "Only ItemsControl controls can be enchanced by this behavior");

            _control = (ItemsControl) regionHost;
            _source = region;

            var binding = new Binding("Views");
            binding.Source = region;

            BindingOperations.SetBinding(regionHost, ItemsControl.ItemsSourceProperty, binding);
        }
    }
}