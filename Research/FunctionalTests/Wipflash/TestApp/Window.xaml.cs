using System;
using System.ComponentModel;

namespace WipflashFunctionalTests.TestApp
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        public bool HasBeenClicked { get; set; }

        public Window()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ButtonClicked(object sender, EventArgs e)
        {
            HasBeenClicked = true;
            PropertyChanged(this, new PropertyChangedEventArgs("HasBeenClicked"));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
