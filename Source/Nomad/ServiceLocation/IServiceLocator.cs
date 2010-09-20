namespace Nomad.ServiceLocation
{
    /// <summary>
    ///     Provides means for registering and resolving Services of the specyfic interface.
    /// </summary>
    /// <remarks>
    ///     Allows only One Service of the interface to be registered at the time.
    /// </remarks>
    public interface IServiceLocator
    {
        /// <summary>
        ///     Register passed object as and set it as serviceProvider for provided interface T.
        /// </summary>
        /// <typeparam name="T">Interface of provided service.</typeparam>
        /// <param name="serviceProvider">Service implementation, thus service provider.</param>
        void Register<T>(T serviceProvider);

        /// <summary>
        ///     Gets the object fullfilling the specified service interface.
        /// </summary>
        /// <typeparam name="T">Service type wanted.</typeparam>
        /// <returns>Service Provider</returns>
        T Resolve<T>();
    }
}