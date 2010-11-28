using System.Collections.Generic;
using Nomad.Modules.Manifest;
using Nomad.Updater;

namespace Nomad.Messages.Updating
{
    /// <summary>
    ///     Contains information about updates that are available in repository.
    /// </summary>
    /// <remarks>
    ///     Basing on this information you may call <see cref="Updater.PrepareUpdate"/> which will download all available updates
    /// </remarks>
    public class NomadAvailableUpdatesMessage : NomadMessage
    {
        private readonly List<ModuleManifest> _availableUpdates;
        private readonly bool _error;


        /// <summary>
        ///     Initializes immutable class with manifests of updates.
        /// </summary>
        /// <param name="availableUpdates">collection of newer manifests.</param>
        public NomadAvailableUpdatesMessage(List<ModuleManifest> availableUpdates)
            : this(availableUpdates, false, string.Empty)
        {
        }


        /// <summary>
        ///     Initializes immutable class with manifests of updates.
        /// </summary>
        /// <param name="availableUpdates">Collection of newer manifests.</param>
        /// <param name="error">Specifies if the available updates is valid.</param>
        /// <param name="message">Message specifying the error during loading.</param>
        public NomadAvailableUpdatesMessage(List<ModuleManifest> availableUpdates, bool error,
                                          string message) : base(message)
        {
            _availableUpdates = availableUpdates;
            _error = error;
        }


        /// <summary>
        ///     Describes if there were an error during checking for updates or not.
        /// </summary>
        public bool Error
        {
            get { return _error; }
        }

        /// <summary>
        ///     List of newer versions available
        /// </summary>
        public IList<ModuleManifest> AvailableUpdates
        {
            get { return _availableUpdates.AsReadOnly(); }
        }


        public override string ToString()
        {
            return Message;
        }
    }
}