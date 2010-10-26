namespace Nomad.Modules
{
    /// <summary>
    ///     Can be used by <see cref="ModuleManager"/>. 
    /// </summary>
    /// <remarks>
    /// Implementation of this interface is must have requirement for using assembly as module.
    /// </remarks>
    public interface IModuleBootstraper
    {
        /// <summary>
        ///     Executed during module loading by <see cref="ModuleManager"/>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Executed after being loaded into CLR library space.
        ///     </para>
        ///     <para>
        ///         Any dependencies on the other modules will not be resolved earlier.
        ///     </para>
        /// </remarks>
        void OnLoad();


        /// <summary>
        ///     Executed before unloading module.
        /// </summary>
        /// <remarks>
        ///     This method is executed in random sequence.
        /// </remarks>
        void OnUnLoad();
    }
}