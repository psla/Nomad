using System;

namespace InterDomainCommunicationTests.CreateTestObject
{
    public interface ICreateTestObject
    {
        TestObjectMarshall CreateObject(TestObjectMarshall testObject, AppDomain appDomain, string exeAssembly);
    }
}