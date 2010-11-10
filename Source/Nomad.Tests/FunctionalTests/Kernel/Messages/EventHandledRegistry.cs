using System;
using System.Collections.Generic;

namespace Nomad.Tests.FunctionalTests.Kernel.Messages
{
    public static class EventHandledRegistry
    {
        private readonly static List<Type> _types = new List<Type>();

        public static void RegisterEventType(Type eventType)
        {
            _types.Add(eventType);
        }

        public static IList<Type> GetRegisteredTypes()
        {
            return _types.AsReadOnly();
        }
    }
}