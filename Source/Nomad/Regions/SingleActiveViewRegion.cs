using System;

namespace Nomad.Regions
{
    /// <summary>
    ///     A region that has only one active view at a time.
    /// </summary>
    public class SingleActiveViewRegion : IRegion
    {
        private readonly IViewCollection _views = new ViewCollection();

        public IViewCollection Views
        {
            get { return _views; }
        }

        public void AddView(object view)
        {
            _views.Add(view);
        }
    }
}