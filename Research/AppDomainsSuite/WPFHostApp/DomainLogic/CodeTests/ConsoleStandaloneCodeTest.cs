using System;

namespace WPFHostApp.DomainLogic.CodeTests
{
    public class ConsoleStandaloneCodeTest : ICodeToTest
    {
        public override string ToString()
        {
            return "Console StandAlone";
        }

        public Action<AppDomain> GetCode()
        {
            return RunAssembly;
        }

        private static void RunAssembly(AppDomain appDomain)
        {
            appDomain.ExecuteAssembly("../../../LoadedAppDomain/bin/Debug/LoadedAppDomain.exe");
        }
    }
}