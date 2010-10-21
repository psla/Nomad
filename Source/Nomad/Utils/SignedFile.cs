using System;
using Nomad.Modules.Manifest;

namespace Nomad.Utils
{
    /// <summary>
    /// Informs about signed file
    /// </summary>
    /// <remarks>
    ///     <para>This class creates part of <see cref="ModuleManifest"/>. Contains information about module file and it's signature information </para>
    /// </remarks>
    [Serializable]
    public sealed class SignedFile
    {
        ///<summary>
        /// Empty constructor used by serializer
        ///</summary>
        public SignedFile()
        {
        }


        /// <summary>
        /// Relative path to file
        /// </summary>
        public string FilePath { get; set; }

        ///<summary>
        /// Signature of file it will be checked against
        ///</summary>
        public byte[] Signature { get; set; }
    }
}