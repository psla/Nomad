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
            AvailablePackages = availablePackages;
        }


        public IList<WebModulePackageInfo> AvailablePackages { get; private set; }
    }
}