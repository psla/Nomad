using System;
using System.Text.RegularExpressions;

namespace Nomad.Modules.Manifest
{
    /// <summary>
    /// Serializable implementation of version
    /// </summary>
    [Serializable]
    public sealed class Version
    {
        #region == Version Properties ==

        /// <summary>
        /// Gets the value of the build component of the version number for the current Version object.
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        /// Gets the value of the major component of the version number for the current Version object.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets the high 16 bits of the revision number.
        /// </summary>
        public int MajorRevision { get; set; }

        /// <summary>
        /// Gets the value of the minor component of the version number for the current Version object.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets the low 16 bits of the revision number.
        /// </summary>
        public int MinorRevision { get; set; }

        /// <summary>
        /// Gets the value of the revision component of the version number for the current Version object.
        /// </summary>
        public int Revision { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// Creates version without any information. It is used for serialization purposes.
        /// </summary>
        public Version()
        {
        }


        /// <summary>
        /// Initializes version which is serializable counterpart to <see cref="System.Version"/>
        /// </summary>
        /// <param name="version">Version instance to use for creation serializable counterpart</param>
        public Version(System.Version version)
        {
            Build = version.Build;
            Major = version.Major;
            MajorRevision = version.MajorRevision;
            Minor = version.Minor;
            MinorRevision = version.MinorRevision;
            Revision = version.Revision;
        }


        ///<summary>
        /// Initializes serializable version based on provided string
        ///</summary>
        ///<param name="version">version string in format x.x.x.x</param>
        /// <exception cref="ArgumentException">when provided string is not properly formatted</exception>
        public Version(string version)
        {
            StringToVersion(version);
        }


        ///<summary>
        /// Initializes version based on provided version parameters
        ///</summary>
        ///<param name="major"></param>
        ///<param name="minor"></param>
        ///<param name="build"></param>
        ///<param name="revision"></param>
        public Version(int major, int minor, int build, int revision)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        #endregion

        #region System Version Conversion

        ///<summary>
        /// Creates system counterpart of version
        ///</summary>
        ///<returns><see cref="System.Version"/> counterpart for this version</returns>
        public System.Version GetSystemVersion()
        {
            var item = new System.Version(Major, Minor, Build,
                                          Revision);
            return item;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
        }


        private void StringToVersion(string inputString)
        {
            var regex =
                new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)\.(?<revision>\d+)$");
            Match match = regex.Match(inputString);
            if (match.Success)
            {
                Major = int.Parse(match.Groups["major"].Value);
                Minor = int.Parse(match.Groups["minor"].Value);
                Build = int.Parse(match.Groups["build"].Value);
                Revision = int.Parse(match.Groups["revision"].Value);
            }
            else
            {
                throw new ArgumentException("Cannot match the String to Version");
            }
        }
    }
}