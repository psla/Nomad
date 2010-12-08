using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nomad.RepositoryServer.Controllers;

namespace Nomad.RepositoryServer.IoC
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Also register all the controller types as transient
            var controllerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof(IController).IsAssignableFrom(t)
                                  select t;
            
            foreach (Type t in controllerTypes)
                container.Register(
                    Component.For(t).ImplementedBy(t).LifeStyle.Transient
                    );
        }
    }
}