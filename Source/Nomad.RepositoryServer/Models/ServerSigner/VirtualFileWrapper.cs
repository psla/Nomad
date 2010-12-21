using System.IO;

namespace Nomad.RepositoryServer.Models.ServerSigner
{
    public class VirtualFileWrapper
    {
        public string PathToFile { get; set; }
        
        /// <summary>
        ///     Return size of the file in kB.
        /// </summary>
        public int Size { get { return File.ReadAllBytes(PathToFile).Length / 256; } }

        public string FileName { get; set; }

        public bool ToPackage { get; set; }
    }
}