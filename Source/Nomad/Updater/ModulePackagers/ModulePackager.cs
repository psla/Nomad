using System.Collections.Generic;
using Ionic.Zip;

namespace Nomad.Updater.ModulePackagers
{
    /// <summary>
    ///     Default Nomad implementation of <see cref="IModulePackager"/>. Compatible with ZIP file format.
    /// </summary>
    public class ModulePackager : IModulePackager
    {
        public void PerformUpdates(string targetDirectory, IEnumerable<ModulePackage> modulePackages)
        {
            foreach (ModulePackage modulePackage in modulePackages)
            {
                using (ZipFile file = ZipFile.Read(modulePackage.ModuleZip))
                {
                    file.ExtractAll(targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}