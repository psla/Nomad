using System.IO;

namespace Nomad.Signing
{
    /// <summary>
    /// Structure describing file. It's filename and data it contains.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Creates Empty file structure
        /// </summary>
        public File()
        {
        }

        /// <summary>
        /// Creates file structure filled with data from according filepath
        /// </summary>
        /// <param name="filePath">Path to file</param>
        public File(string filePath)
        {
            Data = System.IO.File.ReadAllBytes(filePath);
            FileName = Path.GetFileName(filePath);
        }

        /// <summary>
        /// Filename corresponding to <see cref="Data"/>
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Data of corresponding <see cref="FileName"/>
        /// </summary>
        public byte[] Data { get; set; }
    }
}