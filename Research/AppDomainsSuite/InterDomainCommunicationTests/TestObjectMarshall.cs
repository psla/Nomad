using System;
using System.Threading;
using NLog;

namespace InterDomainCommunicationTests
{
    /// <summary>
    /// Because this class is derived from MarshalByRefObject, a proxy 
    /// to a TestObjectMarshall object can be returned across an AppDomain 
    /// boundary.
    /// </summary>
    public class TestObjectMarshall : MarshalByRefObject
    {

        /// <summary>
        /// Static member should be allocated at the assembly load.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Using method that heavly depends on I/O. Also this method takes one parameter. It is example of the method that just works normally in the program.
        /// Logger here looks as normal I/O operation
        /// </summary>
        /// <param name="callingDomainName">Name of the calling domain.</param>
        public void SomeIOMethod(string callingDomainName)
        {
            // Get this AppDomain's settings and display some of them.
            AppDomainSetup ads = AppDomain.CurrentDomain.SetupInformation;
            Logger.Debug("AppName={0}, AppBase={1}, ConfigFile={2}",
                         ads.ApplicationName,
                         ads.ApplicationBase,
                         ads.ConfigurationFile
                );

            // Display the name of the calling AppDomain and the name 
            // of the second domain.
            // NOTE: The application's thread has transitioned between 
            // AppDomains.
            Logger.Debug("Calling from '{0}' to '{1}'.",
                         callingDomainName,
                         Thread.GetDomain().FriendlyName
                );
        }
        /// <summary>
        /// Simple invocation
        /// </summary>
        /// <param name="simpleParameter"></param>
        /// <param name="simpleParameter2"></param>
        /// <param name="simpleParameter3"></param>
        public void SomeMethodWithSimpleParameter(string simpleParameter, int simpleParameter2, double simpleParameter3)
        {
            //Do something with parameters, casue optimazer might got nasty
            string s = simpleParameter.ToLower();
            int a = ++simpleParameter2;
            double b = simpleParameter3/2.0;

            return;
        }

        /// <summary>
        /// Tests whether serializable objects are passed easier than Marshalled things
        /// </summary>
        /// <param name="parameter"></param>
        public void SomeMethodWithSerializabeParameter(SerializableClass parameter)
        {
            //Do something with this class.
            string s = parameter.ToString();
            return;
        }

        /// <summary>
        /// Test whether marshalled objects can be made
        /// </summary>
        /// <param name="objectMarshall"></param>
        public void SomeMethodWithProxyEneabledParameter(TestObjectMarshall objectMarshall)
        {
            //Do something with this object
            objectMarshall.SomeEmptyMethod();
            objectMarshall.SomeMathMethod();
            objectMarshall.SomeMethodWithSimpleParameter("AlaMakota",9999,2.0);

            return;
        }

        /// <summary>
        /// Call the methods which does only inner mathematic logic.
        /// Should take some time.
        /// </summary>
        public void SomeMathMethod()
        {
            //non-fast (no shifting possible) power function
            Double result = Math.Pow(7, 32);
        }

        /// <summary>
        /// Test just calling the empty method. Simply call overload.
        /// </summary>
        public void SomeEmptyMethod()
        {
            return;
        }
    }
}