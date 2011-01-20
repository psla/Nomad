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


        public void HideAllStatus()
        {
            cancelled.Visibility = Visibility.Collapsed;
            ready.Visibility = Visibility.Collapsed;
            calculating.Visibility = Visibility.Collapsed;
            calculated.Visibility = Visibility.Collapsed;
        }

        public void ShowCancelled()
        {
            HideAllStatus();
            cancelled.Visibility = System.Windows.Visibility.Visible;
        }

        public void ShowCalculating()
        {
            HideAllStatus();
            calculating.Visibility = System.Windows.Visibility.Visible;
        }

        public void ShowReady()
        {
            HideAllStatus();
            ready.Visibility = System.Windows.Visibility.Visible;
        }

        public void ShowCalculated()
        {
            HideAllStatus();
            ready.Visibility = System.Windows.Visibility.Visible;
        }
    }
}