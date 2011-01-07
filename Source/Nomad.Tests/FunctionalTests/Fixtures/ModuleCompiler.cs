using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nomad.Modules.Manifest;
using Nomad.Signing;
using Nomad.Utils.ManifestCreator;

namespace Nomad.Tests.FunctionalTests.Fixtures
{
    /// <summary>
    ///     Generates the module assembly from source files.
    /// </summary>
    /// <remarks>
    ///     Puts the additional Nomad dependency.
    /// </remarks>
    public class ModuleCompiler
    {
        private static readonly string NomadAssembly = AppDomain.CurrentDomain.BaseDirectory +
                                                      @"\Nomad.dll";

        private static readonly string NomadTestAssembly = AppDomain.CurrentDomain.BaseDirectory +
                                                          @"\Nomad.Tests.dll";

        private readonly CodeDomProvider _provider;
        private string _outputDirectory;
        private CompilerParameters _parameters;
        private CompilerResults _results;


        public ModuleCompiler()
        {
            _provider = CodeDomProvider.CreateProvider("CSharp");
        }

        /// <summary>
        ///     Represents the output path with name to the created assembly.
        /// </summary>
        public string OutputName { get; set; }

        /// <summary>
        ///     Represents the output directory where the build artifacts will be put.
        /// </summary>
        public string OutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_outputDirectory))
                    _outputDirectory = ".";
                return _outputDirectory;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!Directory.Exists(value))
                        Directory.CreateDirectory(value);
                }
                _outputDirectory = value;
            }
        }

        /// <summary>
        ///     Gets the path to SimplestModulePossible1.
        /// </summary>
        public static string DefaultSimpleModuleSource
        {
            get { return @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible1.cs"; }
        }
        
        /// <summary>
        ///     Gets the alternative path. Thus SimplestModulePossible2.
        /// </summary>
        public static string DefaultSimpleModuleSourceAlternative
        {
            get { return @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible2.cs"; }
        }

        public static string DefaultSimpleModuleSourceLastAlternative
        {
            get { return @"..\Source\Nomad.Tests\FunctionalTests\Data\SimplestModulePossible3.cs"; }
        }

        /// <summary>
        ///     Generates the module from <paramref name="sourceFilePath"/> with the additional assemblies presented in <paramref name="dependeciesAssembliesPath"/>.
        /// </summary>
        /// <param name="sourceFilePath">ource file </param>
        /// <param name="dependeciesAssembliesPath">Array of the dependecies (assemblies) to be the following assembly dependent on.</param>
        /// <returns>The path to the compiled assembly.</returns>
        public string GenerateModuleFromCode(string sourceFilePath,
                                             params string[] dependeciesAssembliesPath)
        {
            IEnumerable<string> asmReferences =
                dependeciesAssembliesPath.ToList().Concat(new List<string>
                                                              {
                                                                  NomadAssembly,
                                                                  NomadTestAssembly
                                                              });
            if(string.IsNullOrEmpty(OutputName))
                OutputName = Path.Combine(OutputDirectory,
                         Path.GetFileNameWithoutExtension(
                             sourceFilePath) + ".dll");

            _parameters = new CompilerParameters(asmReferences.ToArray())
                              {
                                  GenerateExecutable = false,
                                  TreatWarningsAsErrors = false,
                                  OutputAssembly = OutputName,
                              };

            string srcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sourceFilePath);
            _results = _provider.CompileAssemblyFromFile(_parameters, srcPath);

            if (_results.Errors.Count > 0)
            {
                foreach (CompilerError  error in _results.Errors)
                {
                    Console.WriteLine(error);
                }
                throw new Exception("Compilation exception during compiling modules");
            }

            OutputName = null;

            return _parameters.OutputAssembly;
        }

        /// <summary>
        ///     Wrapps the generating manifest with values that should be provided. Provides access to <paramref name="configuration"/>.
        /// </summary>
        /// <param name="modulePath"></param>
        /// <param name="keyLocation"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public string GenerateManifestForModule(string modulePath, string keyLocation,ManifestBuilderConfiguration configuration)
        {
            string directory = Path.GetFullPath(Path.GetDirectoryName(modulePath));
            var builder = new ManifestBuilder("TEST_ISSUER_COMPILER",
                                              keyLocation,
                                              Path.GetFileName(modulePath), directory,KeyStorage.Nomad,string.Empty,configuration);
            builder.CreateAndPublish();

            return modulePath + ModuleManifest.ManifestFileNameSuffix;
        }

        public string GenerateManifestForModule(string modulePath,string keyLocation)
        {
            return GenerateManifestForModule(modulePath, keyLocation, ManifestBuilderConfiguration.Default);
        }
    }
}