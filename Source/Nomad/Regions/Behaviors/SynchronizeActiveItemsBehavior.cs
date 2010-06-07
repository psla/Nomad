using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace Nomad.Regions.Behaviors
{
    public class SynchronizeActiveItemsBehavior
    {
        private bool _synchronizing = false;
        
        public void Attach(IRegion region, Selector control)
        {
            control.SelectionChanged += delegate(object s, SelectionChangedEventArgs e)
                                            {
                                                if(_synchronizing)
                                                    return;
                                                _synchronizing = true;
                                                foreach (var removedItem in e.RemovedItems)
                                                {
                                                    region.Deactivate(removedItem);
                                                }
                                                foreach (var addedItem in e.AddedItems)
                                                {
                                                    region.Activate(addedItem);
                                                }
                                                _synchronizing = false;
                                            };

            // temp - hack
            region.ActiveViews.CollectionChanged += (s, e) =>
                                                        {
                                                            if(_synchronizing)
                                                                return;
                                                            _synchronizing = true;
                                                            control.SelectedItem =
                                                                region.ActiveViews.FirstOrDefault();
                                                            _synchronizing = false;
                                                        };
        }
    }
}