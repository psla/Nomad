using System;

namespace InterDomainCommunicationTests.CreateTestObject
{
    public class NormalCreation : ICreateTestObject
    {
        public TestObjectMarshall CreateObject(TestObjectMarshall  testObject, AppDomain appDomain, string exeAssembly)
        {
            return new TestObjectMarshall(); 
        }
    }
}