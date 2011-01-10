using System;
using System.IO;
using Nomad.Modules.Manifest;
using Nomad.Utils;

namespace Nomad.Updater.ModuleFinders
{
    /// <summary>
    ///     Nomad default implementation of <see cref="IModuleFinder"/>. Searches the provided folder searching for package.
    /// </summary>
    /// <remarks>
    ///     Checks for existing of the following module in the file system. If the module does not exists it creates new folder
    /// for this module. 
    ///     If module already exists, verification by <see cref="ModuleManifest.ModuleName"/> the unpack will be into its folder.
    ///     Content of the folders won't be change by this method. 
    ///     By using file system operation this method might provide impact on performance. However the search is not totally accurate due to 
    /// impact on the loading each manifest found.
    /// TODO: Method constructs dictionary for speeding up already found element.
    /// </remarks>
    public class ModuleFinder : IModuleFinder
    {
        public string FindDirectoryForPackage(string targetDirectory, ModulePackage modulePackage)
        {
            if (string.IsNullOrEmpty(targetDirectory) || modulePackage == null)
                throw new ArgumentException("Bot arguments must be provided");

            // get module name from package
            var moduleName = modulePackage.ModuleManifest.ModuleName;

            // founded manifest
            string manifestDirectory = string.Empty;

            // search directory for the module with such name
            var directoryInfo = new DirectoryInfo(targetDirectory);
            const string searchPattern = "*" + ModuleManifest.ManifestFileNameSuffix;
            var files = directoryInfo.GetFiles(searchPattern, SearchOption.AllDirectories);
            foreach (var manifestFile in files)
            {
                // get manifest out of the file
                var fileData = File.ReadAllBytes(manifestFile.FullName);
                var manifest = XmlSerializerHelper.Deserialize<ModuleManifest>(fileData);

                // check the names with the manifest)
                if (manifest.ModuleName.Equals(moduleName))
                {
                    manifestDirectory = manifestFile.DirectoryName;
                    break;
                }
            }

            // if no manifest directory found then make the new folder containing the new module
            if (string.IsNullOrEmpty(manifestDirectory))
            {
                manifestDirectory = Path.Combine(targetDirectory, moduleName);
                Directory.CreateDirectory(manifestDirectory);
            }

            return manifestDirectory;
        }
    }
}