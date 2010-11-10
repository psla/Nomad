using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.IntegrationTests.Modules.DependencyChecker
{
    public class DependencyCheckerBase
    {
        private static readonly Random Random = new Random();
       
        protected Nomad.Modules.DependencyChecker DependencyChecker;

        protected IEnumerable<ModuleInfo> ExpectedModules;
        protected IEnumerable<ModuleInfo> Modules;

        private static ModuleDependency SetUpModuleDependency(string name, Version version)
        {
            return new ModuleDependency
                       {
                           ModuleName = name,
                           HasLoadingOrderPriority = false,
                           // default values
                           MinimalVersion = version,
                           ProcessorArchitecture = ProcessorArchitecture.None
                       };
        }


        private static Mock<IModuleManifestFactory> SetUpManifestFactory(string name,
                                                                         List<ModuleDependency>
                                                                             dependencies,
                                                                         Version version)
        {
            var moduleManifestFacotry = new Mock<IModuleManifestFactory>(MockBehavior.Loose);
            moduleManifestFacotry.Setup(x => x.GetManifest(It.IsAny<ModuleInfo>()))
                .Returns(new ModuleManifest
                             {
                                 Issuer = @"ISSUER_NAME",
                                 ModuleName = name,
                                 ModuleDependencies = dependencies,
                                 ModuleVersion = version
                             }
                );

            return moduleManifestFacotry;
        }


        protected static ModuleInfo SetUpModuleInfo(string moduleName,
                                                    params string[] dependenciesNames)
        {
            const string modulePath = @"MODULE_PATH";

            List<ModuleDependency> listOfDependecies =
                dependenciesNames.Select(x => SetUpModuleDependency(x, new Version())).ToList();

            Mock<IModuleManifestFactory> mockModuleManifestFactory =
                SetUpManifestFactory(moduleName, listOfDependecies, new Version());
            var moduleInfo = new ModuleInfo(modulePath, mockModuleManifestFactory.Object);

            return moduleInfo;
        }


        protected static ModuleInfo SetUpModuleInfoWithVersion(string moduleName, Version version,
                                                               params
                                                                   KeyValuePair<string, Version>[]
                                                                   dependenciesNamesAndVersions)
        {
            // make sure that assemblies are not the same beacause of the path.
            string modulePath = @"SAMPLE_MODULE_PATH_WITH_VERSION" + Random.Next(0, 100);

            var moduleManifestFacotry = new Mock<IModuleManifestFactory>(MockBehavior.Loose);
            moduleManifestFacotry.Setup(x => x.GetManifest(It.IsAny<ModuleInfo>()))
                .Returns(new ModuleManifest
                             {
                                 Issuer = @"ISSUER_NAME",
                                 ModuleName = moduleName,
                                 ModuleDependencies = SetUpDependenciesWithVersion(dependenciesNamesAndVersions),
                                 ModuleVersion = version
                             }
                );

            return new ModuleInfo(modulePath, moduleManifestFacotry.Object);
        }


        private static List<ModuleDependency> SetUpDependenciesWithVersion(
            IEnumerable<KeyValuePair<string, Version>> dependenciesNamesAndVersions)
        {
            return
                dependenciesNamesAndVersions.Select(
                    nameAndVersion =>
                    SetUpModuleDependency(nameAndVersion.Key, nameAndVersion.Value)).ToList();
        }
    }
}