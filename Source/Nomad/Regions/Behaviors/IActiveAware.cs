namespace Nomad.Regions.Behaviors
{
    /// <summary>
    ///     Specifies that view should be notified when it becomes active.
    /// </summary>
    public interface IActiveAware
    {
        /// <summary>
        ///     Called when view is activated or deactivated.
        /// </summary>
        /// <remarks>
        ///     This method must not throw any exceptions.
        /// </remarks>
        /// <param name="isActive">True when view is active, false otherwise</param>
        void SetIsActive(bool isActive);
    }
}