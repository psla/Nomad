using System;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public interface IInjectableModulesRegistry
    {
        void Register(Type bootstraperType);
    }
}