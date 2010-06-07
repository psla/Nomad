using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NLog;
using WPFHostApp.DomainLogic;

namespace WPFHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private AppDomainLauncher launcher;

        public MainWindow(AppDomainLauncher launcher)
        {
            this.launcher = launcher;
            logger.Info("On WPF startup");
            InitializeComponent();
            
            CodeToTestSimpleFactory factory = new CodeToTestSimpleFactory();
            //explicitly split into to separete things for introducing another changes
            CodeToTestList codeToTestList = new CodeToTestList(factory.ListOfCodeTests);
            containerListBox.DataContext = codeToTestList;
        }
     
        private void RunButtonClick(object sender, RoutedEventArgs e)
        {
            //Gets the selected container thing and passes it to the luncher.
            if (threadedCheckBox.IsChecked.HasValue && newDomainCheckBox.IsChecked.HasValue)
            {
                launcher.IsRunInNewThread = threadedCheckBox.IsChecked.Value;
                launcher.IsRunInNewDomain = newDomainCheckBox.IsChecked.Value;
            }

            try
            {
                launcher.Run(GetCodeToRun());
            }
            catch (ArgumentNullException exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
        }

        private Action<AppDomain> GetCodeToRun()
        {
            var item = this.containerListBox.SelectedItem as ICodeToTest;
            if (item == null)
            {
                logger.Error("item == null, means nothing was selected");
                throw new ArgumentNullException("No item selected");
            }
                
            return item.GetCode();            
        }
    }
}
