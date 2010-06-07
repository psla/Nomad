namespace Nomad.Regions
{
    public interface IRegion
    {
        IViewCollection Views { get; }

        void AddView(object view);
    }
}