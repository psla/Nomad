using System;
using System.Collections.Generic;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public static class LoadedModulesRegistry
    {
        private static readonly List<Type> _types = new List<Type>();


        public static void Register(Type bootstraperType)
        {
            _types.Add(bootstraperType);
        }


        public static IList<Type> GetRegisteredModules()
        {
            return _types.AsReadOnly();
        }


        public static void Clear()
        {
            _types.Clear();
        }
    }
}