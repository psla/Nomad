using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Nomad.Modules
{
    /// <summary>
    /// Module loading logic. 
    /// </summary>
    public class ModuleLoader
    {
        private readonly IWindsorContainer _rootContainer;

        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleLoader"/> class.
        /// </summary>
        /// <param name="rootContainer">
        ///     Windsor container that will be used as a root container for all modules.
        ///     Modules will be setup using child container of this container.
        /// </param>
        public ModuleLoader(IWindsorContainer rootContainer)
        {
            _rootContainer = rootContainer;
        }


        /// <summary>
        /// Loades module from file.
        /// </summary>
        /// <param name="s">Path to module</param>
        public void LoadModuleFromFile(string s)
        {
            try
            {
                var assembly = Assembly.LoadFile(s);
                var bootstraperTypes =
                    from type in assembly.GetTypes()
                    where type.GetInterfaces().Contains(typeof (IModuleBootstraper))
                    select type;

                var bootstraperType = bootstraperTypes.SingleOrDefault();

                var subContainer = CreateSubContainerConfiguredFor(bootstraperType);
                var bootstraper = subContainer.Resolve<IModuleBootstraper>();
                bootstraper.Initialize();
            }
            catch (Exception e)
            {
                //_logger.WarnException("Couldn't load assembly", e);
            }
        }


        private IWindsorContainer CreateSubContainerConfiguredFor(Type bootstraperType)
        {
            var subContainer = new WindsorContainer();
            _rootContainer.AddChildContainer(subContainer);

            subContainer.Register(
                Component.For<IModuleBootstraper>().ImplementedBy(bootstraperType)
                );
            return subContainer;
        }


        /// <summary>
        /// Loades all modules from provided directory path.
        /// </summary>
        /// <param name="libraryPaths">Path to directory</param>
        public void LoadModulesFromDirectory(string libraryPaths)
        {
            var files = Directory.GetFiles(libraryPaths);
            foreach (var file in files)
            {
                this.LoadModuleFromFile(file);
            }
        }
    }
}