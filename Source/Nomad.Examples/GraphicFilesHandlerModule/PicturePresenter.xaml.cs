using System;
using System.Collections.Generic;
using System.Drawing;
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
using Nomad.Regions.Behaviors;

namespace GraphicFilesHandlerModule
{
    /// <summary>
    /// Interaction logic for PicturePresenter.xaml
    /// </summary>
    public partial class PicturePresenter : UserControl, IActiveAware, IHaveTitle
    {
        private readonly GraphicFilesEditToolbar _graphicFilesEditToolbar;

        public PicturePresenter(string imagePath, GraphicFilesEditToolbar graphicFilesEditToolbar)
        {
            _graphicFilesEditToolbar = graphicFilesEditToolbar;
            InitializeComponent();
            Title = imagePath;
            Image.Source = new BitmapImage(new Uri(imagePath));
        }

        public void SetIsActive(bool isActive)
        {
            if (isActive)
                _graphicFilesEditToolbar.Visibility = Visibility.Visible;
            else
                _graphicFilesEditToolbar.Visibility = Visibility.Collapsed;
        }


        public string Title { get; private set; }
    }
}
