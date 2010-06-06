using System;

namespace WPFHostApp.DomainLogic
{
    public interface ICodeToTest
    {
        Action<AppDomain> GetCode();
    }
}