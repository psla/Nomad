using System;
using System.Reflection;

namespace Nomad.Utils.ManifestCreator.VersionProviders
{
    ///<summary>
    ///     Basic nomad implementation of <see cref="IVersionProvider"/>
    ///</summary>
    /// <remarks>
    ///     Uses <see cref="AssemblyName"/> to get the proper version without loading the assembly.
    /// </remarks>
    public class SimpleVersionProvider : IVersionProvider
    {
        #region IVersionProvider Members

        public Version GetVersion(string assemblyPath)
        {
            Version version = null;

            try
            {
                //version = new Version(AssemblyName.GetAssemblyName(GetAssemblyPath()).Version);
                version = new Version(AssemblyName.GetAssemblyName(assemblyPath).Version);
            }
            catch (Exception)
            {
                //TODO: this cannot be done ! this way. Implement the proper way of logging this exception
                version = new Version("0.0.0.0");
            }

            return version;
        }

        #endregion
    }
}