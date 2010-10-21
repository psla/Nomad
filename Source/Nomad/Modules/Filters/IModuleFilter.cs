namespace Nomad.Modules.Filters
{
    /// <summary>
    /// IModuleFilter provides mininum weight interface to check if ModuleInfo fullfils filtering rules applied.
    /// </summary>
    public interface IModuleFilter
    {
        /// <summary>
        /// Checks if provided <see cref="ModuleInfo"/> matches the filter rules.
        /// </summary>
        /// <param name="moduleInfo">Checked ModuleInfo object.</param>
        /// <returns>True on successful match.</returns>
        bool Matches(ModuleInfo moduleInfo);
    }
}