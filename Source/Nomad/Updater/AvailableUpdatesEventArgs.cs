using System;
using System.Collections.Generic;
using Nomad.Modules.Manifest;

namespace Nomad.Updater
{
    /// <summary>
    /// Contains information about updates that are available in repository.
    /// </summary>
    /// <remarks>
    /// Basing on this information you may call <see cref="Updater.PrepareUpdate"/> which will download all available updates
    /// </remarks>
    public class AvailableUpdatesEventArgs : EventArgs
    {
        private readonly List<ModuleManifest> _availableUpdates;

        /// <summary>
        /// Initializes immutable class with manifests of updates.
        /// </summary>
        /// <param name="availableUpdates">collection of newer manifests</param>
        public AvailableUpdatesEventArgs(List<ModuleManifest> availableUpdates)
        {
            _availableUpdates = availableUpdates;
        }

        /// <summary>
        /// List of newer versions available
        /// </summary>
        public IList<ModuleManifest> AvailableUpdates
        {
            get { return _availableUpdates.AsReadOnly(); }
        }
    }
}