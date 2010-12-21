using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextFileHandlerModule
{
    /// <summary>
    /// Interaction logic for TextPresenter.xaml
    /// </summary>
    public partial class TextPresenter : UserControl
    {
        private readonly string _filename;


        public TextPresenter(string filename)
        {
            _filename = filename;
            InitializeComponent();

            TextBox.Text = string.Join("\n", File.ReadAllLines(filename));
        }
    }
}
