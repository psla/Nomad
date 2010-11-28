using System.Collections.Generic;
using Nomad.Updater;

namespace Nomad.Messages.Updating
{
    /// <summary>
    /// C   ontains whole updates packages. Called when all required updates downloaded
    /// </summary>
    public class NomadUpdatesReadyMessage : NomadMessage
    {
        private readonly bool _error;
        private readonly List<ModulePackage> _modulePackages;


        ///<summary>
        /// Initializes immutable class with information about ready Update Packages.
        ///</summary>
        ///<param name="modulePackages"></param>
        public NomadUpdatesReadyMessage(List<ModulePackage> modulePackages)
            : this(modulePackages, false, string.Empty)
        {
        }


        private NomadUpdatesReadyMessage(List<ModulePackage> modulePackages, bool error, string message)
            : base(message)
        {
            _modulePackages = modulePackages;
            _error = error;
        }


        /// <summary>
        ///     Describes if there were an error during preparing updates or not.
        /// </summary>
        public bool Error
        {
            get { return _error; }
        }

        /// <summary>
        ///     Update elements. Completely ready.
        /// </summary>
        public IList<ModulePackage> ModulePackages
        {
            get { return _modulePackages.AsReadOnly(); }
        }


        public override string ToString()
        {
            return Message;
        }
    }
}