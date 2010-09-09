using System;

namespace InterDomainCommunicationTests
{
    [Serializable]
    public class SerializableClass
    {
        public string Field1 { get; set; }
        public int Field2 { get; set; }
        public double Field3 { get; set; }

        public override string ToString()
        {
            return Field1.ToLower() + Field2.ToString() + Field3.ToString();
        }
    }
}