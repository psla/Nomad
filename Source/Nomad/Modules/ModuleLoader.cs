using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Nomad.Modules
{
    /// <summary>
    ///     Default implementation of <see cref="IModuleLoader"/>
    /// </summary>
    public class ModuleLoader : MarshalByRefObject, IModuleLoader
    {
        private readonly List<ModuleInfo> _loadedModuleInfos = new List<ModuleInfo>();
        private readonly List<IModuleBootstraper> _loadedModules = new List<IModuleBootstraper>();
        private readonly IWindsorContainer _rootContainer;


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleLoader"/> class.
        /// </summary>
        /// <param name="rootContainer">Container that will be used as a root container.
        /// Module's sub-containers will be created based on this container. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="rootContainer"/> is <c>null</c></exception>
        public ModuleLoader(IWindsorContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");

            _rootContainer = rootContainer;
        }

        #region IModuleLoader Members

        /// <summary>Inherited</summary>
        public void LoadModule(ModuleInfo moduleInfo)
        {
            IModuleBootstraper bootstraper;

            try
            {
 
                AssemblyName asmName = AssemblyName.GetAssemblyName(moduleInfo.AssemblyPath);
                Assembly assembly = Assembly.Load(asmName);

                // make sure that all things lazy - loading asm are loaded before changing the bin path
                foreach(var asm in assembly.GetReferencedAssemblies())
                {
                    Console.WriteLine(asm.Name);
                }

                Type bootstraperType = GetBootstrapperType(assembly);

                IWindsorContainer subContainer = CreateSubContainerConfiguredFor(bootstraperType);
                bootstraper = subContainer.Resolve<IModuleBootstraper>();
                bootstraper.OnLoad();
                _loadedModules.Add(bootstraper);
                _loadedModuleInfos.Add(moduleInfo);
            }
            catch (Exception e)
            {
                //TODO: fix this issue
                //_logger.WarnException("Couldn't load assembly", e);
                throw;
                return;
            }
        }


        /// <summary>
        ///     Tries to invoke <see cref="IModuleBootstraper.OnUnLoad"/>  method on each module bootstrapper from set.
        /// </summary>
        /// <remarks>
        ///     Unloads all modules from those having been registered in IoC container.
        /// </remarks>
        public void InvokeUnload()
        {
            foreach (IModuleBootstraper moduleBootstraper in _loadedModules)
            {
                moduleBootstraper.OnUnLoad();
            }
        }


        /// <summary>
        ///     Provides information about loaded modules.
        /// </summary>
        /// <returns>Enumerable collection of <see cref="ModuleInfo"/> concerning modules currently loaded into the application</returns>
        public List<ModuleInfo> GetLoadedModules()
        {
            return _loadedModuleInfos;
        }

        #endregion

        private static Type GetBootstrapperType(Assembly assembly)
        {
            IEnumerable<Type> bootstraperTypes =
                from type in assembly.GetTypes()
                where type.GetInterfaces().Contains(typeof (IModuleBootstraper))
                select type;

            return bootstraperTypes.SingleOrDefault();
        }


        private IWindsorContainer CreateSubContainerConfiguredFor(Type bootstraperType)
        {
            IWindsorContainer subContainer = new WindsorContainer();
            _rootContainer.AddChildContainer(subContainer);

            subContainer.Register(
                Component.For<IModuleBootstraper>().ImplementedBy(bootstraperType)
                );
            return subContainer;
        }
    }
}