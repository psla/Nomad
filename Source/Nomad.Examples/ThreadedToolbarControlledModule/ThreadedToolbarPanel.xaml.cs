using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Nomad.Communication.EventAggregation;

namespace ThreadedToolbarControlledModule
{
    /// <summary>
    /// Interaction logic for ThreadedToolbarPanel.xaml
    /// </summary>
    /// <remarks>
    /// Threaded worker searches for all pictures in last directory provided, loads all, transform to black & white and after finish adds region transformed images
    /// When not fully finished (not all images transformed) it adds only pictures, that were transformed
    /// </remarks>
    public partial class ThreadedToolbarPanel : ToolBar
    {
        private readonly BackgroundWorker _bgWorker;
        private readonly IEventAggregator _eventAggregator;


        public ThreadedToolbarPanel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            _bgWorker = new BackgroundWorker
                            {
                                WorkerReportsProgress = true,
                                WorkerSupportsCancellation = true
                            };
            _bgWorker.DoWork += WorkerMethod;
            _bgWorker.ProgressChanged += HandleProgressChanged;
        }


        private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _eventAggregator.Publish(new ThreadedWorkerProgressMessage(e.ProgressPercentage));
        }


        private void WorkerMethod(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            for (int i = 1; i < 100 && !bw.CancellationPending; i++)
            {
                Thread.Sleep(100);
                _bgWorker.ReportProgress(i);
            }
        }


        private void StopMe(object sender, RoutedEventArgs e)
        {
            _bgWorker.CancelAsync();
        }


        private void RunMe(object sender, RoutedEventArgs e)
        {
            if (!_bgWorker.IsBusy)
            {
                _bgWorker.RunWorkerAsync();
            }
        }
    }
}