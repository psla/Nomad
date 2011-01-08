using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Nomad.Communication.EventAggregation;
using Nomad.Messages.Updating;
using Nomad.Modules.Manifest;
using Nomad.Updater;

namespace WpfUpdaterModule
{
    /// <summary>
    ///     Logic behind the updater visual component - view.
    /// </summary>
    public class UpdaterViewModel : INotifyPropertyChanged
    {
        private readonly RelayCommand _checkCommand;
        private readonly RelayCommand _performCommand;
        private readonly RelayCommand _prepareCommand;

        private readonly IEventAggregator _eventAggregator; 
        private readonly IUpdater _updater;


        public UpdaterViewModel(IUpdater updater, IEventAggregator eventAggregator)
        {
            _updater = updater;
            _eventAggregator = eventAggregator;

            AvaliableUpdates = new List<ModuleManifestWrapper>();
            Info = "Idle";

            // command initialization, execution and and canExecute logic
            _checkCommand = new RelayCommand(CheckMethod, CheckPredicate);
            _prepareCommand = new RelayCommand(PrepareMethod, PreparePredicate);
            _performCommand = new RelayCommand(PerformMethod, PerformPredicate);

            // subscribe for updater events
            _eventAggregator.Subscribe<NomadAvailableUpdatesMessage>(UpdatedAvaliableCallback);
            _eventAggregator.Subscribe<NomadUpdatesReadyMessage>(UpdatesReadyCallback);
        }

        #region Update Event Handlers Groups

        private void UpdatesReadyCallback(NomadUpdatesReadyMessage message)
        {
            // check if in the preparing was an error
            if (message.Error)
            {
                Info = message.Message;
            }

            Info = _updater.Status.ToString();
            InvokePropertyChanged("Info");
        }


        private void UpdatedAvaliableCallback(NomadAvailableUpdatesMessage obj)
        {
            // check if there was an error
            if(obj.Error)
            {
                Info = obj.Message;
                InvokePropertyChanged("Info");
                return;
            }

            // update the view model based on 
            var listOfUpdates = obj.AvailableUpdates.Select(x => new ModuleManifestWrapper()
                                                                     {
                                                                         Manifest = x,
                                                                         SelectedForUpdate = false
                                                                     });

            AvaliableUpdates = new List<ModuleManifestWrapper>(listOfUpdates);
            Info = _updater.Status.ToString();

            InvokePropertyChanged("AvaliableUpdates");
            InvokePropertyChanged("Info");
        }

        #endregion

        #region Command Predicate Groups

        private bool PerformPredicate(object obj)
        {
            return _updater.Status == UpdaterStatus.Prepared;
        }


        private bool PreparePredicate(object obj)
        {
            return _updater.Status == UpdaterStatus.Checked;
        }


        private bool CheckPredicate(object obj)
        {
            return _updater.Status == UpdaterStatus.Idle ||
                   _updater.Status == UpdaterStatus.Checked ||
                   _updater.Status == UpdaterStatus.Prepared;
        }

        #endregion

        #region Command Method Groups

        private void PerformMethod(object obj)
        {
            // invoke the update
            _updater.PrepareUpdate(null);
        }


        private void PrepareMethod(object obj)
        {
            // get list of selected updates from user
            List<ModuleManifest> listForUpdate = AvaliableUpdates
                .Where(x => x.SelectedForUpdate)
                .Select(x => x.Manifest)
                .ToList();

            // pass this list to updater to perform checking and download
            _updater.PrepareUpdate(new List<ModuleManifest>(listForUpdate));
        }


        private void CheckMethod(object obj)
        {
            // invoke this method to check for updates
            _updater.CheckUpdates();
        }

        #endregion

        public IList<ModuleManifestWrapper> AvaliableUpdates { get; private set; }
        public string Info { get; private set; }

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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}