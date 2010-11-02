using System;

namespace Nomad.Tests.FunctionalTests.Modules
{
    /// <summary>
    ///     Generates the module assembly from source files.
    /// </summary>
    /// <remarks>
    ///     Puts the additional Nomad dependency.
    /// </remarks>
    public class ModuleCompiler
    {
        /// <summary>
        ///     Generates the module from <paramref name="sourceFilePath"/> with the additional assemblies presented in <paramref name="dependeciesAssembliesPath"/>.
        /// </summary>
        /// <param name="sourceFilePath">ource file </param>
        /// <param name="dependeciesAssembliesPath">Array of the dependecies (assemblies) to be the following assembly dependent on.</param>
        public void GenerateModuleFromCode(string sourceFilePath, params string[] dependeciesAssembliesPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Represents the output directory where the build artifacts will be put.
        /// </summary>
        public string OutputDirectory { get; set; }
    }
}