using System;
using System.Windows;

namespace Nomad.Regions
{
    /// <summary>
    ///     Defines methods required to create a region and attach it to a view
    /// </summary>
    public interface IRegionFactory
    {
        /// <summary>
        ///     Creates and attaches new region to the specified <paramref name="view"/>
        /// </summary>
        /// <param name="view">View to create new region for</param>
        /// <returns>Initialized and attached <see cref="IRegion"/> object</returns>
        /// <exception cref="InvalidOperationException">When a new region cannot be instantiated, beause view's type is not supported by this factory</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="view"/> is null</exception>
        IRegion CreateRegion(DependencyObject view);
    }
}