using System;

namespace Nomad.Modules
{
    /// <summary>
    ///     Represents single module's manifest - all the information
    ///     uniquely identifying a module and required to properly load
    ///     and initialize it.
    /// </summary>
    public class ModuleInfo : IEquatable<ModuleInfo>
    {
        private readonly string _assemblyPath;


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleInfo"/> class.
        /// </summary>
        /// <param name="assemblyPath">Full or relative path to the assembly file containing module's code</param>
        /// <exception cref="ArgumentException">When <paramref name="assemblyPath"/> is <c>null</c> or empty.</exception>
        public ModuleInfo(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException("assemblyPath is required", assemblyPath);

            _assemblyPath = assemblyPath;
        }


        /// <summary>
        ///     Gets full or relative path to the assembly file containing module's code
        /// </summary>
        public string AssemblyPath
        {
            get { return _assemblyPath; }
        }

        #region Equality comparison 

        /// <summary>Inherited.</summary>
        public bool Equals(ModuleInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._assemblyPath, _assemblyPath);
        }


        /// <summary>Inherited.</summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ModuleInfo)) return false;
            return Equals((ModuleInfo) obj);
        }


        /// <summary>Inherited.</summary>
        public override int GetHashCode()
        {
            return _assemblyPath.GetHashCode();
        }


        /// <summary>Inherited.</summary>
        public override string ToString()
        {
            return string.Format("AssemblyPath: {0}", _assemblyPath);
        }

        #endregion
    }
}