using System;
using Nomad.Core;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Kernel
{
    [IntegrationTests]
    public class AppDomainManagment
    {
        private NomadKernel _nomadKernel;

        [SetUp]
        public void set_up()
        {
            _nomadKernel = new NomadKernel();
        }

        [Test]
        public void creating_module_appdomain_upon_module_loading()
        {
            
        }

        [Test]
        public void unloading_module_appdomain_upon_unload_request()
        {
            
        }

        [Test]
        public void verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded()
        {
            
        }


    }
}