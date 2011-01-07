using System;
using Nomad.Updater;

namespace WpfUpdaterModule
{
    /// <summary>
    ///     Logic behind the updater visual component - view.
    /// </summary>
    public class UpdaterViewModel
    {
        private readonly IUpdater _updater;


        public UpdaterViewModel(IUpdater updater)
        {
            _updater = updater;
        }
    }
}