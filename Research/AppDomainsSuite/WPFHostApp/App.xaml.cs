using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using WPFHostApp.DomainLogic;

namespace WPFHostApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppDomainLauncher Launcher { set; get;}

        protected override void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("Starting AppDomain Launcher");
            
            AppDomainLauncher domainLauncher = new AppDomainLauncher();
            
            var mainWindow = new MainWindow(domainLauncher);
            mainWindow.Show();
        }
    }
}
