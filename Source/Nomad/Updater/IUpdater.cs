using System.Collections.Generic;
using System.Threading;
using Nomad.Core;
using Nomad.Messages.Updating;
using Nomad.Modules.Discovery;
using Nomad.Modules.Manifest;
using Nomad.Updater.ModulePackagers;
using Nomad.Updater.ModuleRepositories;

namespace Nomad.Updater
{
    /// <summary>
    ///     Describes the behavior needed by updater.
    /// </summary>
    public interface IUpdater
    {
        /// <summary>
        ///     <see cref="EventWaitHandle"/> for blocking other threads until update is finished.
        /// </summary>
        AutoResetEvent UpdateFinished { get; }

        /// <summary>
        ///    Gets the state of the updater. Used to signalize potential problems.
        /// </summary>
        UpdaterStatus Status { get; }

        /// <summary>
        ///     Gets or sets the mode in which updater is working. 
        /// </summary>
        UpdaterType Mode { get; set; }

        /// <summary>
        ///     Gets or sets the default <see cref="IModuleDiscovery"/> of modules to be loaded <c>after</c> update. 
        /// </summary>
        /// <remarks>
        ///     This feature should be used in conjunction with <see cref="UpdaterType.Automatic"/> mode.
        /// </remarks>
        IModuleDiscovery DefaultAfterUpdateModules { get; set; }

        /// <summary>
        ///     Runs update checking using provided <see cref="IModulesRepository"/>.
        /// </summary>
        /// <remarks>
        ///     Method return result by <see cref="NomadAvailableUpdatesMessage"/> event, so it may be invoked asynchronously
        /// </remarks>
        void CheckUpdates();


        /// <summary>
        ///     Prepares update for available updates. Result returned as message.
        /// </summary>
        /// <remarks>
        ///     Using provided <see cref="IModulesRepository"/> downloads all modules and their dependencies. Methods results
        /// by message, though it can invoked asynchronously.
        /// </remarks>
        /// <param name="avaliableUpdates">List of updates to download.</param>
        void PrepareUpdate(IEnumerable<ModuleManifest> avaliableUpdates);


        /// <summary>
        ///     Starts update process.
        /// </summary>
        /// <param name="discovery">The discovery of modules to be loaded <c>after</c> update. Null value uses default.</param>
        /// <remarks>
        /// <para>
        /// Using provided <see cref="IModulesOperations"/> it unloads all modules, then uses provided <see cref="IModulePackager"/> to
        /// switch the updates. After that performs the load of the modules using proivded <paramref name="discovery"/>. If discovery is <c>null</c>
        /// then <see cref="DefaultAfterUpdateModules"/> will be used.
        /// </para>
        /// <para>
        /// Upon success or failure sets the flag <see cref="NomadUpdater.Status"/> with corresponding value.
        /// </para>
        /// </remarks>
        void PerformUpdates(IModuleDiscovery discovery);
    }
}