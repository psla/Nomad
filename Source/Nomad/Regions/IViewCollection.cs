using System.Collections.Generic;
using System.Collections.Specialized;

namespace Nomad.Regions
{
    /// <summary>
    ///     Collection of region's views
    /// </summary>
    public interface IViewCollection : IList<object>, INotifyCollectionChanged
    {
        
    }
}