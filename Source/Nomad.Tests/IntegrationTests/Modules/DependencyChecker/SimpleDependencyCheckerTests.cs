using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Nomad.Modules;
using Nomad.Modules.Manifest;
using NUnit.Framework;
using TestsShared;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.IntegrationTests.Modules.DependencyChecker
{
    /// <summary>
    ///     Tests are performed with purpose of checking only dependency resolving using 
    ///     <see cref="ModuleManifest.ModuleDependencies"/> of <see cref="ModuleManifest"/> with:
    ///     <see cref="ModuleDependency.ModuleName"/> and <see cref="ModuleDependency.HasLoadingOrderPriority"/> properties.
    /// </summary>
    [IntegrationTests]
    public class SimpleDependencyCheckerTests
    {
        private Nomad.Modules.DependencyChecker _dependencyChecker;

        private IEnumerable<ModuleInfo> _expectedModules;
        private IEnumerable<ModuleInfo> _modules;


        [SetUp]
        public void set_up()
        {
            _dependencyChecker = new Nomad.Modules.DependencyChecker();
        }


        [Test]
        public void sorting_empty_list()
        {
            _modules = new List<ModuleInfo>();
            _expectedModules = new List<ModuleInfo>();

            IEnumerable<ModuleInfo> result = _dependencyChecker.SortModules(_modules);

            Assert.AreEqual(_expectedModules, result);
        }


        [Test]
        public void sorting_hard_linked_dag_list()
        {
            /* The graph:
             * A -> B -> C - > X
             *   \            /
             *    > D -> E  /
             */

            var a = SetUpModuleInfo("A", "B", "D");
            var b = SetUpModuleInfo("B", "C");
            var d = SetUpModuleInfo("D", "E");
            var c = SetUpModuleInfo("C", "X");
            var e = SetUpModuleInfo("E", "X");
            var x = SetUpModuleInfo("X");

            _modules = new List<ModuleInfo>
                           {
                               
                               b,
                               a,
                               d,
                               c,
                               x,
                               e,
                           };

            _expectedModules = new List<ModuleInfo>
                                   {
                                       x,
                                       e,
                                       d,
                                       c,
                                       b,
                                       a
                                   };

            // perform test
            Assert.AreEqual(_expectedModules, _dependencyChecker.SortModules(_modules));
        }


        [Test]
        public void sorting_hard_linked_non_dag_list_results_in_exception()
        {
            /*
             * The non-dag graph:
             * A -> B -> C -> A
             * |
             * > X -> Y
             */
            var a = SetUpModuleInfo("A", "B", "X");
            var b = SetUpModuleInfo("B", "C");
            var c = SetUpModuleInfo("C", "A");
            var e = SetUpModuleInfo("X", "Y");
            var x = SetUpModuleInfo("Y");

            _modules = new List<ModuleInfo>()
                           {
                               a,
                               b,
                               c,
                               e,
                               x
                           };
            _expectedModules = null;

            // perform test , TODO: change the type of exception or the wway  the sorting is done.
            Assert.Throws<Exception>(() => _dependencyChecker.SortModules(_modules));
        }

        #region SetUp of ModuleInfo

        private static ModuleDependency SetUpModuleDependencyWithNameOnly(string name)
        {
            return new ModuleDependency
                       {
                           ModuleName = name,
                           // default values
                           HasLoadingOrderPriority = false,
                           MinimalVersion = new Version(),
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

        
        private static ModuleInfo SetUpModuleInfo(string moduleName,
                                                  params string[] dependenciesNames)
        {
            const string modulePath = @"MODULE_PATH";

            List<ModuleDependency> listOfDependecies =
                dependenciesNames.Select(SetUpModuleDependencyWithNameOnly).ToList();

            Mock<IModuleManifestFactory> mockModuleManifestFactory =
                SetUpManifestFactory(moduleName, listOfDependecies, new Version());
            var moduleInfo = new ModuleInfo(modulePath, mockModuleManifestFactory.Object);

            return moduleInfo;
        }

        #endregion

       
    }
}