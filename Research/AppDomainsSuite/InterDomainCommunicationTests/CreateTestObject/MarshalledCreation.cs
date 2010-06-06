using System;

namespace InterDomainCommunicationTests.CreateTestObject
{
    public class MarshalledCreation : ICreateTestObject
    {
        public TestObjectMarshall CreateObject(TestObjectMarshall testObject, AppDomain appDomain, string exeAssembly)
        {
            
            
                // Create an instance of MarshalbyRefType in the second AppDomain. 
                // A proxy to the object is returned.
                testObject = (TestObjectMarshall)appDomain.CreateInstanceAndUnwrap(
                    exeAssembly,
                    typeof(TestObjectMarshall).FullName
                                             );
            

            return testObject;
        }
    }
}