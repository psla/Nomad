using System.Windows;

namespace Nomad.Regions
{
    public interface IRegionFactory
    {
        IRegion CreateRegion(DependencyObject view);
    }
}