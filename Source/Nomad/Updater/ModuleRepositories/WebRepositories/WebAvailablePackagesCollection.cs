using System;
using System.Collections.Generic;

namespace Nomad.Updater.ModuleRepositories.WebRepositories
{
    /// <summary>
    /// Runtime representation of client pulled available packages collection.
    /// </summary>
    [Serializable]
    public class WebAvailablePackagesCollection
    {
        public WebAvailablePackagesCollection(IList<WebModulePackageInfo> availablePackages)
        {
            AvailablePackages = new List<WebModulePackageInfo>(availablePackages);
        }

        /// <summary>
        ///     Constructor for XML serialization.
        /// </summary>
        public WebAvailablePackagesCollection()
        {
            
        }


        public List<WebModulePackageInfo> AvailablePackages { get;  set; }
    }
}