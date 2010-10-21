using Nomad.Modules.Manifest;
using Nomad.Utils;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Modules
{
    [UnitTests]
    public class VersionConverting
    {
        [Test]
        public void creates_valid_version_from_valid_version_string()
        {
            Version version = new Version("12.34.56.78");
            Assert.AreEqual(12, version.Major, "Major number is not correctly parsed");
            Assert.AreEqual(34, version.Minor, "Minor number is not correctly parsed");
            Assert.AreEqual(56, version.Build, "Build number is not correctly parsed");
            Assert.AreEqual(78, version.Revision, "Major number is not correctly parsed");
        }


        [Test]
        public void creates_valid_version_from_system_version()
        {
            System.Version version = new System.Version(78, 56, 34, 12);
            Version serializableVersion = new Version(version);
            Assert.AreEqual(version.Major, serializableVersion.Major,
                            "Major number was not correctly parsed");
            Assert.AreEqual(version.Minor, serializableVersion.Minor,
                            "Minor number was not correctly parsed");
            Assert.AreEqual(version.Build, serializableVersion.Build,
                            "Build number was not correctly parsed");
            Assert.AreEqual(version.Revision, serializableVersion.Revision,
                            "Revision number was not correctly parsed");
        }


        [Test]
        public void returns_valid_system_version()
        {
            var serializableVersion = new Version(1, 2, 3, 4);
            var systemVersion = serializableVersion.GetSystemVersion();
            Assert.AreEqual(1, systemVersion.Major, "Major number is not correct");
            Assert.AreEqual(2, systemVersion.Minor, "Minor number is not correct");
            Assert.AreEqual(3, systemVersion.Build, "Build number is not correct");
            Assert.AreEqual(4, systemVersion.Revision, "Revision number is not correct");
        }
    }
}