using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ThreadedToolbarControlledModule
{
    /// <summary>
    /// Interaction logic for ThreadedStatusBar.xaml
    /// </summary>
    public partial class ThreadedStatusBar : UserControl
    {
        public ThreadedStatusBar()
        {
            InitializeComponent();
        }


        private void HideAllStatus()
        {
            cancelled.Visibility = Visibility.Collapsed;
            ready.Visibility = Visibility.Collapsed;
            calculating.Visibility = Visibility.Collapsed;
            calculated.Visibility = Visibility.Collapsed;
        }

        public void ShowCancelled()
        {
            Dispatcher.BeginInvoke((ThreadStart) delegate
                                       {
                                           HideAllStatus();
                                           cancelled.Visibility = System.Windows.Visibility.Visible;
                                       });
        }

        public void ShowCalculating()
        {
            Dispatcher.BeginInvoke((ThreadStart) delegate
                                                     {
                                            HideAllStatus();
                                            calculating.Visibility = System.Windows.Visibility.Visible;
                                                     });
        }

        public void ShowReady()
        {
            Dispatcher.BeginInvoke((ThreadStart) delegate
                                                     {
                                             HideAllStatus();
                                             ready.Visibility = System.Windows.Visibility.Visible;
                                                     });
        }

        public void ShowCalculated()
        {
            Dispatcher.BeginInvoke((ThreadStart) delegate
                                                     {
                                            HideAllStatus();
                                            ready.Visibility = System.Windows.Visibility.Visible;
                                                     });
        }
    }
}