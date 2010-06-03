using System;
using System.Windows;
using System.Linq;

namespace Nomad.Regions
{
    /// <summary>
    ///     Default implementation of <see cref="IRegionFactory"/>
    /// </summary>
    public class RegionFactory : IRegionFactory
    {
        private readonly IRegionAdapter[] _regionAdapters;


        public RegionFactory(IRegionAdapter[] regionAdapters)
        {
            _regionAdapters = regionAdapters;
        }


        public IRegion CreateRegion(DependencyObject view)
        {
            var type = view.GetType();
            while(type != null)
            {
                var adapter = FindAdapterFor(type);
                if (adapter != null)
                    return adapter.AdaptView(view);

                type = type.BaseType;
            }

            throw new InvalidOperationException("No adapter has been found for view of type " + view.GetType().Name);
        }


        private IRegionAdapter FindAdapterFor(Type type)
        {
            return _regionAdapters.FirstOrDefault(adapter => adapter.SupportedType == type);
        }
    }
}