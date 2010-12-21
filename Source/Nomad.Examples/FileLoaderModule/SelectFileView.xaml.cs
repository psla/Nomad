using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Nomad.Communication.EventAggregation;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace FileLoaderModule
{
    /// <summary>
    /// Interaction logic for SelectFileView.xaml
    /// </summary>
    public partial class SelectFileView : UserControl
    {
        private readonly EventAggregator _eventAggregator;


        public SelectFileView(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                _eventAggregator.Publish(new FileSelectedMessage(fileDialog.FileName));
            }
        }
    }
}
