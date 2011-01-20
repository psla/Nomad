using System;

namespace Nomad.Internationalization
{
    ///<summary>
    /// Provides always the same text
    ///</summary>
    public class FakeResourceSource : IResourceSource
    {
        public string Retrieve(string request)
        {
            return "fa³szywy resource";
        }
    }
}