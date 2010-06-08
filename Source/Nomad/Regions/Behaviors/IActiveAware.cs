namespace Nomad.Regions.Behaviors
{
    /// <summary>
    /// Specifies that view should be notified when it becomes active
    /// </summary>
    public interface IActiveAware
    {
        void SetIsActive(bool isActive);
    }
}