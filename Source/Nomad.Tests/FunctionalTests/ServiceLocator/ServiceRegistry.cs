using System;
using System.Collections.Generic;

namespace Nomad.Tests.FunctionalTests.ServiceLocator
{
    /// <summary>
    ///     Stores information about running services for tests purposes only.
    /// </summary>
    public class ServiceRegistry
    {
        private static readonly List<Type> _types = new List<Type>();

        public static void Register(Type serviceType)
        {
            _types.Add(serviceType);
        }

        public static IList<Type> GetRegisteredServices()
        {
            return _types.AsReadOnly();
        }

        public static void Clear()
        {
            if(_types != null)
                _types.Clear();
        }
    }
}