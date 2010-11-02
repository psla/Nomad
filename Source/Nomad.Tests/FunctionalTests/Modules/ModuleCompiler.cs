using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public static string NomadAssembly = AppDomain.CurrentDomain.BaseDirectory + @"\Nomad.dll";

        public static string NomadTestAssembly = AppDomain.CurrentDomain.BaseDirectory +
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
                    Directory.CreateDirectory(value);
                }
                _outputDirectory = value;
            }
        }


        /// <summary>
        ///     Generates the module from <paramref name="sourceFilePath"/> with the additional assemblies presented in <paramref name="dependeciesAssembliesPath"/>.
        /// </summary>
        /// <param name="sourceFilePath">ource file </param>
        /// <param name="dependeciesAssembliesPath">Array of the dependecies (assemblies) to be the following assembly dependent on.</param>
        public void GenerateModuleFromCode(string sourceFilePath,
                                           params string[] dependeciesAssembliesPath)
        {
            var asmReferences = dependeciesAssembliesPath.ToList()
                .Select(x => Path.Combine(OutputDirectory, x));

            asmReferences = asmReferences.Concat(new List<string>
                                                     {
                                                         NomadAssembly,
                                                         NomadTestAssembly
                                                     });

            _parameters = new CompilerParameters(asmReferences.ToArray());

            _parameters.GenerateExecutable = false;
            _parameters.TreatWarningsAsErrors = false;
            _parameters.OutputAssembly = Path.Combine(OutputDirectory,
                                                      Path.GetFileNameWithoutExtension(
                                                          sourceFilePath) + ".dll");
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
        }
    }
}