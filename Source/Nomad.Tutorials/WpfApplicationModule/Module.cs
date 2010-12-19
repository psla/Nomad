using System;
using Nomad.Modules;

namespace WpfApplicationModule
{
    public class Module : IModuleBootstraper
    {
        private App _app;
        [STAThread()]
        public void OnLoad()
        {
            _app = new App();

            _app.Run(new MainWindow());
            //var window = new MainWindow();
            //window.Show();
        }


        public void OnUnLoad()
        {
            //_app.Shutdown();
        }
    }
}