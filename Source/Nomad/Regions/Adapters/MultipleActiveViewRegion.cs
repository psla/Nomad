using System;

namespace Nomad.Regions.Adapters
{
    /// <summary>
    /// A region that may have multiple active regions at a time
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class MultipleActiveViewRegion : IRegion
    {
        private readonly IViewCollection _activeViews = new ViewCollection();
        private readonly IViewCollection _views = new ViewCollection();

        #region IRegion Members

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
            if (view == null)
                throw new ArgumentNullException("view");
            _views.Add(view);
        }


        /// <summary>
        /// When acitvating view which is currently active, nothing happens.
        /// </summary>
        /// <param name="view">View to be activated</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        public void Activate(object view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            if (!_activeViews.Contains(view))
                _activeViews.Add(view);
        }


        ///<summary>
        /// When deactivating view which was not active, nothing happens.
        /// </summary>
        /// <param name="view">View to be deactivated</param>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        public void Deactivate(object view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            _activeViews.Remove(view);
        }


        public void ClearViews()
        {
            _views.Clear();
        }

        #endregion
    }
}