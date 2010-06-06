using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NLog;

namespace InterDomainCommunicationTests
{

    class Program
    {
        /// <summary>
        /// NLog logger for documenting the whole experiment
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(String[] args)
        {
            Logger.Info("Starting program InterDomainCommunicationTests");

            TestRunner  runner  =new TestRunner(1000);
            runner.RunTests();

            Logger.Info("Finishing work of the InterDomainCommunicationTests");

            //For viewing the console
            Console.ReadLine();

        }


    }
}
