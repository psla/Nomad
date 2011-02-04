using System.Resources;
using System.Threading;

namespace Nomad.Internationalization
{
    ///<summary>
    /// Provides resources from <see cref="ResourceManager"/>
    ///</summary>
    public class ResourceManagerResourceSource : IResourceSource
    {
        private readonly ResourceManager _resourceManager;


        public ResourceManagerResourceSource(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        #region IResourceSource Members

        public object Retrieve(string request)
        {
            try
            {
                return _resourceManager.GetString(request, Thread.CurrentThread.CurrentUICulture);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}