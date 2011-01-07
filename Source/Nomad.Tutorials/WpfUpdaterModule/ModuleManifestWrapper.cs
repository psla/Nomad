using Nomad.Modules.Manifest;

namespace WpfUpdaterModule
{
    /// <summary>
    ///     Class representing module for update
    /// </summary>
    public class ModuleManifestWrapper
    {
        public ModuleManifest Manifest { get; set; }
        public bool SelectedForUpdate { get; set; }

    }
}