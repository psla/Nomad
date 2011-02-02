using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Nomad.Core;
using Nomad.Modules.Discovery;
using Nomad.Tests.FunctionalTests.Fixtures;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Kernel
{
    [FunctionalTests]
    public class NomadSandboxingTests : ModuleLoadingWithCompilerTestFixture
    {
        private const string dir = @"Modules\Sandboxing";


        [TearDown]
        public void CleanUp()
        {
            Kernel.UnloadModules();
            Directory.Delete(dir, true);
        }


        [SetUp]
        public override void SetUp()
        {
            //empty HERE!
        }


        [Test]
        public void default_configuration_provides_no_security()
        {
            // initialize kernel
            Kernel = new NomadKernel();

            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\FileWritingModule.cs");

            // perform kernel test and assert
            LoadModulesFromDiscovery(new DirectoryModuleDiscovery(dir,
                                                                  SearchOption.TopDirectoryOnly));

            using (StreamReader verificationFile = File.OpenText(@"Modules\Sandboxing\WrittenFile"))
            {
                Assert.AreEqual(verificationFile.ReadLine(),
                                "PermissionSet allows me to write files!");
            }
        }

        [Test]
        public void AppDomain_with_empty_permission_set_throws_security_exception()
        {
            // prepare configuration
            NomadConfiguration configuration = NomadConfiguration.Default;

            //code execution... obvious? should be as default?
            configuration.ModulesDomainPermissions = new PermissionSet(PermissionState.None);
            configuration.ModulesDomainPermissions.AddPermission(
                new SecurityPermission(SecurityPermissionFlag.Execution));



            // initialize kernel
            Kernel = new NomadKernel(configuration);

            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\FileWritingModule.cs");


            LoadModulesFromDiscovery(new DirectoryModuleDiscovery(dir, SearchOption.TopDirectoryOnly));

            // perform kernel test and assert
            Assert.Throws<PolicyException>(
                () => LoadModulesFromDiscovery(new DirectoryModuleDiscovery(dir,
                                                                            SearchOption.
                                                                                TopDirectoryOnly)));
        }

        [Test]
        public void AppDomain_with_manually_given_permission_set_allows_file_write()
        {
            // prepare configuration
            NomadConfiguration configuration = NomadConfiguration.Default;

            //code execution... obvious? should be as default?
            configuration.ModulesDomainPermissions = new PermissionSet(PermissionState.None);
            configuration.ModulesDomainPermissions.AddPermission(
                new SecurityPermission(SecurityPermissionFlag.Execution));


            // access to test file
            configuration.ModulesDomainPermissions.AddPermission(
                new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.GetFullPath(dir)));



            // initialize kernel
            Kernel = new NomadKernel(configuration);

            SetUpModuleWithManifest(dir,
                                    @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\FileWritingModule.cs");


            LoadModulesFromDiscovery(new DirectoryModuleDiscovery(dir, SearchOption.TopDirectoryOnly));

            // perform kernel test and assert
            using (StreamReader verificationFile = File.OpenText(@"Modules\Sandboxing\WrittenFile"))
            {
                Assert.AreEqual(verificationFile.ReadLine(),
                                "PermissionSet allows me to write files!");
            }
        }

    }
}