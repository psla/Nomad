using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace WPFHostApp.DomainLogic
{
    /// <summary>
    /// Class for creating and lunching the another AppDomains.
    /// </summary>
    public class AppDomainLauncher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private List<AppDomain> alreadyStartedDomains;

        public bool IsRunInNewThread { set; get; }
        public bool IsRunInNewDomain { set; get; }

        public AppDomainLauncher()
        {
            alreadyStartedDomains = new List<AppDomain>();
        }

        public void Run(Action<AppDomain> methodRunner)
        {
            AppDomain appDomain = GetLastUsedAppDomain();

            //register events
            appDomain.AssemblyLoad += AssemblyLoadHandler;
            appDomain.DomainUnload += DomainUnloadHandler;

            if(IsRunInNewDomain)
            {
                appDomain = GetNewAppDomain();
            }

            if(IsRunInNewThread)
            {
                ThreadPool.QueueUserWorkItem( (x) => methodRunner(appDomain));
            }
            else
            {
                methodRunner(appDomain);
            }
        }

        private static void AssemblyLoadHandler(object sender,
                                                AssemblyLoadEventArgs args)
        {
            AppDomain ad = sender as AppDomain;
            Debug.Assert(ad == AppDomain.CurrentDomain);
            string a = args.LoadedAssembly.GetName().Name;
            AppDomain c = ad.GetData("Creator") as AppDomain;
            
            logger.Info("Assembly Loaded: '{0}' in '{1}' created by '{2}'", a, ad.FriendlyName, c.FriendlyName);
        }

        private static void DomainUnloadHandler(object sender, EventArgs args)
        {
            AppDomain ad = sender as AppDomain;
            Debug.Assert(ad == AppDomain.CurrentDomain);
            logger.Info("Domain unloaded");
        }

        private AppDomain GetLastUsedAppDomain()
        {
            if(alreadyStartedDomains.Count == 0)
            {
                GetNewAppDomain();
            }
            return alreadyStartedDomains[alreadyStartedDomains.Count -1];
        }

        private AppDomain GetNewAppDomain()
        {
            alreadyStartedDomains.Add(AppDomain.CreateDomain("AppLauncher " + this.alreadyStartedDomains.Count));
            return alreadyStartedDomains[alreadyStartedDomains.Count -1];
        }
    }
}