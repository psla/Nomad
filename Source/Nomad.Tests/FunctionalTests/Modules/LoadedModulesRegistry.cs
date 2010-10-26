using System;
using System.Collections.Generic;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public static class LoadedModulesRegistry
    {
        private static readonly List<Type> _types = new List<Type>();

        private static readonly List<Type> _unloadedTypes = new List<Type>();


        public static void Register(Type bootstraperType)
        {
            _types.Add(bootstraperType);
        }


        public static void UnRegister(Type bootstraperType)
        {
            _unloadedTypes.Add(bootstraperType);
        }


        public static IList<Type> GetRegisteredModules()
        {
            return _types.AsReadOnly();
        }


        public static IList<Type> GetUnRegisteredModules()
        {
            return _unloadedTypes.AsReadOnly();
        }


        public static void Clear()
        {
            _types.Clear();
            _unloadedTypes.Clear();
        }
    }
}