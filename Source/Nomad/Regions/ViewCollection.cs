using System.Collections.ObjectModel;

namespace Nomad.Regions
{
    /// <summary>
    ///     Default implementation of <see cref="IViewCollection"/> interface.
    /// </summary>
    public class ViewCollection : ObservableCollection<object>, IViewCollection
    {
    }
}