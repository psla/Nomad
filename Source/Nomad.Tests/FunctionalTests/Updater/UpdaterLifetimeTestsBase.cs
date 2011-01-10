using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ionic.Zip;
using Moq;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Tests.FunctionalTests.Fixtures;
using Nomad.Updater;
using Nomad.Updater.ModuleRepositories;
using Nomad.Utils;
using Nomad.Utils.ManifestCreator;
using Nomad.Utils.ManifestCreator.DependenciesProvider;
using Nomad.Utils.ManifestCreator.VersionProviders;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Updater
{
    /// <summary>
    ///     Base class wih various helper methods for better readability of the test cases.
    /// </summary>
    public class UpdaterLifetimeTestsBase : ModuleLoadingWithCompilerTestFixture
    {
        protected NomadConfiguration NomadConfigurationSettings;
        protected Mock<IModulesRepository> ModulesRepository;
        protected const string TestSpacePath = @"UpdaterLifetime\";


        /// <summary>
        ///     Sets the kernel up with the provided in <see cref="NomadConfigurationSettings"/> configuration.
        /// </summary>
        protected void SetUpKernel()
        {
            Kernel = new NomadKernel(NomadConfigurationSettings);

            Domain = Kernel.KernelAppDomain;
        }

        /// <summary>
        ///    Set up the Two Modules A,B into with provided version. Based on <see cref="ModuleCompiler.DefaultSimpleModuleSource"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="versionString"></param>
        /// <returns></returns>
        private IModuleDiscovery SetUpModulesWithVersion(string directory,
                                                         string versionString)
        {
            string moduleA = directory + @"\ModuleA";
            string moduleB = directory + @"\ModuleB";

            ManifestBuilderConfiguration builderConfiguration = ManifestBuilderConfiguration.Default;
            builderConfiguration.VersionProvider = GetVersionProviderForVersion(versionString);

            SetUpModuleWithManifest(moduleA, ModuleCompiler.DefaultSimpleModuleSource,
                                    builderConfiguration);
            SetUpModuleWithManifest(moduleB, ModuleCompiler.DefaultSimpleModuleSourceAlternative,
                                    builderConfiguration);

            return new CompositeModuleDiscovery(new DirectoryModuleDiscovery(moduleA, SearchOption.TopDirectoryOnly),
                                                new DirectoryModuleDiscovery(moduleB, SearchOption.TopDirectoryOnly));
        }

        /// <summary>
        ///     Set Ups two simple modles with versions, makes them into repository.
        /// </summary>
        /// <returns></returns>
        protected IModuleDiscovery SetUpTwoSimpleModulesGetTheirDiscovery()
        {
            string repositoryDir = Path.Combine(TestSpacePath, "Repository");

            // create the modules with specific version (mocking the version provider) and use the special manifest builder
            IModuleDiscovery v0Discovery =
                SetUpModulesWithVersion(NomadConfigurationSettings.ModuleDirectoryPath, "1.0.0.0");

            // prepare module for this test with versions v1 (only of module A and module B) and put them into repository
            IModuleDiscovery v1Discovery = SetUpModulesWithVersion(repositoryDir, "2.0.0.0");

            // putting them into repo
            var updateModuleInfos = new List<ModuleInfo>(v1Discovery.GetModules());
            List<ModuleManifest> updateManifests = (from moduleInfo in updateModuleInfos
                                                    select moduleInfo.Manifest).ToList();

            SetUpModulesRepository(updateManifests,updateModuleInfos);

            return v0Discovery;
        }

        /// <summary>
        ///     Set ups repository using provided manifests.
        /// </summary>
        /// <param name="moduleManifests"></param>
        /// <param name="updateModuleInfos"></param>
        protected void SetUpModulesRepository(List<ModuleManifest> moduleManifests,IEnumerable<ModuleInfo> updateModuleInfos)
        {
            ModulesRepository
                .Setup(x => x.GetAvailableModules())
                .Returns(new AvailableModules(moduleManifests));

            ModulesRepository.Setup(
                x => x.GetModule(It.IsAny<string>()))
                .Returns<string>(name => new ModulePackage
                                             {
                                                 ModuleManifest = updateModuleInfos
                                                     .Where(x => x.Manifest.ModuleName.Equals(name))
                                                     .Select(x => x.Manifest)
                                                     .Single(),

                                                 ModuleZip = GetZippedData(updateModuleInfos,
                                                                           name)
                                             });
        }


        /// <summary>
        ///     Asserts version of provided moduleName with provided version in modules enumerable.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="modules"></param>
        /// <param name="moduleName"></param>
        protected static void AssertVersion(string version, IEnumerable<ModuleInfo> modules,
                                          string moduleName)
        {
            string loadedVersion = modules
                .Where(x => x.Manifest.ModuleName.Equals(moduleName))
                .Select(x => x.Manifest.ModuleVersion)
                .Single()
                .ToString();

            Assert.AreEqual(version, loadedVersion);
        }

        /// <summary>
        ///     Gets the zipped data for module with provided name
        /// </summary>
        /// <param name="updateModuleInfos"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static byte[] GetZippedData(IEnumerable<ModuleInfo> updateModuleInfos,
                                            string name)
        {
            string tmpFile = Path.GetTempFileName();
            using (var zipFile = new ZipFile())
            {
                // get the directory in which we have this module
                string asmPath =
                    updateModuleInfos.Where(x => x.Manifest.ModuleName.Equals(name)).Select(
                        x => x.AssemblyPath).Single();

                DirectoryInfo directoryInfo = new DirectoryInfo(asmPath).Parent;

                // get all files from this directory into zip archive
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    zipFile.AddFile(fileInfo.FullName,".");
                }

                zipFile.Save(tmpFile);
            }

            return File.ReadAllBytes(tmpFile);
        }


        protected static IModulesDependenciesProvider GetModuleDependenciesOnSingleModule(string s, string versionString)
        {
            var moduleAonBDependency = new ModuleDependency()
                                           {
                                               HasLoadingOrderPriority = false,
                                               MinimalVersion = new Version(versionString),
                                               ModuleName = s,
                                               ProcessorArchitecture = ProcessorArchitecture.MSIL
                                           };

            var moduleDependencyProviderMock = new Mock<IModulesDependenciesProvider>(MockBehavior.Loose);
            moduleDependencyProviderMock
                .Setup(x => x.GetDependencyModules(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<ModuleDependency>() { moduleAonBDependency });

            return moduleDependencyProviderMock.Object;
        }


        protected static IVersionProvider GetVersionProviderForVersion(string versionString)
        {
            var mockedVersionProvider = new Mock<IVersionProvider>(MockBehavior.Loose);
            mockedVersionProvider
                .Setup(x => x.GetVersion(It.IsAny<string>()))
                .Returns(new Version(versionString));

            return mockedVersionProvider.Object;
        }
    }
}