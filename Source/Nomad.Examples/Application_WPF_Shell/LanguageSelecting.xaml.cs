using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Nomad.Communication.EventAggregation;
using Nomad.Messages;

namespace Application_WPF_Shell
{
    /// <summary>
    /// Interaction logic for LanguageSelecting.xaml
    /// </summary>
    public partial class LanguageSelecting : UserControl
    {
        private readonly IEventAggregator _eventAggregator;


        public LanguageSelecting(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
        }


        private void SetEnglish(object sender, RoutedEventArgs e)
        {
            _eventAggregator.Publish(new NomadCultureChangedMessage(new CultureInfo("en-GB"), ""));
        }


        private void SetPolish(object sender, RoutedEventArgs e)
        {
            _eventAggregator.Publish(new NomadCultureChangedMessage(new CultureInfo("pl-PL"), ""));
        }
    }
}