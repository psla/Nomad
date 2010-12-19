using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Nomad.Modules.Manifest;
using Nomad.RepositoryServer.Models.ServerSigner;
using Nomad.Utils;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Packs the data into zip file.
    /// </summary>
    public class ZipPackager
    {
        public byte[] Package(string toFolder)
        {
            // pack the things from the form into zip file 
            string tmpZipFile = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {
                var directoryInfo = new DirectoryInfo(toFolder);

                // get all files from this directory into zip archive
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    zipFile.AddFile(fileInfo.FullName, ".");
                }

                zipFile.Save(tmpZipFile);
            }

            byte[] data = File.ReadAllBytes(tmpZipFile);
            File.Delete(tmpZipFile);
            return data;
        }


        public byte[] Package(string pathToFolder, IEnumerable<VirtualFileWrapper> inFolderFiles)
        {
            // pack the things from the form into zip file 
            string tmpZipFile = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {
                var directoryInfo = new DirectoryInfo(pathToFolder);

                // get all files from this directory into zip archive
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    // find this file in in folderFiles 
                    FileInfo info = fileInfo;
                    VirtualFileWrapper item = inFolderFiles
                        .Where(x => x.FileName.Equals(info) && x.ToPackage == false)
                        .DefaultIfEmpty(null)
                        .SingleOrDefault();

                    if (item != null)
                        continue;

                    zipFile.AddFile(fileInfo.FullName, ".");
                }

                zipFile.Save(tmpZipFile);
            }

            byte[] data = File.ReadAllBytes(tmpZipFile);
            File.Delete(tmpZipFile);
            return data;
        }


        /// <summary>
        ///     Unzips the data from <paramref name="packageData"/> and access <see cref="ModuleManifest"/>.
        /// </summary>
        /// <param name="packageData">The raw data of the package.</param>
        /// <returns><see cref="ModuleInfo"/> object that has ModuleManifest found.</returns>
        public IModuleInfo UnPack(byte[] packageData)
        {
            if (packageData == null)
                throw new ArgumentNullException("packageData");

            ZipFile zipFile = null;
            ModuleManifest manifest = null;

            try
            {
                zipFile = ZipFile.Read(packageData);

                // find manifest inside the zip data
                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (zipEntry.FileName.EndsWith(ModuleManifest.ManifestFileNameSuffix))
                    {
                        // use this file as manifest
                        var ms = new MemoryStream();
                        zipEntry.Extract(ms);
                        manifest = XmlSerializerHelper.Deserialize<ModuleManifest>(ms.ToArray());

                        // now search for the assembly described by this manifest, simply by iterating through zip entries
                        if (
                            !zipFile.Any(
                                entry =>
                                entry.FileName.EndsWith(".dll") | entry.FileName.EndsWith("exe")))
                        {
                            throw new ArgumentException("No assembly in the package");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException("The file is corrupted",e);
            }
            finally
            {
                if (zipFile != null)
                    zipFile.Dispose();
            }

            // have not found manifest
            if (manifest == null)
                throw new InvalidDataException("No manifest file in package");

            return new ModuleInfo
                       {
                           ModuleData = packageData,
                           Manifest = manifest
                       };
        }
    }
}