using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ManagingAppDomain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World to application domain managing thing");

            Console.WriteLine("My name  is: " + AppDomain.CurrentDomain.FriendlyName);

            AppDomain appDomain = AppDomain.CreateDomain("Playground Domain");
            
            appDomain.AssemblyLoad += AssemblyLoadHandler;
            appDomain.DomainUnload += DomainUnloadHandler;

            //appDomain.ExecuteAssembly("../../../LoadedAppDomain/bin/Debug/LoadedAppDomain.exe");
            appDomain.DoCallBack( () =>
                                     {
                                         AppDomain myDomain = AppDomain.CurrentDomain;
                                         Console.WriteLine("Im in '{0}' domain", myDomain.FriendlyName);

                                     }
                                     );


            AppDomain.Unload(appDomain);

            Console.ReadLine();

        }

        private static void AssemblyLoadHandler(object sender, 
                                                 AssemblyLoadEventArgs args) 
      {
         AppDomain ad = sender as AppDomain;
         Debug.Assert(ad == AppDomain.CurrentDomain);
         string a = args.LoadedAssembly.GetName().Name;         
         AppDomain c = ad.GetData("Creator") as AppDomain;
         Console.WriteLine("Assembly Loaded: '{0}' in '{1}' created by '{2}'", a, ad.FriendlyName, c.FriendlyName);
      }

        private static void DomainUnloadHandler(object sender, EventArgs args) 
      {
         AppDomain ad = sender as AppDomain;
         Debug.Assert(ad == AppDomain.CurrentDomain);    
         Console.WriteLine("Domain unloaded");
      }
    }
}
