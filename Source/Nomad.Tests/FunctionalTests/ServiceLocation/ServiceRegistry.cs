using System;
using System.Collections.Generic;

namespace Nomad.Tests.FunctionalTests.ServiceLocation
{
    /// <summary>
    ///     Stores information about running services for tests purposes only.
    /// </summary>
    public class ServiceRegistry
    {
        private static readonly List<Type> _types = new List<Type>();

        private static readonly IDictionary<Type, int> _counter = new Dictionary<Type, int>();


        public static void Register(Type serviceType)
        {
            _types.Add(serviceType);
            _counter[serviceType] = 0;
        }


        public static void IncreaseCounter(Type serviceType)
        {
            _counter[serviceType] += 1;
        }


        public static IList<Type> GetRegisteredServices()
        {
            return _types.AsReadOnly();
        }


        public static IDictionary<Type, int> GetRegisteredServiceCounter()
        {
            return new Dictionary<Type, int>(_counter);
        }


        public static void Clear()
        {
            _types.Clear();
            _counter.Clear();
        }
    }
}