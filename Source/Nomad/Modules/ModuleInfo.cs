using System;
using Nomad.Modules.Manifest;

namespace Nomad.Modules
{
    /// <summary>
    ///     Represents single module - all the information
    ///     uniquely identifying a module and required to properly load
    ///     and initialize it.
    /// </summary>
    [Serializable]
    public class ModuleInfo : IEquatable<ModuleInfo>
    {
        private readonly string _assemblyPath;
        private readonly IModuleManifestFactory _moduleManifestFactory;
        private ModuleManifest _manifest;


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleInfo"/> class.
        /// </summary>
        /// <param name="assemblyPath">Full or relative path to the assembly file containing module's code</param>
        /// <exception cref="ArgumentException">When <paramref name="assemblyPath"/> is <c>null</c> or empty.</exception>
        public ModuleInfo(string assemblyPath) : this(assemblyPath, null, DefaultFactory)
        {
        }


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleInfo"/> class.
        /// </summary>
        /// <param name="assemblyPath">Full or relative path to the assembly file containing module's code</param>
        /// <param name="manifest">Module Manifest connected with assembly file defined in <paramref name="assemblyPath"/>. Can be null.</param>
        /// <param name="factory">IModuleManifest factory that connects the concrete implementation of ModuleManifest with in program ModuleManifest. Can be null</param>
        /// <exception cref="ArgumentException">
        /// When <paramref name="assemblyPath"/> is <c>null</c> or empty. 
        /// When <paramref name="manifest"/> or <paramref name="factory"/> is <c>null</c>.
        /// </exception>
        public ModuleInfo(string assemblyPath, ModuleManifest manifest,
                          IModuleManifestFactory factory)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException("assemblyPath is required", assemblyPath);

            if (manifest == null && factory == null)
                throw new ArgumentException("manifest OR factory is required");

            _assemblyPath = assemblyPath;
            _manifest = manifest;
            _moduleManifestFactory = factory;
        }


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleInfo"/> class.
        /// </summary>
        /// <param name="assemblyPath">Full or relative path to the assembly file containing module's code</param>
        /// <param name="factory">IModuleManifest factory that connects the concrete implementation of ModuleManifest with in program ModuleManifest</param>
        public ModuleInfo(string assemblyPath, IModuleManifestFactory factory)
            : this(assemblyPath, null, factory)
        {
        }


        /// <summary>
        ///     Default implementation of the <see cref="IModuleManifestFactory"/> that provides connection between ModuleManifest with FileSystem.
        /// </summary>
        public static IModuleManifestFactory DefaultFactory
        {
            get { return new ModuleManifestFactory(); }
        }


        /// <summary>
        ///     Gets or sets the manifest describing the module defined in <see cref="AssemblyPath"/>.
        /// </summary>
        public ModuleManifest Manifest
        {
            get
            {
                if (_manifest == null)
                {
                    _manifest = _moduleManifestFactory.GetManifest(this);
                }
                return _manifest;
            }
            set { _manifest = value; }
        }

        /// <summary>
        ///     Gets the current <see cref="IModuleManifestFactory"/> for this <see cref="ModuleInfo"/> instance.
        /// </summary>
        public IModuleManifestFactory ManifestFactory
        {
            get { return _moduleManifestFactory; }
        }


        /// <summary>
        ///     Gets full or relative path to the assembly file containing module's code.
        /// </summary>
        public string AssemblyPath
        {
            get { return _assemblyPath; }
        }

        #region IEquatable<ModuleInfo> Members

        public bool Equals(ModuleInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            //TODO : add equality by version, watch out for getting Manifest ;)

            return Equals(other.Manifest.ModuleName, Manifest.ModuleName) &&
                   Equals(other._assemblyPath, _assemblyPath);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ModuleInfo)) return false;
            return Equals((ModuleInfo) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return ((_manifest != null ? _manifest.GetHashCode() : 0) * 397) ^
                       (_assemblyPath != null ? _assemblyPath.GetHashCode() : 0);
            }
        }


        public override string ToString()
        {
            return string.Format("Assembly Path {0}", AssemblyPath);
        }
    }
}