using System;
using System.Windows;

namespace Nomad.Regions
{
    public interface IRegionAdapter
    {
        Type SupportedType { get; }
        IRegion AdaptView(DependencyObject view);
    }
}