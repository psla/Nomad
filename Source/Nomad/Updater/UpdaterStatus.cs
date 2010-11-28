namespace Nomad.Updater
{
    /// <summary>
    ///     State in which is the updater.
    /// </summary>
    public enum UpdaterStatus
    {
        /// <summary>
        ///     No process of update has been initialized.
        /// </summary>
        Idle,
        /// <summary>
        ///     The updater is checking for updates.
        /// </summary>
        Checking,
        /// <summary>
        ///     The updater has checked for updates.
        /// </summary>
        Checked,
        /// <summary>
        ///     The updater is downloading updates.
        /// </summary>
        Preparing,
        /// <summary>
        ///     The updater has checked for updates.
        /// </summary>
        Prepared,
        /// <summary>
        ///     The updater is performing updates.
        /// </summary>
        Performing,
        /// <summary>
        ///     The updater has been pushed into invalid state due to some error.
        /// </summary>
        Invalid
    }
}