using System.Collections.Generic;
using System.Reflection;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Modules
{
    [IntegrationTests]
    public class DependencyCheckerTests
    {
        private DependencyChecker _dependencyChecker;
        private IEnumerable<ModuleInfo> _expectedModules;

        private IEnumerable<ModuleInfo> _modules;


        [SetUp]
        public void set_up()
        {
            _dependencyChecker = new DependencyChecker();
        }


        [Test]
        public void sorting_empty_list()
        {
            _modules = new List<ModuleInfo>();
            _expectedModules = new List<ModuleInfo>();

            IEnumerable<ModuleInfo> result = _dependencyChecker.SortModules(_modules);

            Assert.AreEqual(_expectedModules, result);
        }


        private static ModuleDependency SetUpModuleDependencyWithNameOnly(string name)
        {
            return new ModuleDependency
                       {
                           HasLoadingOrderPriority = false,
                           MinimalVersion = new Version(),
                           ModuleName = name,
                           ProcessorArchitecture = ProcessorArchitecture.None
                       };
        }


        private Mock<IModuleManifestFactory> SetUpManifestFactory(string name,
                                                                  List<ModuleDependency>
                                                                      dependencies, Version version)
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


        [Test]
        public void sorting_hard_linked_dag_list()
        {
            /* The graph:
             * A -> B -> C - > X
             *   \            /
             *    > D -> E  /
             */

            _modules = new List<ModuleInfo>
                           {
                               //FIXME: Assembly Path required.
                               new ModuleInfo(string.Empty,
                                              SetUpManifestFactory("A", new List<ModuleDependency>
                                                                            {
                                                                                SetUpModuleDependencyWithNameOnly
                                                                                    ("B"),
                                                                                SetUpModuleDependencyWithNameOnly
                                                                                    ("D")
                                                                            }, new Version()).Object)
                           };
        }


        [Test]
        public void sorting_hard_linked_non_dag_list()
        {
        }


        /* TODO: finish this bit more complex cases
        
        public void sorting_soft_linked_dag_with_two_service_providers_for_one_interface()
        {
            
        }

        public void sorting_soft_linked_dag_with_two_users_of_interface()
        {
            
        }

        public void sorting_complex_case()
        {
            
        }
         
         */
    }
}