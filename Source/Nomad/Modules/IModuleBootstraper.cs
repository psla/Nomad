namespace Nomad.Modules
{
    /// <summary>
    /// Can be started by <see cref="ModuleLoader"/>. 
    /// </summary>
    /// <remarks>
    /// Implementation of this interface is must have requirment for using assembly as module.
    /// </remarks>
    public interface IModuleBootstraper
    {
        /// <summary>
        /// Executed during module loading by <see cref="ModuleLoader"/>
        /// </summary>
        /// <remarks>
        ///     Executed after being loaded into CLR library space.
        /// </remarks>
        void Initialize();
    }
}