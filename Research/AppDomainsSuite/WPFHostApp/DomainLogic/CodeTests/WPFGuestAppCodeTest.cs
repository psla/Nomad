using System;

namespace WPFHostApp.DomainLogic.CodeTests
{
    public class WpfGuestAppCodeTest : ICodeToTest
    {
        public override string ToString()
        {
            return "WPF (3.5) Guest Application ";
        }

        public Action<AppDomain> GetCode()
        {
            return RunAssembly;
        }

        private static void RunAssembly(AppDomain appDomain)
        {
            appDomain.ExecuteAssembly("../../../WPFGuestApp/bin/Debug/WPFGuestApp.exe");
        }
    }
}