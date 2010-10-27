namespace Nomad.Modules
{
    /// <summary>
    ///     Responsible for loading a single module defined by <see cref="ModuleInfo"/>.
    /// </summary>
    public interface IModuleLoader
    {
        /// <summary>
        ///     Tries to load and initialize a module identified by passed module info.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This method will try to load an assembly containing a module identified
        ///     by passed <paramref name="moduleInfo"/>, then it will search for a single,
        ///     public, non-abstract bootstraper class contained in that assembly.
        ///     </para>
        ///     <para>
        ///     Then, the bootstraper object will be instantiated in an enclosed service
        ///     location context (i.e. sub-container - see implementation documentation for
        ///     more details) and executed. 
        ///     </para>
        ///     <para>
        ///     Module will be considered successfully loaded if and only if all above steps 
        ///     are successful.
        ///     </para>
        ///     <para>
        ///     Since this method will try to load a module in current AppDomain, 
        ///     the module's assembly will not be unloaded if module's bootstraper fails.
        ///     </para>
        /// </remarks>
        /// <param name="moduleInfo">Module's manifest</param>
        void LoadModule(ModuleInfo moduleInfo);


        /* TODO: Add prevalidation step - load assemblies reflection only and validate bootstraper is present and valid
         * Right now, assemblies are loaded before bootstrapers classes are identified.
         * This means, that it is possible to filter out some more invalid modules without poluting the appdomain with
         * assemblies. However, this seems a little overkill to me and I've decided not to do so.
         * 
         * This can be implemented as a ModuleFilter, though
         * */

        /// <summary>
        ///     Tries to invoke <see cref="IModuleBootstraper.OnUnLoad"/>  method on each module bootstraper from set.
        /// </summary>
        void InvokeUnload();
    }
}