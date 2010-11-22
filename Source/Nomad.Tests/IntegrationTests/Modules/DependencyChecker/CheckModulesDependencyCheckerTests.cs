using System.Collections.Generic;
using Nomad.Modules;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Modules.DependencyChecker
{
    /// <summary>
    ///     It is considered that local list is coherent in terms of the dependencies 
    ///     all depenencies are satisfied in the local list.
    /// </summary>
    [IntegrationTests]
    public class CheckModulesDependencyCheckerTests : DependencyCheckerBase
    {
        private IEnumerable<ModuleInfo> _resultModules;
        private bool _resultBool;
        private IEnumerable<ModuleInfo> _updateModules;

        [SetUp]
        public void set_up()
        {
            DependencyChecker = new Nomad.Modules.DependencyChecker();
            _resultModules = null;
            _resultBool = false;
        }

        #region Simple Tests

        [Test]
        public void update_list_is_empty_local_must_be_feasible()
        {
            var version = new Version("1.0.0.0");

            var a = SetUpModuleInfoWithVersion("A", version,
                                               new KeyValuePair<string, Version>("B", version));
            var b = SetUpModuleInfoWithVersion("B", version,
                                               new KeyValuePair<string, Version>("C", version));
            var c = SetUpModuleInfoWithVersion("C", version);

            Modules = new[] {a, b, c};
            _updateModules = new ModuleInfo[] {};


            _resultBool = DependencyChecker.CheckModules(Modules, _updateModules, out _resultModules);

            Assert.IsTrue(_resultBool);
            Assert.IsFalse(_resultModules.GetEnumerator().MoveNext());
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
             */

            var localVersion = new Version("1.0.0.0");
            var remoteVersion = new Version("2.3.5.3");

            var a = SetUpModuleInfoWithVersion("A", localVersion,
                                               new KeyValuePair<string, Version>("B", localVersion));
            var b = SetUpModuleInfoWithVersion("B", localVersion);

            var x = SetUpModuleInfoWithVersion("X", remoteVersion,
                                               new KeyValuePair<string, Version>("Y", remoteVersion));
            var y = SetUpModuleInfoWithVersion("Y", remoteVersion);

            Modules = new[] {a, b};
            _updateModules = new[] {x, y};

            _resultBool = DependencyChecker.CheckModules(Modules, _updateModules, out _resultModules);

            Assert.IsTrue(_resultBool);
            Assert.IsFalse(_resultModules.GetEnumerator().MoveNext());
        }

        [Test]
        public void update_list_is_newer_version_of_local_list()
        {
            /*
             * Local
             * A (v1)-> B(v1)
             * 
             * Remote:
             * A(v2) -> B(v2)
             * 
             * with v1 < v2
             */

            var v1 = new Version("1.0.0.0");
            var v2 = new Version("2.0.0.0");

            var a1 = SetUpModuleInfoWithVersion("A", v1, new KeyValuePair<string, Version>("B", v1));
            var b1 = SetUpModuleInfoWithVersion("B", v1);

            var a2 = SetUpModuleInfoWithVersion("A", v2, new KeyValuePair<string, Version>("B", v2));
            var b2 = SetUpModuleInfoWithVersion("B", v2);

            Modules = new[] {a1, b1};
            _updateModules = new[] {a2, b2 };

            _resultBool = DependencyChecker.CheckModules(Modules, _updateModules, out _resultModules);

            Assert.IsTrue(_resultBool);
            Assert.IsFalse(_resultModules.GetEnumerator().MoveNext());

        }

        #endregion

        //TODO: implement the cross list depenendeicies with various versions

        #region Complex Tests

        [Test]
        public void update_list_has_newer_memebers_than_some_in_local_list()
        {
            
        }

        [Test]
        public void update_list_has_older_members_than_some_in_local_list()
        {

        }

        #endregion

    }
}