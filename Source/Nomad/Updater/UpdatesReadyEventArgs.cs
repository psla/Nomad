using System;
using System.Collections.Generic;

namespace Nomad.Updater
{
    /// <summary>
    /// Contains whole updates packages. Called when all required updates downloaded
    /// </summary>
    public class UpdatesReadyEventArgs : EventArgs
    {
        private readonly List<ModulePackage> _modulePackages;


        ///<summary>
        /// Initializes immutable class with information about ready Update Packages.
        ///</summary>
        ///<param name="modulePackages"></param>
        public UpdatesReadyEventArgs(List<ModulePackage> modulePackages)
        {
            _modulePackages = modulePackages;
        }


        /// <summary>
        /// Update elements. Completely ready.
        /// </summary>
        public IList<ModulePackage> ModulePackages
        {
            get { return _modulePackages.AsReadOnly(); }
        }
    }
}