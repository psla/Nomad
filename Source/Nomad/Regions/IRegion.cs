namespace Nomad.Regions
{
    public interface IRegion
    {
        IViewCollection Views { get; }
        IViewCollection ActiveViews { get; }

        void AddView(object view);
        void Activate(object view);
        void Deactivate(object view);
    }
}