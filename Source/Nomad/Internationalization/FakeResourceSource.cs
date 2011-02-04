using System;

namespace Nomad.Internationalization
{
    ///<summary>
    /// Provides always the same text
    ///</summary>
    public class FakeResourceSource : IResourceSource
    {
        public object Retrieve(string request)
        {
            return "Fake resource";
        }
    }
}