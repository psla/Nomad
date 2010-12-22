using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Nomad.Regions.Behaviors
{
    public class SynchronizeToolbarsBehavior
    {
        private ToolBarTray _control;
        private IRegion _region;


        public void Attach(IRegion region, DependencyObject regionHost)
        {
            if(region == null) throw new ArgumentNullException("region");
            if(regionHost == null) throw new ArgumentNullException("regionHost");
            if(!(regionHost is ToolBarTray))
                throw new InvalidOperationException("SynchronizeToolbars can be executed only on toolbar tray");

            _control = (ToolBarTray) regionHost;
            _region = region;

            _region.Views.CollectionChanged += ViewsCollectionChanged;
        }

        void ViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _control.ToolBars.Clear();
            foreach (var view in _region.Views)
            {
                if (!(view is ToolBar))
                    throw new InvalidOperationException("view must be of type ToolBar");

                _control.ToolBars.Add(view as ToolBar);
            }
        }
    }
}
