using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace Nomad.RepositoryServer.IoC
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer _windsorContainer;


        public WindsorControllerFactory(IWindsorContainer windsorContainer)
        {
            _windsorContainer = windsorContainer;
        }


        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return null;

            return (IController) _windsorContainer.Resolve(controllerType);
        }
    }
}