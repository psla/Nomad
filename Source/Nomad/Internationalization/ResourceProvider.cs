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
        private readonly IDictionary<string, IResourceSource> _resourceSources =
            new Dictionary<string, IResourceSource>();


        /// <summary>
        /// Returns resources based on current thread <see cref="CultureInfo"/>
        /// </summary>
        /// <remarks>
        /// 1. Look up into current culture resource source
        /// 2. If not found, return request
        /// <param name="request">resource to find</param>
        /// <returns>Resource or request when no found</returns>
        //TODO: We may add 2nd step return default if not found for current culture
        public string Retrieve(string request)
        {
            IResourceSource source;
            if(_resourceSources.TryGetValue(Thread.CurrentThread.CurrentCulture.Name, out source))
            {
                var resource = source.Retrieve(request);
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
    }
}