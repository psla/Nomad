namespace Nomad.Updater
{
    /// <summary>
    ///     Describes the type of the process of update.
    /// </summary>
    public enum UpdaterType
    {
        /// <summary>
        ///     All actions must be invoked separately.
        /// </summary>
        /// <remarks>
        ///     Meaning that chain of the actions: 
        /// check for updates, prepare updates (download) then perform updates (install)
        /// </remarks>
        Manual,

        /// <summary>
        ///     Only initialization is needed, thus check for updates.
        /// </summary>
        Automatic
    }
}