using System;
using Nomad.Modules;

namespace WpfApplicationModule
{
    /// <summary>
    /// Responsible for running wpf application as a module
    /// </summary>
    public class WpfMainApplication : IModuleBootstraper
    {
        private App _app;

        #region IModuleBootstraper Members

        /// <summary>
        /// Start WPF application.
        /// </summary>
        [STAThread]
        public void OnLoad()
        {
            _app = new App();

            _app.Run(new MainWindow());
        }


        /// <summary>
        /// Kill all threads when unload. Shutdown the application
        /// </summary>
        public void OnUnLoad()
        {
            _app.Shutdown();
        }

        #endregion
    }
}