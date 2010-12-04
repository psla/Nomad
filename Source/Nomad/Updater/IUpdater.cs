using System.Collections.Generic;
using System.Threading;
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
        AutoResetEvent UpdateFinished { get; }

        /// <summary>
        ///    Gets the state of the updater. Used to signalize potential threading problems.
        /// </summary>
        UpdaterStatus Status { get; }

        /// <summary>
        ///     Gets or sets the mode in which updater is working. 
        /// </summary>
        UpdaterType Mode { get; set; }

        /// <summary>
        ///     Gets or sets the default <see cref="IModuleDiscovery"/> of modules to be loaded <c>after</c> update.
        /// </summary>
        IModuleDiscovery DefaultAfterUpdateModules { get; set; }

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
        /// <param name="avaliableUpdates">List of updates to download.</param>
        void PrepareUpdate(IEnumerable<ModuleManifest> avaliableUpdates);


        /// <summary>
        ///     Starts update process.
        /// </summary>
        /// <param name="discovery">The discovery of modules to be loaded after update.</param>
        /// <remarks>
        /// Using provided <see cref="IModulesOperations"/> it unloads all modules, 
        /// than it places update files into modules directory, and loads modules back.
        /// 
        /// Upon success or failure sets the flag <see cref="Updater.Status"/> with corresponding value.
        /// </remarks>
        void PerformUpdates(IModuleDiscovery discovery);
    }
}