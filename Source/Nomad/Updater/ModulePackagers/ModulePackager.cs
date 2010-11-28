using Ionic.Zip;

namespace Nomad.Updater.ModulePackagers
{
    /// <summary>
    ///     Default Nomad implementation of <see cref="IModulePackager"/>. Compatible with ZIP file format.
    /// </summary>
    public class ModulePackager : IModulePackager
    {
        #region IModulePackager Members

        public void PerformUpdates(string targetDirectory, ModulePackage modulePackage)
        {
            using (ZipFile file = ZipFile.Read(modulePackage.ModuleZip))
            {
                file.ExtractAll(targetDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        #endregion
    }
}