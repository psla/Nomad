using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Nomad.Regions.Behaviors
{
    public class SynchronizeItemsBehavior
    {
        public void Attach(IRegion region, ItemsControl control)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (control == null) throw new ArgumentNullException("control");

            var binding = new Binding("Views");
            binding.Source = region;

            BindingOperations.SetBinding(control, ItemsControl.ItemsSourceProperty, binding);
        }
    }
}