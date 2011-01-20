using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Nomad.Internationalization
{
    /// <summary>
    /// Provides resources in current thread language
    /// </summary>
    /// <remarks>
    /// Any member of this is not thread safe (although some reads may be concurrent)
    /// </remarks>
    public class ResourceProvider
    {
        ///<summary>
        /// Contains current instance of resource provider
        ///</summary>
        public static readonly ResourceProvider CurrentResourceProvider = new ResourceProvider();

        private readonly IDictionary<string, IResourceSource> _resourceSources =
            new Dictionary<string, IResourceSource>();


        private ResourceProvider()
        {
        }


        ///<summary>
        /// Invoked when culture changes
        ///</summary>
        public event EventHandler<EventArgs> CultureChanged;


        internal void InvokeCultureChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = CultureChanged;
            if (handler != null) handler(this, e);
        }


        /// <summary>
        /// Returns resources based on current thread <see cref="CultureInfo"/>
        /// </summary>
        /// <remarks>
        /// 1. Look up into current UI culture resource source
        /// 2. If not found, return request
        /// <param name="request">resource to find</param>
        /// <returns>Resource or request when no found</returns>
        //TODO: We may add 2nd step return default if not found for current culture
        public string Retrieve(string request)
        {
            IResourceSource source;
            if (_resourceSources.TryGetValue(Thread.CurrentThread.CurrentUICulture.Name, out source))
            {
                string resource = source.Retrieve(request);
                if (null != resource)
                    return resource;
            }
            return request;
        }


        ///<summary>
        /// Adds resource source in specific culture name
        ///</summary>
        /// <remarks>
        /// The same resource may be added many times to vary culture names</remarks>
        ///<param name="cultureName">culture name for which resource should be used</param>
        ///<param name="resourceSource">source to use for specified language</param>
        /// <exception cref="ArgumentException">when provided <see cref="cultureName"/> was already registered</exception>
        public void AddSource(string cultureName, IResourceSource resourceSource)
        {
            if (_resourceSources.ContainsKey(cultureName))
                throw new ArgumentException(
                    "There is already defined resource source for provided culture name",
                    "cultureName");

            _resourceSources.Add(cultureName, resourceSource);
        }


        /// <summary>
        /// Changes UI Culture of current thread and notifies its children
        /// </summary>
        /// <param name="newUiCulture"></param>
        public void ChangeUiCulture(CultureInfo newUiCulture)
        {
            Thread.CurrentThread.CurrentUICulture = newUiCulture;
            InvokeCultureChanged(EventArgs.Empty);
        }


        /// <summary>
        /// Resets state of resource provider
        /// </summary>
        public void Reset()
        {
            _resourceSources.Clear();
            InvokeCultureChanged(EventArgs.Empty);
        }
    }
}