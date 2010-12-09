using System;
using System.IO;
using Ionic.Zip;

namespace Nomad.RepositoryServer.Models
{
    /// <summary>
    ///     Packs the data into zip file.
    /// </summary>
    public class ZipPackager
    {
        public byte[] Package(string pathToFolder)
        {
            // pack the things from the form into zip file 
            string tmpZipFile = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {

                var directoryInfo = new DirectoryInfo(pathToFolder);

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
    }
}