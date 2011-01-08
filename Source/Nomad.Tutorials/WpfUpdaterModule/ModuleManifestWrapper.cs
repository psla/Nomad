using Nomad.Modules.Manifest;

namespace WpfUpdaterModule
{
    /// <summary>
    ///     Class representing module for update
    /// </summary>
    public class ModuleManifestWrapper
    {
        /// <summary>
        ///     Manifest describing module
        /// </summary>
        public ModuleManifest Manifest { get; set; }

        /// <summary>
        ///     Indicates if selected module is for update
        /// </summary>
        public bool SelectedForUpdate { get; set; }

    }
}