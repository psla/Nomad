using System;

namespace Nomad.ServiceLocation
{
    /// <summary>
    ///     Default Nomad Implementation of IServiceLocator based on Castle Windsor IoC Container.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        #region Implementation of IServiceLocator

        public void Register<T>(T serviceProvider)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}