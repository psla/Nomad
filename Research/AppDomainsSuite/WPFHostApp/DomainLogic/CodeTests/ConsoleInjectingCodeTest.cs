using System;

namespace WPFHostApp.DomainLogic.CodeTests
{
    public class ConsoleInjectingCodeTest : ICodeToTest
    {
        public override string ToString()
        {
            return "Console Injecting";
        }

        public Action<AppDomain> GetCode()
        {
            return MethodToBeRun;
        }

        private static void MethodToBeRun(AppDomain appDomain)
        {
            appDomain.DoCallBack(() =>
            {
                AppDomain myDomain = AppDomain.CurrentDomain;

                Console.WriteLine("Im in '{0}' domain", myDomain.FriendlyName);

                for (int i = 0; i < 10000000; i++)
                {
                    Console.WriteLine("Im in domain in {0}", myDomain.GetHashCode());
                }

                //Accessing outerscope varible, not possible in AppDomain, will generate error.
                // Console.WriteLine(abc);

                //For simple hang up.
                Console.ReadLine();

            }
               );
        }
    }
}