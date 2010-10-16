using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Nomad.Modules
{
    /// <summary>
    ///     Default implementation of <see cref="IModuleLoader"/>
    /// </summary>
    public class ModuleLoader : MarshalByRefObject,IModuleLoader
    {
        private readonly IWindsorContainer _rootContainer;


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleLoader"/> class.
        /// </summary>
        /// <param name="rootContainer">Container that will be used as a root container. Module's sub-containers will be created based on this container. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="rootContainer"/> is <c>null</c></exception>
        public ModuleLoader(IWindsorContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");

            _rootContainer = rootContainer;
        }


        /// <summary>Inherited</summary>
        public void LoadModule(ModuleInfo moduleInfo)
        {
            IModuleBootstraper bootstraper;

            try
            {
                var assembly = Assembly.LoadFile(moduleInfo.AssemblyPath);
                var bootstraperTypes =
                    from type in assembly.GetTypes()
                    where type.GetInterfaces().Contains(typeof (IModuleBootstraper))
                    select type;

                var bootstraperType = bootstraperTypes.SingleOrDefault();

                var subContainer = CreateSubContainerConfiguredFor(bootstraperType);
                bootstraper = subContainer.Resolve<IModuleBootstraper>();
            }
            catch (Exception e)
            {
                //TODO: fix this issue
                //_logger.WarnException("Couldn't load assembly", e);
                //throw;
                return;
            }

            bootstraper.Initialize();
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
    }
}