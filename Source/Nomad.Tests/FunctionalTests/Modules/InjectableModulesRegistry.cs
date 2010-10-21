using System;
using System.Collections.Generic;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public class InjectableModulesRegistry : IInjectableModulesRegistry
    {
        private readonly List<Type> _types = new List<Type>();


        public void Register(Type bootstraperType)
        {
            _types.Add(bootstraperType);
        }


        public IList<Type> GetRegisteredModules()
        {
            return _types.AsReadOnly();
        }
    }
}