using System.Windows;
using System.Windows.Controls;
using Castle.Core.Resource;

namespace Nomad.Regions.Behaviors
{
    public class ItemsControlTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var frameworkElement = container as FrameworkElement;
            if(frameworkElement == null)
                return base.SelectTemplate(item, container);
            
            if(item is IHaveTitle)
            {
                var template = frameworkElement.TryFindResource("Nomad_ItemWithTitle") as DataTemplate;
                if (template != null)
                    return template;
                    
            }

            return base.SelectTemplate(item, container);
        }
    }
}