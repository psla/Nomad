using System.Collections.Generic;
using Nomad.Modules;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Modules.DependencyChecker
{
    /// <summary>
    ///     It is considered that local list is coherent in terms of the dependencies 
    ///     all dependencies are satisfied in the local list.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Notation used in tests. v0,v1,v2 are generations of versions. Thus v0 lower than v1 etc.
    ///     </para>
    ///     <para>
    ///         Arrow symbol (->) defines relation of precedence. A -> (requires) B to work. Especially:
    ///     A(v1) -> B(v2) : A with version v1 requires B to work which has version v2. If version is omitted then versions meet requirements.
    ///     A(v1) v2-> B(v3) : A with version v1 requires B with version v2 to work. B has v3 version.
    ///     </para>
    /// </remarks>
    [IntegrationTests]
    public class CheckModulesTest : DependencyCheckerBase
    {
        private bool _resultBool;
        private IEnumerable<ModuleInfo> _resultNonValidModules;
        private IEnumerable<ModuleInfo> _updateModules;

        #region Versions

        private Version _v0;
        private Version _v1;
        private Version _v2;
        private Version _v3;

        #endregion

        [SetUp]
        public void set_up()
        {
            DependencyChecker = new Nomad.Modules.DependencyChecker();
            _resultNonValidModules = null;
            _resultBool = false;

            // set up versions
            _v0 = new Version("1.0.0.1");
            _v1 = new Version("1.1.0.1");
            _v2 = new Version("1.2.0.1");
            _v3 = new Version("1.3.0.1");
        }


        private static void PrepareChainWithVersion(Version version, out ModuleInfo a1,
                                                    out ModuleInfo b1, out ModuleInfo c1)
        {
            a1 = SetUpModuleInfoWithVersion("A", version,
                                            new KeyValuePair<string, Version>("B", version));
            b1 = SetUpModuleInfoWithVersion("B", version,
                                            new KeyValuePair<string, Version>("C", version));
            c1 = SetUpModuleInfoWithVersion("C", version);
        }


        private void AssertSuccess()
        {
            Assert.IsTrue(_resultBool);
            Assert.IsFalse(_resultNonValidModules.GetEnumerator().MoveNext());
        }


        private void PerformTest()
        {
            _resultBool = DependencyChecker.CheckModules(Modules, _updateModules,
                                                         out _resultNonValidModules);
        }

        #region Basic

        [Test]
        public void update_list_is_empty_local_must_be_feasible()
        {
            /*
             * Local:
             * A -> B -> C
             * Remote:
             * ...
             * 
             * Result: Success
             */

            ModuleInfo a = SetUpModuleInfoWithVersion("A", _v0,
                                                      new KeyValuePair<string, Version>("B", _v0));
            ModuleInfo b = SetUpModuleInfoWithVersion("B", _v0,
                                                      new KeyValuePair<string, Version>("C", _v0));
            ModuleInfo c = SetUpModuleInfoWithVersion("C", _v0);

            Modules = new[] {a, b, c};
            _updateModules = new ModuleInfo[] {};

            PerformTest();

            AssertSuccess();
        }


        [Test]
        public void update_list_is_fully_disjointed_with_local_list()
        {
            /*
             * Local:
             * A -> B 
             * 
             * Update:
             * X -> Y
             * 
             * Result: Success
             */

            ModuleInfo a = SetUpModuleInfoWithVersion("A", _v1,
                                                      new KeyValuePair<string, Version>("B",
                                                                                        _v1));
            ModuleInfo b = SetUpModuleInfoWithVersion("B", _v1);

            ModuleInfo x = SetUpModuleInfoWithVersion("X", _v2,
                                                      new KeyValuePair<string, Version>("Y",
                                                                                        _v2));
            ModuleInfo y = SetUpModuleInfoWithVersion("Y", _v2);

            Modules = new[] {a, b};
            _updateModules = new[] {x, y};

            PerformTest();

            AssertSuccess();
        }


        [Test]
        public void update_list_is_newer_version_of_local_list()
        {
            /*
             * Local
             * A(v1) -> B(v1)
             * 
             * Remote:
             * A(v2) -> B(v2)
             * 
             * Result: Success
             */

            ModuleInfo a1 = SetUpModuleInfoWithVersion("A", _v1,
                                                       new KeyValuePair<string, Version>("B", _v1));
            ModuleInfo b1 = SetUpModuleInfoWithVersion("B", _v1);

            ModuleInfo a2 = SetUpModuleInfoWithVersion("A", _v2,
                                                       new KeyValuePair<string, Version>("B", _v2));
            ModuleInfo b2 = SetUpModuleInfoWithVersion("B", _v2);

            Modules = new[] {a1, b1};
            _updateModules = new[] {a2, b2};

            PerformTest();

            AssertSuccess();
        }

        #endregion

        #region Success Complex Tests

        [Test]
        public void update_list_has_newer_memebers_than_some_in_local_list()
        {
            /*
             * Local
             * A(v1) -> B(v1) -> C(v1)
             * 
             * Remote:
             * C(v2)
             * Result: Success
             */

            ModuleInfo c1;
            ModuleInfo a1;
            ModuleInfo b1;

            PrepareChainWithVersion(_v1, out a1, out b1, out c1);

            ModuleInfo c2 = SetUpModuleInfoWithVersion("C", _v2);

            Modules = new[] {a1, b1, c1};
            _updateModules = new[] {c2};

            PerformTest();

            AssertSuccess();
        }


        [Test]
        public void update_list_has_member_with_dependency_on_newer_existing_element()
        {
            /*
             * Local
             * A(v1) v1-> B(v1) v1-> C(v1)
             * 
             * Remote:
             * A(v2) v2-> B(v2)
             * 
             * Result: Success
             */

            ModuleInfo c1;
            ModuleInfo a1;
            ModuleInfo b1;

            PrepareChainWithVersion(_v1, out a1, out b1, out c1);
            Modules = new[] {a1, b1, c1};

            ModuleInfo a2 = SetUpModuleInfoWithVersion("A", _v2,
                                                       new KeyValuePair<string, Version>("B", _v2));
            ModuleInfo b2 = SetUpModuleInfoWithVersion("B", _v2);

            _updateModules = new[] {a2, b2};

            PerformTest();

            AssertSuccess();
        }


        [Test]
        public void update_list_has_member_with_dependency_on_existing_element_which_is_already_used
            ()
        {
            /*
             * Local
             * Z(v1) v1 ->B(v1)
             * A(v1) v1-> B(v1) v1-> C(v1)
             * 
             * Remote:
             * A(v2) v2-> B(v2)
             * 
             * Result: Success
             */
            ModuleInfo c1;
            ModuleInfo a1;
            ModuleInfo b1;
            ModuleInfo z1 = SetUpModuleInfoWithVersion("Z", _v1,
                                                       new KeyValuePair<string, Version>("B", _v1));

            PrepareChainWithVersion(_v1, out a1, out b1, out c1);
            Modules = new[] {a1, b1, c1, z1};

            ModuleInfo a2 = SetUpModuleInfoWithVersion("A", _v2,
                                                       new KeyValuePair<string, Version>("B", _v2));
            ModuleInfo b2 = SetUpModuleInfoWithVersion("B", _v2);

            _updateModules = new[] {a2, b2};

            PerformTest();

            AssertSuccess();
        }

        #endregion

        #region Failure Complex Tests

        [Test]
        public void update_list_has_newer_members_with_dependency_missing()
        {
            /*
             * Local:
             * A(v1) -> B(v1) -> C(v1)
             * 
             *Remote:
             * B(v2) v2-> ...
             * with dependency on C(v2) which is missing from update list
             *
             * Result: Failure
             */

            ModuleInfo c1;
            ModuleInfo a1;
            ModuleInfo b1;

            PrepareChainWithVersion(_v1, out a1, out b1, out c1);

            ModuleInfo b2 = SetUpModuleInfoWithVersion("B", _v2,
                                                       new KeyValuePair<string, Version>("C", _v2));

            Modules = new[] {a1, b1, c1};
            _updateModules = new[] {b2};

            PerformTest();

            ExpectedModules = new[] {b2};

            Assert.IsFalse(_resultBool);
            Assert.AreEqual(ExpectedModules, _resultNonValidModules);
        }


        [Test]
        public void update_list_has_multiple_missing_dependencies()
        {
            /*
             * Local:
             * A(v1) v1-> B(v1)
             * X(v1) v1-> Y(v1)
             * 
             * Remote:
             * A(v2) v2-> ...
             * X(v2) v2-> ...
             * 
             * Result: failure
             */

            ModuleInfo a1 = SetUpModuleInfoWithVersion("A", _v1,
                                                       new KeyValuePair<string, Version>("B", _v1));
            ModuleInfo b1 = SetUpModuleInfoWithVersion("B", _v1);

            ModuleInfo x1 = SetUpModuleInfoWithVersion("X", _v1,
                                                       new KeyValuePair<string, Version>("Y", _v1));
            ModuleInfo y1 = SetUpModuleInfoWithVersion("Y", _v1);

            Modules = new[] {a1, b1, x1, y1};

            ModuleInfo a2 = SetUpModuleInfoWithVersion("A", _v2,
                                                       new KeyValuePair<string, Version>("B", _v2));
            ModuleInfo x2 = SetUpModuleInfoWithVersion("X", _v2,
                                                       new KeyValuePair<string, Version>("Y", _v2));

            _updateModules = new[] {a2, x2};

            ExpectedModules = new[] {a2, x2};

            PerformTest();

            Assert.IsFalse(_resultBool);
            Assert.AreEqual(ExpectedModules, _resultNonValidModules);
        }


        [Test]
        public void update_list_closes_the_cycle_with_additional_module()
        {
            /*
             * Local:
             * A(v1) v1-> B(v1) v1 -> C(v1)
             * 
             * Remote:
             * C(v2) v2-> D(v2) v0-> A(v1) from local
             * 
             * Result: failure
             */

            ModuleInfo c1;
            ModuleInfo a1;
            ModuleInfo b1;

            PrepareChainWithVersion(_v1, out a1, out b1, out c1);

            ModuleInfo c2 = SetUpModuleInfoWithVersion("C", _v2,
                                                       new KeyValuePair<string, Version>("D", _v2));
            ModuleInfo d2 = SetUpModuleInfoWithVersion("D", _v2,
                                                       new KeyValuePair<string, Version>("A", _v0));

            Modules = new[] {a1, b1, c1};
            _updateModules = new[] {c2, d2};

            // TODO:  should it be empty list ? or we should put what ?
            ExpectedModules = new ModuleInfo[] {};

            PerformTest();

            Assert.IsFalse(_resultBool);
            Assert.AreEqual(ExpectedModules, _resultNonValidModules);
        }

        #endregion
    }
}