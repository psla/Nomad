using System;

namespace Nomad.Regions
{
    /// <summary>
    ///     A region that has only one active view at a time.
    /// </summary>
    public class SingleActiveViewRegion : IRegion
    {
        private readonly IViewCollection _views = new ViewCollection();
        private readonly IViewCollection _activeViews = new ViewCollection();

        public IViewCollection Views
        {
            get { return _views; }
        }

        public IViewCollection ActiveViews
        {
            get { return _activeViews; }
        }

        public void AddView(object view)
        {
            _views.Add(view);
        }

        public void Activate(object view)
        {
            if(_activeViews.Count > 0)
                _activeViews.RemoveAt(0);
            _activeViews.Add(view);
        }

        public void Deactivate(object view)
        {
            _activeViews.Remove(view);
        }
    }
}