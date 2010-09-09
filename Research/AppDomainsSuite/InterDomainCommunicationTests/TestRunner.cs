using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using InterDomainCommunicationTests.CreateTestObject;
using NLog;


namespace InterDomainCommunicationTests
{
    public class TestRunner
    {
        /// <summary>
        /// NLog logger for documenting the whole experiment
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly decimal _repeatNumber = 0;

        private TestObjectMarshall _mbrt;

        /// <summary>
        /// List of creation methods to be invoked
        /// </summary>
        private List<ICreateTestObject> _creators;

        private delegate TestObjectMarshall CreateObjectDelegate(
            TestObjectMarshall testObject, AppDomain appDomain, string exeAssembly);

        public TestRunner(decimal repeatNumber)
        {
            _repeatNumber = repeatNumber;
            _mbrt = null;
        }

        /// <summary>
        /// Prepares the creators, instead of simple factory.
        /// </summary>
        private void PrepareCreators()
        {
            _creators = new List<ICreateTestObject>();
            _creators.Add(new NormalCreation());
            _creators.Add(new MarshalledCreation());
        }

        public  void RunTests()
        {
            // Get and display the friendly name of the default AppDomain.
            string exeAssembly;
            AppDomain ad2;

            PrepareCreators();
            string callingDomainName = SetUpTestEviorment(out exeAssembly, out ad2);
            
            foreach (var creator in _creators)
            {
                //Testing the creation time within the specified creators
                long elapsedCreationTime = 0 ;

                elapsedCreationTime = TimeTestCreation(creator.CreateObject, _mbrt, ad2, exeAssembly);

                Logger.Info("Creating {0} |  " +  elapsedCreationTime + "ms", creator.GetType().Name);

                //Testing the invoking time for all methods in TestObjectMarshall
                //TODO make filtering
                foreach (var methodInfo in typeof(TestObjectMarshall).GetMethods())
                {

                    long elapsedMiliseconds = 0;

                    //Test single method in a TestObjectClass
                    if(methodInfo.GetParameters().Length == 0)
                    {
                        elapsedMiliseconds = TimeTestMethodCall( () => methodInfo.Invoke(_mbrt, new object[] { }));         
                    }

                    if((methodInfo.GetParameters().Length == 1) && (methodInfo.GetParameters()[0].ParameterType.Equals(typeof(String))))
                    {
                       elapsedMiliseconds = TimeTestMethodCall ( () => methodInfo.Invoke(_mbrt, new object[] {callingDomainName}));         
                    }

                    if ((methodInfo.GetParameters().Length == 3) && methodInfo.Name.Equals("SomeMethodWithSimpleParameter"))
                    {
                        elapsedMiliseconds =
                            TimeTestMethodCall(() => methodInfo.Invoke(_mbrt, new object[] {"AlaMakota", 10, 10.0}));
                    }

                    if ((methodInfo.GetParameters().Length == 1) && methodInfo.Name.Equals("SomeMethodWithSerializabeParameter"))
                    {
                        elapsedMiliseconds = TimeTestMethodCall(
                                                                () =>
                                                                methodInfo.Invoke(_mbrt, new object[]
                                                                                             {
                                                                                                 new SerializableClass()
                                                                                                     {
                                                                                                         Field1 =
                                                                                                             "field1",
                                                                                                         Field2 = 1,
                                                                                                         Field3 = 1.0
                                                                                                     }
                                                                                             }));
                    }

                    if ((methodInfo.GetParameters().Length == 1) && methodInfo.Name.Equals("SomeMethodWithProxyEneabledParameter"))
                    {
                        //Performed marshalled creation in domian C

                        TestObjectMarshall objectMarshall = null;
                        objectMarshall = new MarshalledCreation().CreateObject(objectMarshall, AppDomain.CreateDomain("Domain C"), exeAssembly);

                        elapsedMiliseconds = TimeTestMethodCall(
                            ()
                            =>
                            methodInfo.Invoke(_mbrt, new object[] {objectMarshall}));
                    }


                    Logger.Info("Invoking {0} within class {1} |  " + elapsedMiliseconds +"ms", methodInfo.Name,
                                methodInfo.MemberType.GetType().Name);
                }
            }
        }

        private long TimeTestMethodCall(Action action)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            for (int i = 0; i < _repeatNumber; i++)
            {
                action.Invoke();
            }

            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }

        private long TimeTestCreation(CreateObjectDelegate @delegate ,TestObjectMarshall  testObject, AppDomain appDomain, string exeAssembly)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            for (int i = 0; i < _repeatNumber; i++ )
            {
                _mbrt = @delegate.Invoke(testObject, appDomain, exeAssembly);
            }

            _stopwatch.Stop();
            return _stopwatch.ElapsedMilliseconds;
        }

        private  string SetUpTestEviorment(out string exeAssembly, out AppDomain ad2)
        {
            string callingDomainName = Thread.GetDomain().FriendlyName;
            Logger.Debug(callingDomainName);

            // Get and display the full name of the EXE assembly.
            exeAssembly = Assembly.GetEntryAssembly().FullName;
            Logger.Debug(exeAssembly);

            // Construct and initialize settings for a second AppDomain.
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase =
                Environment.CurrentDirectory;
            ads.DisallowBindingRedirects = false;
            ads.DisallowCodeDownload = true;
            ads.ConfigurationFile =
                AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            // Create the second AppDomain.
            ad2 = AppDomain.CreateDomain("AD #2", null, ads);
            return callingDomainName;
        }


    }

}