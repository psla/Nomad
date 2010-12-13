using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Nomad.RepositoryServer.Models.ServerSigner;

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

            var data = File.ReadAllBytes(tmpZipFile);
            File.Delete(tmpZipFile);
            return data;
        }


        public byte[] Package(string pathToFolder, IList<VirtualFileWrapper> inFolderFiles)
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
                    var item = inFolderFiles
                        .Where(x => x.FileName.Equals(info) && x.ToPackage == false)
                        .DefaultIfEmpty(null)
                        .SingleOrDefault();

                    if(item != null)
                        continue;

                    zipFile.AddFile(fileInfo.FullName, ".");
                }

                zipFile.Save(tmpZipFile);
            }

            var data = File.ReadAllBytes(tmpZipFile);
            File.Delete(tmpZipFile);
            return data;
        }


        public IModuleInfo UnPack(byte[] packageData)
        {
            if (packageData == null)
                throw new ArgumentNullException("packageData");

            
            return new ModuleInfo();
        }
    }
}