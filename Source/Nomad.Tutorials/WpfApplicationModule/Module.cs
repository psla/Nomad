using System;
using Nomad.Modules;

namespace WpfApplicationModule
{
    public class Module : IModuleBootstraper
    {
        private App _app;

        #region IModuleBootstraper Members

        [STAThread]
        public void OnLoad()
        {
            _app = new App();

            _app.Run(new MainWindow());
        }


        public void OnUnLoad()
        {
            _app.Shutdown();
        }

        #endregion
    }
}