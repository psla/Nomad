using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace Nomad.Regions.Behaviors
{
    /// <summary>
    ///     Behavior that allows <see cref="Selector"/> and <see cref="MultiSelector"/> controls
    ///     to synchronize active views with region they host.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Information about active views is synchronized two-way. When user selects or deselects view(s), they are
    ///     activated or deactivated in region that is hosted by the selector. When region's <see cref="IRegion.ActiveViews"/>
    ///     collection changes, <see cref="Selector.SelectedItem"/> (for <see cref="Selector"/> controls) 
    ///     or <see cref="MultiSelector.SelectedItems"/> (for <see cref="MultiSelector"/>) property are adjusted to reflect 
    ///     new selection.
    /// </para> 
    /// <para>
    ///     If behavior is attached so that region supports only one active view (e.g. <see cref="SingleActiveViewRegion"/>)
    ///     and regionHost is a <see cref="MultiSelector"/> it is indeterminate which view will become active in <see cref="IRegion"/>. 
    ///     It is not recommended to create such configuration.
    /// </para>
    /// <para>
    ///     If behavior is attached so that region supports multiple active views and regionHost is not a <see cref="MultiSelector"/>
    ///     it is indeterminate which view will be selected after multiple items are activated in <see cref="IRegion"/>.
    ///     It is not recommended to create such configuration.
    /// </para>
    /// </remarks>
    public class SynchronizeActiveItemsBehavior
    {
        private bool _isSynchronizing;
        private IRegion _region;
        private Selector _control;


        public void Attach(IRegion region, DependencyObject regionHost)
        {
            if(!(regionHost is Selector))
                throw new InvalidOperationException("Only Selector and MultiSelector controls can be enchanced by this behavior");

            _control = (Selector)regionHost;
            _region = region;

            _control.SelectionChanged += ControlSelectionChanged;
            _region.ActiveViews.CollectionChanged += RegionActiveViewsChanged;
        }


        private void RegionActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // avoid reentries
            if (_isSynchronizing)
                return;
            _isSynchronizing = true;

            var multiSelector = _control as MultiSelector;
            if (multiSelector == null)
            {
                _control.SelectedItem = _region.ActiveViews.FirstOrDefault();
            }
            else
            {
                foreach (var oldItem in e.OldItems)
                    multiSelector.SelectedItems.Remove(oldItem);

                foreach (var newItem in e.NewItems)
                    multiSelector.SelectedItems.Add(newItem);
            }

            _isSynchronizing = false;
        }


        private void ControlSelectionChanged(object s, SelectionChangedEventArgs e)
        {
            // avoid reentries
            if (_isSynchronizing)
                return;
            _isSynchronizing = true;

            foreach (var removedItem in e.RemovedItems)
                _region.Deactivate(removedItem);
            foreach (var addedItem in e.AddedItems)
                _region.Activate(addedItem);

            _isSynchronizing = false;
        }
    }
}