using System;
using System.Windows.Controls;
using Nomad.Communication.EventAggregation;

namespace ThreadedToolbarControlledModule
{
    public class ProgressBarHelper : IDisposable
    {
        private ProgressBar _progressBar = new ProgressBar() {Height = 20};
        
        /// <summary>
        /// Contains progress bar to represent state
        /// </summary>
        public ProgressBar ProgressBar { get { return _progressBar; } }


        public ProgressBarHelper(IEventAggregator eventAggregator)
        {
            _ticket = eventAggregator.Subscribe<ThreadedWorkerProgressMessage>(ProgressChanged, DeliveryMethod.GuiThread);
        }


        /// <summary>
        /// Processes new progress value
        /// </summary>
        /// <param name="obj"></param>
        private void ProgressChanged(ThreadedWorkerProgressMessage obj)
        {
            ProgressBar.Value = obj.Value;
        }


        #region IDisposable

        private bool _disposed;
        private IEventAggregatorTicket<ThreadedWorkerProgressMessage> _ticket;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _ticket.Dispose();
                }
    
                _disposed = true;

            }
        }
        #endregion
    }
}