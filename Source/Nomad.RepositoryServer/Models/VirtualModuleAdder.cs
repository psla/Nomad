using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Nomad.Modules.Manifest;
using Version = Nomad.Utils.Version;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Encapsulates all information about process of module being added.
    /// </summary>
    /// <remarks>
    ///     Mostly all logic for managing temporary virtual directories and so on.
    /// </remarks>
    public class VirtualModuleAdder : IDisposable
    {
        public VirtualModuleAdder(string fileName)
        {
            // create temporary virtual folder for uploaded module
            VirtualFolderPath = Path.GetTempFileName();
            File.Delete(VirtualFolderPath);
            Directory.CreateDirectory(VirtualFolderPath);

            // save file in temporary space
            AssemblyFilePath = Path.Combine(VirtualFolderPath, fileName);

            // set the other files as empty
            InFolderFiles = new List<VirtualFileWrapper>();
        }


        public IModuleInfo ModuleInfo { get; private set; }
        public string VirtualFolderPath { get; private set; }

        public string AssemblyFilePath { get; private set; }
        public IList<VirtualFileWrapper> InFolderFiles { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            if (Directory.Exists(VirtualFolderPath))
                Directory.Delete(VirtualFolderPath, true);
        }

        #endregion

        public IModuleInfo GenerateModuleInfo()
        {
            AssemblyName asmName = AssemblyName.GetAssemblyName(AssemblyFilePath);

            var manifest = new ModuleManifest
                               {
                                   ModuleName = asmName.Name,
                                   ModuleVersion = new Version(asmName.Version),
                               };

            // get the moduleInfo but so called virtual implementation
            ModuleInfo = new VirtualModuleInfo
                                         {
                                             Manifest = manifest,
                                             ModuleData = File.ReadAllBytes(AssemblyFilePath),
                                         };

            return ModuleInfo;
        }


        public void SaveFileToVirtualFolder(HttpPostedFileBase file)
        {
            var filePath = Path.Combine(VirtualFolderPath, file.FileName);
            file.SaveAs(filePath);
            InFolderFiles.Add(new VirtualFileWrapper()
                                  {
                                      PathToFile = filePath,
                                      ToPackage = false,
                                      FileName = file.FileName,
                                  });

        }
    }
}