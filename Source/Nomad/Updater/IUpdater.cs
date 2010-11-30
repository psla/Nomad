using System.Collections.Generic;
using Nomad.Core;
using Nomad.Messages.Updating;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater.ModuleRepositories;

namespace Nomad.Updater
{
    /// <summary>
    ///     Describes the behavior needed by updater.
    /// </summary>
    public interface IUpdater
    {
        /// <summary>
        ///    Gets the state of the updater.
        /// </summary>
        UpdaterStatus Status { get; }


        /// <summary>
        /// Runs update checking. For each discovered module performs check for update.
        /// If such check finds higher version of assembly, than new <see cref="ModuleManifest"/> will be in result
        /// </summary>
        /// <remarks>
        /// Method return result by <see cref="NomadAvailableUpdatesMessage"/> event, so it may be invoked asynchronously
        /// </remarks>
        void CheckUpdates(IModuleDiscovery moduleDiscovery);


        /// <summary>
        ///     Prepares update for available updates. Result returned as message.
        /// </summary>
        /// <remarks>
        ///     Using provided <see cref="IModulesRepository"/> downloads all modules and their dependencies.
        /// </remarks>
        /// <param name="nomadAvailableUpdates">modules to install. </param>
        void PrepareUpdate(NomadAvailableUpdatesMessage nomadAvailableUpdates);


        /// <summary>
        ///     Starts update process
        /// </summary>
        /// <remarks>
        /// Using provided <see cref="IModulesOperations"/> it unloads all modules, 
        /// than it places update files into modules directory, and loads modules back.
        /// 
        /// Upon success or failure sets the flag <see cref="Updater.Status"/> with corresponding value.
        /// </remarks>
        /// <param name="modulePackages">collection of packages to install</param>
        void PerformUpdates(IEnumerable<ModulePackage> modulePackages);
    }
}