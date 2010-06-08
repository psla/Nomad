using System;
using Nomad.Regions.Behaviors;

namespace Nomad.Regions
{
    /// <summary>
    ///     A region that has only one active view at a time.
    /// </summary>
    public class SingleActiveViewRegion : IRegion
    {
        private readonly IViewCollection _views = new ViewCollection();
        private readonly IViewCollection _activeViews = new ViewCollection();

        /// <summary>
        ///     Gets collection of all views that are members of this region
        /// </summary>
        public IViewCollection Views
        {
            get { return _views; }
        }

        /// <summary>
        ///     Gets collection of all views that are active (e.g. selected by user, visible, ...)
        /// </summary>
        /// <remarks>
        ///     Specific interpretation of what active means depends on type of control, that
        ///     is region's host, and on region adapter used.
        /// </remarks>
        public IViewCollection ActiveViews
        {
            get { return _activeViews; }
        }


        /// <summary>
        ///     Adds view to this region
        /// </summary>
        /// <remarks>
        /// <para>
        ///     <see cref="SingleActiveViewRegion"/> does not have any requirements on view's type.
        /// </para>
        /// </remarks>
        /// <param name="view">View to be added</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        public void AddView(object view)
        {
            if (view == null) throw new ArgumentNullException("view");
            _views.Add(view);
        }


        /// <summary>
        ///     Marks <paramref name="view"/> as active.
        /// </summary>
        /// <remarks>
        ///     When activating a view, previously activated view is deactivated.
        /// </remarks>
        /// <param name="view">View to be activated</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        public void Activate(object view)
        {
            if (view == null) throw new ArgumentNullException("view");
            if(_activeViews.Count > 0)
                _activeViews.RemoveAt(0);
            _activeViews.Add(view);
        }


        /// <summary>
        ///     Marks <paramref name="view"/> as not active
        /// </summary>
        /// <param name="view">View to be deactivated</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        public void Deactivate(object view)
        {
            _activeViews.Remove(view);
        }
    }
}