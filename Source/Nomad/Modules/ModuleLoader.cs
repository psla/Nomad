using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nomad.Communication.EventAggregation;

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
        private readonly IGuiThreadProvider _guiThreadProvider;


        /// <summary>
        ///     Initializes new instance of the <see cref="ModuleLoader"/> class.
        /// </summary>
        /// <param name="rootContainer">Container that will be used as a root container.
        /// Module's sub-containers will be created based on this container. Must not be <c>null</c>.</param>
        /// <param name="guiThreadProvider">Used to execute OnLoad method on module's bootstraper. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="rootContainer"/> or <paramref name="guiThreadProvider"/> is <c>null</c></exception>
        public ModuleLoader(IWindsorContainer rootContainer, IGuiThreadProvider guiThreadProvider)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");
            if (guiThreadProvider == null) throw new ArgumentNullException("guiThreadProvider");

            _rootContainer = rootContainer;
            _guiThreadProvider = guiThreadProvider;
        }

        #region IModuleLoader Members

        /// <summary>Inherited</summary>
        public void LoadModule(ModuleInfo moduleInfo)
        {
            var assembly = LoadAssembly(moduleInfo);
            var bootstraper = CreateBootstraper(assembly);
            ExecuteOnLoad(bootstraper);
            RegisterModule(moduleInfo, bootstraper);
        }

        private Assembly LoadAssembly(ModuleInfo moduleInfo)
        {
            var asmName = AssemblyName.GetAssemblyName(moduleInfo.AssemblyPath);
            return Assembly.Load(asmName);
        }

        private IModuleBootstraper CreateBootstraper(Assembly assembly)
        {
            var bootstraperType = GetBootstrapperType(assembly);
            var subContainer = CreateSubContainerConfiguredFor(bootstraperType);
            return subContainer.Resolve<IModuleBootstraper>();
        }

        private void ExecuteOnLoad(IModuleBootstraper bootstraper)
        {
            using (var waitHandle = new AutoResetEvent(false))
            {
                Action @delegate = () =>
                                       {
                                           bootstraper.OnLoad();
                                           waitHandle.Set();
                                       };
                _guiThreadProvider.RunInGui(@delegate);
                waitHandle.WaitOne();
            }
        }

        private void RegisterModule(ModuleInfo moduleInfo, IModuleBootstraper bootstraper)
        {
            _loadedModules.Add(bootstraper);
            _loadedModuleInfos.Add(moduleInfo);
        }


        /// <summary>
        ///     Tries to invoke <see cref="IModuleBootstraper.OnUnLoad"/>  method on each module bootstrapper from set.
        /// </summary>
        /// <remarks>
        ///     Unloads all modules from those having been registered in IoC container.
        /// </remarks>
        public void InvokeUnload()
        {
            foreach (var moduleBootstraper in _loadedModules)
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