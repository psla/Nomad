using System;
using System.Collections.Generic;
using System.Windows.Input;
using Nomad.Communication.EventAggregation;
using Nomad.Updater;

namespace WpfUpdaterModule
{
    /// <summary>
    ///     Logic behind the updater visual component - view.
    /// </summary>
    public class UpdaterViewModel
    {
        private readonly IUpdater _updater;
        private readonly IEventAggregator _eventAggregator;
        private RelayCommand _checkCommand;
        private RelayCommand _prepareCommand;
        private RelayCommand _performCommand;

        public IList<ModuleManifestWrapper> AvaliableUpdates { get; private set; }

        #region Commands

        public ICommand CheckCommand
        {
            get { return _checkCommand; }
        }

        public ICommand PrepareCommand
        {
            get { return _prepareCommand; }
        }

        public ICommand PerformCommand
        {
            get { return _performCommand; }
        }

        #endregion

        public UpdaterViewModel(IUpdater updater, IEventAggregator eventAggregator)
        {
            _updater = updater;
            _eventAggregator = eventAggregator;

            // command initialization with logical values when active 
            // TODO: add support for all types of conditions
            _checkCommand = new RelayCommand(CheckMethod, (x) =>_updater.Status == UpdaterStatus.Idle );
            _prepareCommand = new RelayCommand(PrepareMethod, (x) => _updater.Status == UpdaterStatus.Checked);
            _performCommand = new RelayCommand(PerformMethod, (x) => _updater.Status == UpdaterStatus.Prepared);
        }

        #region Command Method Gropus

        private void PerformMethod(object obj)
        {
            throw new NotImplementedException();
        }


        private void PrepareMethod(object obj)
        {
            throw new NotImplementedException();
        }

        private void CheckMethod(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}