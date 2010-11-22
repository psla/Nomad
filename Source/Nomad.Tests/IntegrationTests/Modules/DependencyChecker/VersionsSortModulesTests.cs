using System;
using System.Collections.Generic;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;
using Version = Nomad.Utils.Version;

namespace Nomad.Tests.IntegrationTests.Modules.DependencyChecker
{
    [IntegrationTests]
    public class VersionsSortModulesTests : DependencyCheckerBase
    {
        [SetUp]
        public void set_up()
        {
            DependencyChecker = new Nomad.Modules.DependencyChecker();
        }


        [Test]
        public void one_module_has_lower_version_than_required_results_in_exception()
        {
            /* Graph:
             * A  -> B (wrong version) -> C
             */
            var upper = new Version("1.2.0.0");
            var lower = new Version("1.0.0.0");

            var c = SetUpModuleInfoWithVersion("C", upper);
            var b = SetUpModuleInfoWithVersion("B", lower,
                                                      new KeyValuePair<string, Version>("C", lower));
            var a = SetUpModuleInfoWithVersion("A", upper,
                                                      new KeyValuePair<string, Version>("B", upper));

            Modules = new[]
                          {
                              a,
                              b,
                              c
                          };

            // no expected modules but exception
            Assert.Throws<ArgumentException>(() => DependencyChecker.SortModules(Modules),
                                             "The modules should not be possible to sort. Missing version.");
        }

        [Test]
        public void sorting_proper_with_various_versions()
        {
            /*
             * Quite the DAG graph
             *       Q 
             *     /
             *    P -- R
             *   /      \
             *  A        Z
             *   \     /
             *      B 
             * 
             */
            var high = new Version("2.2.37.2");
            var neutral = new Version("1.5.0.0");
            var low = new Version("1.0.0.50");

            var z = SetUpModuleInfoWithVersion("Z", neutral);
            var b = SetUpModuleInfoWithVersion("B", neutral,
                                               new KeyValuePair<string, Version>("Z", neutral));
            var r = SetUpModuleInfoWithVersion("R", neutral,
                                               new KeyValuePair<string, Version>("Z", low));
            var q = SetUpModuleInfoWithVersion("Q", high);
            var p = SetUpModuleInfoWithVersion("P", neutral,
                                               new KeyValuePair<string, Version>("Q", high),
                                               new KeyValuePair<string, Version>("R", low));

            var a = SetUpModuleInfoWithVersion("A", high,
                                               new KeyValuePair<string, Version>("P", neutral),
                                               new KeyValuePair<string, Version>("B", low));

            Modules = new[]
                          {
                              a, p, q, z, b, r,
                          };
            ExpectedModules = new[]
                                  {
                                      q, z, r, p, b, a
                                  };

            Assert.AreEqual(ExpectedModules,DependencyChecker.SortModules(Modules),"This complex case should be possible to sort");

        }

        [Test]
        public void sorting_properly_modules_with_equal_high_versions()
        {
            /*  More complex graph:
             *     P 
             *   /   \
             *  A     Z
             *   \   /
             *     Q
             */

            var version = new Version("2.2.1.4");

            var z = SetUpModuleInfoWithVersion("Z", version);
            var p = SetUpModuleInfoWithVersion("P", version,
                                                      new KeyValuePair<string, Version>("Z", version));
            var q = SetUpModuleInfoWithVersion("Q", version,
                                                      new KeyValuePair<string, Version>("Z", version));

            ModuleInfo a = SetUpModuleInfoWithVersion("A", version,
                                                      new KeyValuePair<string, Version>("P", version),
                                                      new KeyValuePair<string, Version>("Q", version));

            Modules = new[]
                          {
                              a, q, p, z
                          };
            ExpectedModules = new[]
                                  {
                                      z, p, q, a
                                  };

            Assert.AreEqual(ExpectedModules, DependencyChecker.SortModules(Modules),
                            "The list was sorted wrongly.");
        }
    }
}